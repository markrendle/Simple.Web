using System;
using System.Collections.Generic;
using System.IO;
using Simple.Web.Http;

namespace Simple.Web.Owin
{
	class ResponseWrapper : IResponse
	{
		MemoryStream buffer;
		readonly HeaderDictionary headers;

		public ResponseWrapper()
		{
			buffer = new MemoryStream();
			headers = new HeaderDictionary();
		}

		public int StatusCode {get; set; }

		public string StatusDescription { get; set; }

		public Stream OutputStream
		{
			get {
				return Buffer;
			}
		}

		public string ContentType { get; set; }

		public MemoryStream Buffer
		{
			get { return buffer; }
		}

		public HeaderDictionary Headers
		{
			get { return headers; }
		}

		public void SetHeader(string headerName, string value)
		{
			headers.Include(headerName, value);
		}

		public void SetCookie(string name, string value, DateTime? expires = new DateTime?(), bool httpOnly = false, bool secure = false, string domain = null, string path = null)
		{
			Headers.Include("Set-Cookie", name + "=" + value); // todo: finish this!
		}

		public void SetCookie(string name, IDictionary<string, string> values, DateTime? expires = new DateTime?(), bool httpOnly = false, bool secure = false, string domain = null, string path = null)
		{
			foreach (var key in values.Keys) {
				SetCookie(key, values[key], expires, httpOnly, secure, domain, path);
			}
		}

		public void RemoveCookie(string name)
		{
		}

		public void TransmitFile(string file)
		{
			buffer = new MemoryStream(File.ReadAllBytes(file));
		}

		public void DisableCache()
		{
		}

		public void SetCacheAbsoluteExpiry(DateTime expiresAt)
		{
		}

		public void SetCacheSlidingExpiry(TimeSpan expiresIn)
		{
		}

		public void SetETag(string etag)
		{
		}

		public void SetLastModified(DateTime lastModified)
		{
		}

		public void SetCacheVaryByContentEncodings(ICollection<string> varyByContentEncodings)
		{
		}

		public void SetCacheVaryByParams(ICollection<string> varyByParams)
		{
		}

		public void SetCacheVaryByHeaders(ICollection<string> varyByHeaders)
		{
		}
	}
}