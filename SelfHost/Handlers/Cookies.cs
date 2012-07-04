using Simple.Web;
using Simple.Web.Behaviors;

namespace SelfHost
{
	[UriTemplate("/cookies")]
    public class GetCookies : IGet
    {
        public Status Get()
        {
            return Status.OK;
        }

		[Cookie]
		public string ASampleCookieValue { get; set; }
    }

	[UriTemplate("/addcookie")]
	public class AddCookie : IPost, IMayRedirect, IInput<CookieText> {
		public Status Post() {
			//A_Sample_Cookie_Value = Input.CookieValue;

			Location = "/cookies";
			return Status.SeeOther;
		}

		public string Location { get; set; }

		public CookieText Input { get; set; }

		[Cookie]
		public string ASampleCookieValue { get; set; }
	}

	public class CookieText {
		public string CookieValue { get; set; }
	}
}