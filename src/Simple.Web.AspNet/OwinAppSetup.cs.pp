namespace $rootnamespace$
{
    using Fix;
    using Simple.Web;

    public class OwinAppSetup
    {
        public static void Setup(Fixer fixer)
        {
            fixer.Use(Application.Run);
        }
    }
}