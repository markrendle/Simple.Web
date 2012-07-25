namespace Simple.Web.Http
{
    using System;
    using System.Text;
    using System.Web;

    internal static class CookieWriter
    {
        private static readonly string DeleteExpiryDate = new DateTime(1970,1,1,0,0,0,DateTimeKind.Utc).ToString("R");
        public static string Write(string name, string value, int timeOut, bool httpOnly, bool secure, string path)
        {
            path = path ?? "/";
            var builder = new StringBuilder();
            builder.AppendFormat("{0}={1}; Path={2}", name, HttpUtility.UrlEncode(value, Encoding.Default), path);
            if (timeOut != 0)
            {
                builder.AppendFormat("; Expires={0}", (DateTime.UtcNow + TimeSpan.FromSeconds(timeOut)).ToString("R"));
            }

            if (secure)
            {
                builder.Append("; Secure");
            }
            if (httpOnly)
            {
                builder.Append("; HttpOnly");
            }

            return builder.ToString();
        }

        public static string WriteDelete(string name)
        {
            return string.Format("{0}=deleted; Expires={1}", name, DeleteExpiryDate);
        }
    }
}