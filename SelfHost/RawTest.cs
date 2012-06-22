using Simple.Web.Behaviors;
using Simple.Web;

namespace SelfHost
{
	[UriTemplate("/raw/{Message}")]
    public class RawTest : IGet, IOutput<RawHtml>
    {
        public Status Get()
        {
            return 200;
        }

    	public string Message { get; set; }

    	public RawHtml Output
    	{
    		get {
				return Raw.Html(@"
<html><head><title></title></head><body>
<p>This message is coming to you from the codes:'"+(Message??"")+@"'</p>
<p>(Try <a href='/raw/hello,world'>this</a> if you don't see a message)</p></body></html>"); }
    	}
    }
}