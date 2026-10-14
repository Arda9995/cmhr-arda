# Google AI Feed and XML Archive — Change Record

This document records the changes and validation completed during the investigation, including the rolling 48-hour archive and creating-editor fallback for `dc:creator`. Measurements and database values below describe the observed runs, not guaranteed current state. Historical validation results are identified separately from the current implementation.

## 1. Final switch behavior

The CMS switch now uses positive wording:

- English: **Include in Google AI Feed**
- Turkish: **Google AI Feed'e Dahil Et**

| UI switch | Request property `AllowGoogleAiFeed` | Stored `ExcludeFromGoogleAiFeed` | Meaning |
|---|---|---|---|
| ON | `true` | `false` | Allow eligible articles in the Google AI feed and archive |
| OFF | `false` | `true` | Exclude the article from the Google AI feed and archive |

`NewsRequest.AllowGoogleAiFeed` inversely maps to the existing `ExcludeFromGoogleAiFeed` property. The database column and service-side exclusion checks retain their original meaning. No data migration or bulk reversal of existing article settings was performed.

The previous excluded-by-default behavior for new requests is preserved: the new switch defaults to OFF. Existing included articles display ON; existing excluded articles display OFF.

### Changed files

- `Quark.CMS.Business/Models/Requests/NewsRequests/NewsRequest.cs`
- `Quark.CMS/Views/News/_Edit/_Permissions.cshtml`
- `Quark.Resources/Resources/CommonResource.en.resx`
- `Quark.Resources/Resources/CommonResource.tr.resx`

## 2. Archive output and eligibility

Archive base directory:

`E:\GitHub\cgazete-google-rss\GoogleAiFeedArchive`

File format:

`<base>\<year>\feed-YYYY-MM-DD.xml`

Each refresh rebuilds **today's file containing all eligible articles from the preceding 48 hours**, including articles published on previous days. The filename's year/date come from the refresh time using the application's existing local-time convention, not the triggering article's publication date. For example, a refresh on September 9, 2026 writes `GoogleAiFeedArchive\2026\feed-2026-09-09.xml`. Subsequent refreshes that day replace the same file; no `(1)`, `(2)`, or other numbered files are created. Previous dates and previously generated numbered files remain historical snapshots.

The archive uses the live feed's full RSS format: `content`, `media`, `atom`, and `dc` namespaces; channel logo, TTL, Atom self-link; article enclosure, media image dimensions, and CDATA text. The existing feed convention of JPEG enclosures with a `50000` length value is retained; this value is not a measured image byte count. Actual image URLs/dimensions come from the selected published photo (high-resolution first, then main, then cover), with the existing fallback image behavior. The archive directory stores XML, not image binaries.

Articles must:

- Exist in the published-news table.
- Have `AllowToSendRSS = true`.
- Have `ExcludeFromGoogleAiFeed = false` (the new UI switch is ON).
- Be of the supported News or Column Articles type.
- Have a publication timestamp between the refresh time minus 48 hours and the refresh time, inclusive. Future-dated articles are excluded.

The live Google AI RSS feed and today's archive use the same rolling 48-hour query and RSS generator. The previous Google AI 100-item limit has been removed; all eligible articles are included. Limits on other RSS feeds are unchanged. An existing article ID supplied to `ArchiveGoogleAiNewsAsync` triggers a refresh of today's complete rolling archive, not a single-article export or a rebuild of that article's publication day.

Publishing, unpublishing, and ordinary live-feed refreshes update today's archive. The existing Hangfire startup job and three-minute recurring Google AI RSS job also use this path, so articles that age out are removed on the next refresh even without a publication event. Scheduled updates require the Hangfire host to be running.

Daily files reflect the published records at refresh time: switching OFF and publishing removes the article from today's file and the live feed. If the rolling window has no eligible articles, today's file is written as valid RSS with no items. Previous daily files are not continuously filtered or retroactively regenerated. No automatic deletion of older or numbered files has been implemented.

Turning the switch ON does not bypass the other eligibility rules. Changes must be saved and published so that the published record reflects them.

## 3. Problems found and fixes

### A. Missing photo stopped publishing before archive generation

A publish exception was traced to `RefreshNewsPublishedSearchByID`: the search projection called `TrimStart()` on a missing main/cover photo URL.

Both single-article and bulk published-news search refreshes now handle missing or empty photo URLs and return an empty cover-photo value instead of throwing.

File: `Quark.Business.Caching/Services/NewsCachingService.cs`

### B. Unrelated cache failures prevented archive generation

A later publish log showed a SQL timeout in `RefreshNewsBySourceSlug`. The Google AI refresh previously ran after this operation and therefore was never reached when that operation threw.

`HandlePublishCacheOperations` and `HandleUnPublishCacheOperations` now invoke `RefreshGoogleAiRSS()` once before unrelated cache work. This shared operation builds and writes today's rolling archive before updating the live Google AI feed in Elasticsearch. Separate publication-day archive calls were removed to avoid duplicate refreshes.

This fixes the archive's dependency on those operations. It does **not** fix the separate source-cache SQL timeout or guarantee that the complete publish HTTP request succeeds.

### C. Local XML depended on Elasticsearch

Archive writing previously ran after the Elasticsearch update. `RefreshGoogleAiRSS()` now uses `BuildAndArchiveGoogleAiFeedAsync()` to build and atomically write the XML before attempting the Elasticsearch update. An Elasticsearch update failure therefore does not undo the archive already written. `RefreshGoogleAiArchiveAsync()` provides an archive-only entry point. Failures are logged and reported through the corresponding service result; an archive failure prevents the combined operation from proceeding to the Elasticsearch update.

File: `Quark.Business.Caching/Services/XmlFeedCachingService.cs`

### D. Empty windows could retain stale output

The archive previously wrote only days with eligible articles. Excluding or unpublishing the last eligible article could leave a stale file unchanged.

The daily writer now rebuilds even when the triggering published article is excluded, and unpublishing refreshes today's rolling file. If no eligible articles remain in the 48-hour window, the file contains an empty channel rather than stale items. An unknown article ID passed to `ArchiveGoogleAiNewsAsync` does not create a file.

### E. Duplicate UTF-8 markers produced invalid XML

Validation detected the byte prefix `EF-BB-BF-EF-BB-BF`: serialization included a UTF-8 byte-order mark, and file writing added a second one.

Archive file writing avoids adding the second marker. Files are written to unique temporary files and then replace the canonical daily XML. An exclusive `GoogleAiFeedArchive\google-ai.lock` file serializes CMS/Hangfire archive writers before capturing the refresh date and executing the database query, including refreshes across midnight. Sharing violations are retried for up to roughly 10 seconds. Temporary files are cleaned up on failure. Other dates and pre-existing numbered XML files are not modified.

## 4. Archive validation performed

A separate diagnostic runner was added:

- `tools/GoogleAiArchiveCheck/GoogleAiArchiveCheck.csproj`
- `tools/GoogleAiArchiveCheck/Program.cs`

Its `--write-archive` mode invokes the real archive service for a supplied news ID, reads the approved development database, and simulates an Elasticsearch update failure without contacting Elasticsearch. The current checks compare rolling-window item URLs against eligible database records, verify RSS/media/CDATA structure and creator attribution, confirm repeated refreshes reuse today's filename, and check unrelated-file checksums and unchanged article settings. They also check that a missing article ID does not create an archive and that the combined live-feed refresh maintains today's archive despite the simulated Elasticsearch failure.

**Warning:** This mode rebuilds and overwrites today's daily file multiple times: two archive refreshes followed by a combined refresh with a simulated Elasticsearch failure. It does not modify the database or unrelated XML files. It is an explicit maintenance/check command, not a read-only filesystem inspection. The table below describes earlier validation; section 11 records the subsequent database-backed validation of the rolling-window and creating-editor changes.

Example from the solution root:

```powershell
dotnet run --project .\tools\GoogleAiArchiveCheck\GoogleAiArchiveCheck.csproj -- "E:\GitHub\cgazete-google-rss" --write-archive 2440937
```

Historical successful validation of the earlier daily-writer implementation (before sequential single-article files):

| File in `GoogleAiFeedArchive\2026` | Eligible items | Size | Result |
|---|---:|---:|---|
| `feed-2026-09-06.xml` | 0 | 651 bytes | Valid RSS/XML |
| `feed-2026-09-07.xml` | 1 | 1,086 bytes | Valid RSS/XML |
| `feed-2026-09-08.xml` | 1 | 1,916 bytes | Valid RSS/XML; included article 2440937 |

During that validation, article 2440937's stored exclusion flag was `false`. The diagnostic did not change it. XML creation succeeded despite the deliberately simulated Elasticsearch failure; the corresponding error log was expected.

## 5. Switch regression checks

Added:

- `tools/GoogleAiFeedSwitchCheck/GoogleAiFeedSwitchCheck.csproj`
- `tools/GoogleAiFeedSwitchCheck/Program.cs`

Run:

```powershell
dotnet run --project .\tools\GoogleAiFeedSwitchCheck\GoogleAiFeedSwitchCheck.csproj
```

All six checks passed using actual ASP.NET Core model binding or the real request model:

1. ON with Razor's additional hidden `false` value allows the article.
2. OFF posts `false` and excludes the article.
3. A missing switch preserves the existing excluded-by-default behavior.
4. Legacy `ExcludeFromGoogleAiFeed=true` posts still exclude.
5. Legacy `ExcludeFromGoogleAiFeed=false` posts still allow.
6. Loading saved settings and switching ON/OFF preserve the inverse mapping.

These checks do not access the database or external services. The solution build also succeeded after the switch changes.

## 6. Earlier SQL fix and remaining issues

A separate, approved index was created on `News-Migration` for the published-news picker:

- Table: `dbo.[News.NewsPublished]`
- Index: `IX_NewsPublished_NewsTypeID_Include_NewsID`
- Key: `NewsTypeID`
- Included column: `NewsID`
- Deployment script: `Quark.Data/Scripts/NewsPublishedCountIndex.sql`

For the measured empty-search count for news 2440937, elapsed SQL time fell from 28,091 ms to 14,048 ms and published-table logical reads from 1,338,385 to 6,356. The filtered count remained 1,231,342. Other post-index executions varied; server scheduling and I/O delays remained visible. Application timeouts were not increased.

This index does not fix the separate media-gallery queries. Saved logs also showed timeouts in `MediaGalleryService.GetFolderImagesAsync` during counting and paging. Their targeted measurement/fix remained pending the failing folder's request parameters.

No successful end-to-end browser validation of the entire publish or gallery workflow was claimed from the SQL or archive checks alone.

## 7. Applying and checking the changes

1. Restart the CMS and relevant Web/Hangfire hosts with the rebuilt assemblies. The latest changes were not applied to the app during the active debugging session. Restart debugging, or use Hot Reload where supported. Keep Hangfire running for the three-minute scheduled refresh.
2. Reload the news editor so it displays the new positive switch and labels.
3. Set **Include in Google AI Feed** to ON, ensure RSS is enabled, then save/publish an eligible article.
4. Verify `GoogleAiFeedArchive\<current-year>\feed-<today's-date>.xml` contains every eligible article from the preceding 48 hours, without future-dated articles or a 100-item cap. Publish again and verify the same file updates without a numbered duplicate.
5. Verify item descriptions, style-free HTML inside `content:encoded` CDATA, and `dc:creator`. An assigned author name takes precedence; otherwise the name of the editor identified by `News.CreatedBy` is used.
6. Set the switch OFF and save/publish again to remove the article from today's file and the live feed. Unpublishing also refreshes today's file. Previous daily files and legacy numbered snapshots remain unchanged.
7. If the daily file does not update, inspect the `Google AI daily archive`, `Google AI archive for news`, and `Google AI live feed refresh failed` log entries, along with Hangfire execution and filesystem permissions.

The archive path is currently hard-coded and refers to the machine running the service. That process must have write permission. Running the service on another server does not write to the developer computer's `E:` drive.

The source-cache timeout and media-gallery timeout investigations remain separate follow-up work.

## 8. Article content and RSS format

The shared generator retains RSS 2.0 with the existing content, media, Atom, and Dublin Core namespaces. Channel `language` is now `tr`, and channel dates use the captured refresh time. Each item includes a plain-text `description` derived from the article subtitle and full article HTML in `content:encoded` CDATA.

`CleanGoogleAiContent` uses the existing HtmlAgilityPack dependency to:

- Remove `style` and `class` attributes.
- Remove `style` and `script` elements and stylesheet links.
- Remove inline event-handler attributes and JavaScript/VBScript values in `href`, `src`, and `xlink:href` attributes.
- Preserve semantic HTML, paragraphs, emphasis, images, ordinary links, and meaningful attributes such as image `src`/`alt`.

Description cleanup removes markup and decodes HTML entities. No package was added or upgraded for these changes. This is feed-specific cleanup, not a general-purpose HTML security sanitizer.

## 9. Missing dc:creator and creating-editor fallback

Previously, the feed emitted `dc:creator` only when `NewsPublished.AuthorID` resolved to an author with a nonblank name. Articles without that optional author therefore had no creator element, even when an editor had created the article.

The shared query now resolves the creating editor through:

`NewsPublished.NewsID -> News.ID -> News.CreatedBy -> Users.ID`

`CreateGoogleAiCreator` applies these rules:

1. Use a nonblank assigned author name when available.
2. Otherwise use the creating editor's first and last name.
3. Trim the selected name and serialize it as a Dublin Core `creator` element, with normal XML escaping.
4. If neither name is available, omit the element rather than inventing a name or exposing an email address.

This uses the account that originally created the article, not `UpdatedBy` or `PublishedBy`. Missing article/account relationships do not discard an otherwise eligible feed item. The behavior applies to both live RSS and newly refreshed daily archives; existing historical XML files are not rewritten automatically.

## 10. Files changed and latest validation

Rolling-window and creator changes were made in:

- `Quark.Business.Caching/Interfaces/IXmlFeedCachingService.cs`: replaced the publication-day refresh contract with `RefreshGoogleAiArchiveAsync()`.
- `Quark.Business.Caching/Services/XmlFeedCachingService.cs`: inclusive 48-hour filtering, no Google AI item cap, combined archive/live refresh, atomic current-day writing, content cleanup, descriptions, and creating-editor attribution.
- `Quark.Business.Caching/Services/NewsCachingService.cs`: publish/unpublish hooks call the combined refresh once before other cache work.
- `tools/GoogleAiArchiveCheck/Program.cs`: updated archive assertions and added offline content, window, and creator checks.

Run the offline checks from the solution root:

```powershell
dotnet run --project .\tools\GoogleAiArchiveCheck\GoogleAiArchiveCheck.csproj -- --check-content
dotnet run --project .\tools\GoogleAiArchiveCheck\GoogleAiArchiveCheck.csproj -- --check-window
dotnet run --project .\tools\GoogleAiArchiveCheck\GoogleAiArchiveCheck.csproj -- --check-creator
```

Latest observed results:

- 13 HTML/CDATA cases and the description-cleanup check passed, including Turkish text, preserved images/markup, CSS/script removal, and CDATA round trips.
- Two rolling-window scenarios passed, each containing 154 eligible articles. Coverage includes the exact 48-hour cutoff, future-date exclusion, opt-outs, previous-day inclusion, and year rollover.
- 10 creator-attribution cases passed, covering assigned-author priority, editor fallback, trimmed/missing names, the `dc` namespace, and XML escaping.
- The solution build passed after the rolling-window changes.
- During creator validation, the normal Visual Studio build was blocked by the active debugging session. The check project and its dependencies were instead built to `tools/GoogleAiArchiveCheck/bin/creator-validation` using `--artifacts-path`; that build completed with zero errors and 20 existing warnings, and all offline checks above passed from that isolated output.
- Earlier restore/build output reported existing `NU1903` for `System.Linq.Dynamic.Core` 1.4.5. No dependency versions were changed or vulnerability fixes claimed.

These offline modes do not access the news database or Elasticsearch and do not regenerate archive XML. Database-backed archive validation was subsequently completed as recorded below. No end-to-end browser validation of the publishing workflow or real Elasticsearch update is claimed. Deploy/restart the updated hosts and trigger a feed refresh before expecting those hosts' output to change.

## 11. Actual archive creator verification — September 10, 2026

After a further report that today's XML still lacked `dc:creator`, the exact reported file was checked:

`E:\GitHub\cgazete-google-rss\GoogleAiFeedArchive\2026\feed-2026-09-10.xml`

At the initial filesystem inspection around 10:19 local time, this file was absent. Source inspection confirmed the creating-editor fallback was present. CMS/Web caching assembly timestamps were newer than the source, while the Hangfire copy was older; this did not establish which writer had produced the user's previously viewed output. No speculative attribution change was made.

The current archive checker and its dependencies were built to the isolated `creator-validation` artifacts directory. The documented `--write-archive 2440937` check then invoked the real service against the guarded development database, rebuilding today's complete rolling-window file rather than exporting only that article. It ran two archive refreshes and one combined refresh with a deliberately simulated Elasticsearch failure.

Verified output:

| File | Eligible articles | Nonblank `dc:creator` elements | Size | `lastBuildDate` |
|---|---:|---:|---:|---|
| `feed-2026-09-10.xml` | 3 | 3 | 4,545 bytes | `Thu, 10 Sep 2026 10:22:28 +0300` |

- Creator values matched the actual assigned-author or `News.CreatedBy` editor names in the database-backed checks.
- Independent parsing of the generated file confirmed every item had a nonblank creator in the Dublin Core namespace using the `dc` prefix.
- Repeated refreshes reused the same filename; historical-file checksums and the triggering article's stored settings remained unchanged.
- The simulated Elasticsearch failure was reported as expected and did not remove the generated archive. No real Elasticsearch request or database write was performed.
- The complete solution build subsequently passed. All 10 creator cases, 13 HTML/CDATA cases, the description check, and both 154-item rolling-window scenarios also passed again.

The existing generator successfully produced the required creators; this follow-up regenerated the actual local XML and updated this record without changing generator code or inventing creator names. Reload the file in any editor/viewer that still shows an older buffer. This validates the local archive, not the live Elasticsearch-backed feed. Any already-running host must load the rebuilt assemblies to use them for future refreshes.

## 12. XmlFeedCachingService duplication refactor

The requested optimization was a behavior-preserving cleanup, not performance profiling or a database optimization. `XmlFeedCachingService.cs` was reduced from 1,432 to 1,071 lines by extracting repeated implementation blocks:

| Shared helper | Consolidated responsibilities |
|---|---|
| `GetOrRefreshXmlAsync` | Cache lookup, refresh on missing/empty content, and second lookup for eight identical getters |
| `CacheXmlAsync` | XML serialization, cache document metadata and Elasticsearch updates for nine refresh paths |
| `CreateRssDocument` | Shared main/category/Google AI channel structure, logo, namespaces and Atom self-link |
| `CreateRssItem` | Shared item title/link, enclosure, media metadata, GUID and publication date, followed by feed-specific elements |
| `GetSitemapSchedule` | Shared change-frequency/priority thresholds for source and monthly post/tag sitemaps |

RSS namespace definitions are shared as well. Public method signatures, SQL queries, item filters and refresh orchestration were not changed. Main/category RSS retain their existing links, CDATA descriptions and 100-item limit. Google AI retains its uncapped inclusive 48-hour window, creating-editor fallback, cleaned content, high-resolution photo handling, archive-first write, lock and atomic replacement behavior. The helpers preserve caller-supplied dates and existing local/UTC choices rather than normalizing them.

`GetCategoryRSS` was deliberately left unchanged: its existing post-refresh cache read is discarded, and a missing category can still be dereferenced. Fixing those pre-existing behaviors is separate from this refactor. The sequential and parallel sitemap orchestration methods were also left distinct.

Added `tools/GoogleAiArchiveCheck/XmlFeedRefactorChecks.cs` and registered an offline command in the existing runner:

```powershell
dotnet run --project .\tools\GoogleAiArchiveCheck\GoogleAiArchiveCheck.csproj -- --check-refactor
```

Validation completed:

- Complete solution build passed.
- The standalone check project and its dependencies built successfully in `tools/GoogleAiArchiveCheck/bin/refactor-validation`, with zero errors and 20 existing warnings.
- Cache checks passed for eight public getter key/index mappings, eight read-through cases, two exception-propagation cases and two cache-write results, including XML serialization/preamble validation.
- XML fixtures passed for all three channel variants and standard/Google AI items, including element order, metadata, links, CDATA and creator placement.
- All 13 sitemap scheduling cases passed, including exact thresholds, future dates and nullable input.
- Existing checks passed again: 10 creator cases, 13 HTML/CDATA cases plus description cleanup, and both 154-item rolling-window scenarios.

This refactor validation did not access the database or Elasticsearch, and did not read or regenerate archive XML. The database-backed archive result in section 11 predates this refactor; it was not rerun as part of the cleanup. No new packages, migrations or runtime performance improvements are claimed.
