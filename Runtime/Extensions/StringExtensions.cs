using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Hivefive.Utils
{
    public static class StringExtensions
    {
        private const string threeDots = "…";
        private static readonly Regex splitCamelCaseRegex = new("(\\B[A-Z0-9])", RegexOptions.Compiled);
        
        public static string AppendCharsToDesiredLength([NotNull] this string s, int length, char c)
        {
            return s.Length >= length ? s : s + new string(c, length - s.Length);
        }
        
        public static bool IsNullOrEmpty(this string text) { return string.IsNullOrEmpty(text); }

        public static bool IsNullOrWhiteSpace(this string text) { return string.IsNullOrWhiteSpace(text); }

        public static string SetRichTextStyle(this string text,
            Color? color = null,
            RichStyle richStyle = RichStyle.None)
        {
            if (text.IsNullOrEmpty()) {
                return text;
            }

            if (color.HasValue) {
                text = WrapWithTag(text, "color", $"#{ColorUtility.ToHtmlStringRGBA(color.Value)}");
            }

            if (richStyle != RichStyle.None) {
                text = richStyle.HasFlag(RichStyle.Italic) ? WrapWithTag(text, "i") : text;
                text = richStyle.HasFlag(RichStyle.Bold) ? WrapWithTag(text, "b") : text;
            }

            return text;
        }

        private static string WrapWithTag(string text, string tag, string value = null)
        {
            value = value.IsNullOrEmpty() ? "" : $"={value}";
            return tag.IsNullOrEmpty() ? text : $"<{tag}{value}>{text}</{tag}>";
        }
        
        // method to limit string to a certain length and add "..." at the end
        public static string LimitLength(this string text, int maxLength)
        {
            if (text.IsNullOrEmpty()) {
                return text;
            }

            return text.Length > maxLength ? text.Substring(0, maxLength) + threeDots : text;
        }
        
        public static string SearchAndLimitLength(this string text, string search, int maxLength)
        {
            int index;
            if (text.IsNullOrEmpty() || (index = text.IndexOf(search, StringComparison.Ordinal)) == -1) {
                return null;
            }

            var start = index - maxLength / 2;
            if (start < 0) {
                start = 0;
            }

            var end = start + maxLength;
            if (end > text.Length) {
                end = text.Length;
            }

            var result = text.Substring(start, end - start);
            if (start > 0) {
                result = threeDots + result;
            }

            if (end < text.Length) {
                result += threeDots;
            }

            return result;
        }

        public static string SplitCamelCase(this string text) { return splitCamelCaseRegex.Replace(text, " $1").Trim(); }
    }

    [Flags] public enum RichStyle { None = 0, Bold = 1 << 0, Italic = 1 << 1 }
}