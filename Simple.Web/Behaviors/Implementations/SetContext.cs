namespace Simple.Web.Behaviors.Implementations
{
    using Http;

    public static class SetContext
    {
        public static void Impl(INeedContext handler, IContext context)
        {
            handler.Context = context;
        }
    }
}