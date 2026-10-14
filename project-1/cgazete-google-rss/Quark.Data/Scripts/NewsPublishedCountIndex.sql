-- Approved target: News-Migration. ONLINE requires a supporting SQL Server edition.
-- Covers the published-news count's type join and related-news exclusion.
SET NOCOUNT ON;
SET XACT_ABORT ON;
SET LOCK_TIMEOUT 60000;

IF DB_NAME() <> N'News-Migration'
	THROW 50000, 'Run this script only against the approved News-Migration database.', 1;

IF OBJECT_ID(N'dbo.[News.NewsPublished]', N'U') IS NULL
	THROW 50001, 'The published-news table was not found.', 1;

IF EXISTS (
	SELECT 1
	FROM sys.indexes AS i
	WHERE i.object_id = OBJECT_ID(N'dbo.[News.NewsPublished]')
	  AND i.name = N'IX_NewsPublished_NewsTypeID_Include_NewsID'
)
BEGIN
	IF NOT EXISTS (
		SELECT 1
		FROM sys.indexes AS i
		WHERE i.object_id = OBJECT_ID(N'dbo.[News.NewsPublished]')
		  AND i.name = N'IX_NewsPublished_NewsTypeID_Include_NewsID'
		  AND i.type = 2
		  AND i.is_disabled = 0
		  AND i.is_hypothetical = 0
		  AND i.has_filter = 0
		  AND EXISTS (
			  SELECT 1
			  FROM sys.index_columns AS ic
			  JOIN sys.columns AS c ON c.object_id = ic.object_id AND c.column_id = ic.column_id
			  WHERE ic.object_id = i.object_id AND ic.index_id = i.index_id
				AND c.name = N'NewsTypeID' AND ic.key_ordinal = 1
		  )
		  AND EXISTS (
			  SELECT 1
			  FROM sys.index_columns AS ic
			  JOIN sys.columns AS c ON c.object_id = ic.object_id AND c.column_id = ic.column_id
			  WHERE ic.object_id = i.object_id AND ic.index_id = i.index_id
				AND c.name = N'NewsID' AND ic.is_included_column = 1
		  )
	)
		THROW 50002, 'An index with the candidate name exists but does not match the required covering definition.', 1;

	PRINT 'The covering index already exists; no changes made.';
END
ELSE
BEGIN
	CREATE NONCLUSTERED INDEX [IX_NewsPublished_NewsTypeID_Include_NewsID]
	ON [dbo].[News.NewsPublished] ([NewsTypeID])
	INCLUDE ([NewsID])
	WITH (
		ONLINE = ON (WAIT_AT_LOW_PRIORITY (MAX_DURATION = 1 MINUTES, ABORT_AFTER_WAIT = SELF)),
		MAXDOP = 2
	);

	PRINT 'Created IX_NewsPublished_NewsTypeID_Include_NewsID.';
END
