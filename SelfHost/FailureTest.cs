using System;
using Simple.Web;
using Simple.Web.Behaviors;

namespace SelfHost
{
    [UriTemplate("/fail")]
	public class FailureTest: IGet, IOutput<RawHtml>
	{
    	public Status Get()
    	{
    		return 200;
    	}

    	public RawHtml Output
    	{
    		get { throw new Exception("Aaa! it all went wrong!"); }
    	}
	}
}
