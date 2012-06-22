using System;

namespace Simple.Web.Helpers
{
	public static class StringHelpers {
		/// <summary>
		/// Return the substring up to but not including the first instance of 'c'.
		/// If 'c' is not found, the entire string is returned.
		/// </summary>
		public static string SubstringBefore (this String src, char c) {
			if (String.IsNullOrEmpty(src)) return "";
 
			int idx = Math.Min(src.Length, src.IndexOf(c));
			if (idx < 0) return src;
			return src.Substring(0, idx);
		}
 
 
		/// <summary>
		/// Return the substring up to but not including the last instance of 'c'.
		/// If 'c' is not found, the entire string is returned.
		/// </summary>
		public static string SubstringBeforeLast (this String src, char c) {
			if (String.IsNullOrEmpty(src)) return "";
 
			int idx = Math.Min(src.Length, src.LastIndexOf(c));
			if (idx < 0) return src;
			return src.Substring(0, idx);
		}
 
		/// <summary>
		/// Return the substring after to but not including the first instance of 'c'.
		/// If 'c' is not found, the entire string is returned.
		/// </summary>
		public static string SubstringAfter (this String src, char c) {
			if (String.IsNullOrEmpty(src)) return "";
 
			int idx = Math.Min(src.Length - 1, src.IndexOf(c) + 1);
			if (idx < 0) return src;
			return src.Substring(idx);
		}
 
 
		/// <summary>
		/// Return the substring after to but not including the last instance of 'c'.
		/// If 'c' is not found, the entire string is returned.
		/// </summary>
		public static string SubstringAfterLast (this String src, char c) {
			if (String.IsNullOrEmpty(src)) return "";
 
			int idx = Math.Min(src.Length - 1, src.LastIndexOf(c) + 1);
			if (idx < 0) return src;
			return src.Substring(idx);
		}
	}
}