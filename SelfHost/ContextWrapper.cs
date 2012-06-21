using System.Collections.Generic;
using Owin;
using Simple.Web.Http;

namespace SelfHost
{
	class ContextWrapper :  IContext
	{
		readonly IDictionary<string, object> env;
		readonly ResultDelegate result;

		public ContextWrapper(IDictionary<string, object> env, ResultDelegate result)
		{
			this.env = env;
			this.result = result;
		}

		public IRequest Request
		{
			get { return new RequestWrapper(env);}
		}

		public IResponse Response
		{
			get { return new ResponseWrapper(result); }
		}

		public IDictionary<string, object> Variables
		{
			get {
				return env;
			} //???
		}
	}
}