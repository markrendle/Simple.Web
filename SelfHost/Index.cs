using Simple.Web;

namespace SelfHost
{
	[UriTemplate("/")]
    public class Index : IGet
    {
        public Status Get()
        {
            return 200;
        }
    }
}