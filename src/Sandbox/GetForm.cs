namespace Sandbox
{
    using Simple.Web;
    using Simple.Web.Authentication;
    using Simple.Web.Behaviors;

    [UriTemplate("/form")]
    public class GetForm : IGet, IRequireAuthentication
    {
        public IUser CurrentUser { get; set; }

        public string Title
        {
            get { return "Test Form"; }
        }

        public Status Get()
        {
            return 200;
        }
    }
}