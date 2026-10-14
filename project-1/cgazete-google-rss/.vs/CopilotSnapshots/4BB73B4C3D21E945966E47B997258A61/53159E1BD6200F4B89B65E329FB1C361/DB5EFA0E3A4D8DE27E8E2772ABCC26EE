using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Quark.Core.Extentions
{
    public static class StringExtensions
    {
        public static string ToSlug(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return string.Empty;

            str = str.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (var ch in str)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(ch);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(ch);
                }
            }

            str = sb.ToString().ToLowerInvariant();

            str = str
                .Replace("ü", "u")
                .Replace("ğ", "g")
                .Replace("ç", "c")
                .Replace("ş", "s")
                .Replace("ö", "o")
                .Replace("ı", "i");

            str = Regex.Replace(str, @"[^a-z0-9]+", "-");

            str = Regex.Replace(str, @"-+", "-");

            str = str.Trim('-');

            return str;
        }

        public static string ToSlugTR(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return string.Empty;

            str = str.ToLower(new CultureInfo("tr-TR"));

            str = str
                .Replace("ü", "u")
                .Replace("ğ", "g")
                .Replace("ç", "c")
                .Replace("ş", "s")
                .Replace("ö", "o")
                .Replace("ı", "i");

            str = Regex.Replace(str, @"[^a-z0-9]+", "-");
            str = Regex.Replace(str, @"-+", "-");

            return str.Trim('-');
        }
        public static string ConvertToIso8601(DateTime dateTime)
        {
            var offset = TimeSpan.FromHours(3); // UTC+3
            var dateTimeOffset = new DateTimeOffset(dateTime, offset);
            return dateTimeOffset.ToString("yyyy-MM-ddTHH:mm:sszzz");
        }

        public static string PrepareWebsiteUrl(this string slug, string baseUrl)
        {
            if (string.IsNullOrWhiteSpace(slug))
                return string.Empty;

            if (string.IsNullOrWhiteSpace(baseUrl))
                return string.Empty;

            return $"{baseUrl}{slug.TrimStart('/')}";
        }

        public static string ExtractTextFromHtml(this string html)
        {
            if (string.IsNullOrEmpty(html))
            {
                return "";
            }

            string plainText = Regex.Replace(html, "<[^>]+?>", " ");
            plainText = WebUtility.HtmlDecode(plainText);
            plainText = Regex.Replace(plainText, @"\s+", " ").Trim();

            return plainText;
        }

        public static string ExtractTextFromHtml(this string html, int? maxLength = null)
        {
            if (string.IsNullOrEmpty(html))
            {
                return "";
            }

            // HTML taglerini temizle
            string plainText = Regex.Replace(html, "<[^>]+?>", " ");

            // HTML entity'leri decode et
            plainText = WebUtility.HtmlDecode(plainText);

            // Fazladan boşlukları temizle
            plainText = Regex.Replace(plainText, @"\s+", " ").Trim();

            if (maxLength.HasValue && maxLength.Value > 0 && plainText.Length > maxLength.Value)
            {
                plainText = plainText.Substring(0, maxLength.Value);
            }

            return plainText;
        }

        public static string ToSplitDot(this string time)
		{
			if (string.IsNullOrEmpty(time))
				throw new ArgumentException("Geçersiz format", nameof(time));

			return int.Parse(time.Split('.')[0]).ToString();
		}
        public static string FirstCharToUpper(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            return char.ToUpper(input[0]) + input.Substring(1);
        }
        public static int CharacterCount(this string input)
        {

           if (string.IsNullOrWhiteSpace(input))
                return 0;

            return input.Length;
        }
    }
}
