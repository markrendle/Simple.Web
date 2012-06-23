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
			throw new NotImplementedException();
		}

		public void SetCookie(string name, IDictionary<string, string> values, DateTime? expires = new DateTime?(), bool httpOnly = false, bool secure = false, string domain = null, string path = null)
		{
			throw new NotImplementedException();
		}

		public void RemoveCookie(string name)
		{
			throw new NotImplementedException();
		}

		public void TransmitFile(string file)
		{
			buffer = new MemoryStream(File.ReadAllBytes(file));
		}

		public void DisableCache()
		{
			throw new NotImplementedException();
		}

		public void SetCacheAbsoluteExpiry(DateTime expiresAt)
		{
			throw new NotImplementedException();
		}

		public void SetCacheSlidingExpiry(TimeSpan expiresIn)
		{
			throw new NotImplementedException();
		}

		public void SetETag(string etag)
		{
			throw new NotImplementedException();
		}

		public void SetLastModified(DateTime lastModified)
		{
			throw new NotImplementedException();
		}

		public void SetCacheVaryByContentEncodings(ICollection<string> varyByContentEncodings)
		{
			throw new NotImplementedException();
		}

		public void SetCacheVaryByParams(ICollection<string> varyByParams)
		{
			throw new NotImplementedException();
		}

		public void SetCacheVaryByHeaders(ICollection<string> varyByHeaders)
		{
			throw new NotImplementedException();
		}
	}
}