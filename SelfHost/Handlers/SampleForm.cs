using Simple.Web;
using Simple.Web.Behaviors;

namespace SelfHost
{
    [UriTemplate("/form")]
	public class GetForm: IGet
	{
        public Status Get()
        {
            return Status.OK;
        }
	}

	[UriTemplate("/form")]
	public class PostForm : IPost, IInput<SampleFormContents>, IMayRedirect
	{
		public Status Post()
		{
			Location = "/raw/"+Input.UserName;
            return Status.SeeOther;
		}

		public SampleFormContents Input { get; set; }

		public string Location { get; set; }
	}

	public class SampleFormContents
	{
		public string UserName { get; set; }
		public string Password { get; set; }
	}
}
