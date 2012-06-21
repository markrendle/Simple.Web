using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using Owin;
using Simple.Web;
using Simple.Web.Http;

namespace SelfHost
{
	public class RequestWrapper : IRequest
	{
		readonly IDictionary<string, object> env;
		readonly IDictionary<string, IEnumerable<string>> headers;
		//readonly BodyDelegate body;

		public RequestWrapper(IDictionary<string, object> environment)
		{
			env = environment;
			headers = (IDictionary<string, IEnumerable<string>>)environment[OwinConstants.RequestHeaders];
			//body = (BodyDelegate)environment[OwinConstants.RequestBody];
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
			get {
				// TODO: determine encoding properly?
				return null;
				//return new MemoryStream(UTF8Encoding.Default.GetBytes(env[OwinConstants.RequestBody].ToString()));
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