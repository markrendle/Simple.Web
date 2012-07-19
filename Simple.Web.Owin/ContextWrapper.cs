#if OWIN
using System.Collections.Generic;
using Simple.Web.Http;

namespace Simple.Web.Owin
{
	class ContextWrapper :  IContext
	{
		readonly IDictionary<string, object> env;
		readonly ResponseWrapper reponseWrapper;
		readonly RequestWrapper requestWrapper;

		public ContextWrapper(IDictionary<string, object> env)
		{
			this.env = env;
			reponseWrapper = new ResponseWrapper();
			requestWrapper = new RequestWrapper(env);
		}

		public IRequest Request
		{
			get { return requestWrapper; }
		}

		public IResponse Response
		{
			get {
				return reponseWrapper;
			}
		}

		public IDictionary<string, object> Variables
		{
			get {
				return env;
			} //???
		}
	}
}
#endif