namespace Simple.Web.Behaviors.Implementations {
	using Behaviors;
	using Http;

	public static class WriteResponseCookies {
		/// <summary>
		/// This method supports the framework directly and should not be used from your code
		/// </summary>
		/// <param name="handler">The handler.</param>
		/// <param name="context">The context.</param>
		/// <returns></returns>
		public static void Impl (ISetCookies handler, IContext context) {
			if (context.Response.StatusCode == 200
				|| context.Response.StatusCode == 100
				|| (context.Response.StatusCode >= 301 && context.Response.StatusCode <= 303)
				|| context.Response.StatusCode == 307) {
				foreach (var cookie in handler.ResponseCookies.Values)
					context.Response.SetCookie(cookie.Name, cookie.Value); // TODO: finish this
			}
		}
	}
}