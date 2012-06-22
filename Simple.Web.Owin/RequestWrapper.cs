using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Owin;
using Simple.Web.Http;

namespace Simple.Web.Owin
{
	public class RequestWrapper : IRequest
	{
		readonly IDictionary<string, object> env;
		readonly IDictionary<string, IEnumerable<string>> headers;
		readonly BodyDelegate bodyDelegate;
		readonly MemoryStream bodyStream = new MemoryStream();

		public RequestWrapper(IDictionary<string, object> environment)
		{
			env = environment;
			headers = (IDictionary<string, IEnumerable<string>>)environment[OwinConstants.RequestHeaders];
			bodyDelegate = (BodyDelegate)environment[OwinConstants.RequestBody];

			bodyDelegate(reader, flush => false, ex=> { }, CancellationToken.None);
		}

		bool reader(ArraySegment<byte> data)
		{
			bodyStream.Seek(0, SeekOrigin.Begin);
			bodyStream.Write(data.Array, 0, data.Array.Length);
			return false;
		}

		public Uri Url
		{
			get {
				var x = env[OwinConstants.RequestScheme] + "://"
					+ headers["Host"].Single()
					+ env[OwinConstants.RequestPath];

				return new Uri(x,  UriKind.Absolute);
			}
		}

		public IList<string> AcceptTypes
		{
			get {
				return headers["Accept"].Single().Split(',');
			}
		}

		public IDictionary<string, string> QueryString
		{
			get
			{
				return env[OwinConstants.RequestQueryString].ToString().ToQueryDictionary();
			}
		}


		public Stream InputStream
		{
			get
			{
				// TODO: skip past head
				bodyStream.Seek(0, SeekOrigin.Begin);
				return bodyStream;
			}
		}

		public string ContentType
		{
			get {
				return GetSingleHeaderValue("Content-Type") ?? "text/html"; // POST, HEAD; Get == ""?
			}
		}

		string GetSingleHeaderValue(string headerKey)
		{
			if (!headers.ContainsKey(headerKey)) return null;
			return headers[headerKey].Single();
		}

		public string HttpMethod
		{
			get { return env[OwinConstants.RequestMethod].ToString(); }
		}

		public NameValueCollection Headers
		{
			get {
				return headers.ToNameValueCollection();
			}
		}

		public IEnumerable<IPostedFile> Files
		{
			get { 
				return new List<IPostedFile>(); //TODO
			}
		}

		public IDictionary<string, ICookie> Cookies
		{
			get {
				return new Dictionary<string, ICookie>(); //TODO
			}
		}
	}
}