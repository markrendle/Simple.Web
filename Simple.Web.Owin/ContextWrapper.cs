using System.Collections.Generic;
using Owin;
using Simple.Web.Http;

namespace Simple.Web.Owin
{
	class ContextWrapper :  IContext
	{
		readonly IDictionary<string, object> env;
		readonly ResponseWrapper reponseWrapper;

		public ContextWrapper(IDictionary<string, object> env, ResultDelegate result)
		{
			this.env = env;
			reponseWrapper = new ResponseWrapper(result);
		}

		public IRequest Request
		{
			get { return new RequestWrapper(env);}
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