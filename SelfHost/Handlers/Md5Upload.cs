using System.Collections.Generic;
using System.Linq;
using Simple.Web;
using Simple.Web.Behaviors;

namespace SelfHost
{
	[UriTemplate("/md5")]
    public class GetMd5Upload : IGet
    {
        public Status Get()
        {
            return 200;
        }
    }

	[UriTemplate("/md5")]
    public class Md5UploadPost : IPost, IUploadFiles, IMayRedirect
    {
		public Status Post()
		{
			if (Files.Any())
			{
				Location = "/raw/" + Files.Single().FileName;
			} else
			{
				Location = "/md5";
			}
			return Status.SeeOther;
		}

		public IEnumerable<IPostedFile> Files { get; set; }

		public string Location { get; set; }
    }
}