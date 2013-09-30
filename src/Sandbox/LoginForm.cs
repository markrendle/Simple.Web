namespace Sandbox
{
    using Simple.Web;
    using Simple.Web.Behaviors;

    [UriTemplate("/login")]
    public class LoginForm : IGet, ILoginPage
    {
        public string ReturnUrl { get; set; }

        public Status Get()
        {
            return Status.OK;
        }
    }
}