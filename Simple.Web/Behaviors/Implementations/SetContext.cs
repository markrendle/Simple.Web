namespace Simple.Web.Behaviors.Implementations
{
    using Http;

    /// <summary>
    /// This type supports the framework directly and should not be used from your code.
    /// </summary>
    public static class SetContext
    {
        /// <summary>
        /// This method supports the framework directly and should not be used from your code
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public static void Impl(INeedContext handler, IContext context)
        {
            handler.Context = context;
        }
    }
}