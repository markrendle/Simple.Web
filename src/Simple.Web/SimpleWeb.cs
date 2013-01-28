namespace Simple.Web
{
    /// <summary>
    /// Configuration and Environment information.
    /// </summary>
    public static class SimpleWeb
    {
        /// <summary>
        /// The current Configuration for the app.
        /// </summary>
        public static readonly IConfiguration Configuration = new Configuration();

        /// <summary>
        /// Environmental information for the app.
        /// </summary>
        public static readonly IWebEnvironment Environment = new WebEnvironment();
    }
}