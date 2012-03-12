namespace Sandbox
{
    using Simple.Web;

    [UriTemplate("/submit")]
    public class SubmitEndpoint : PostEndpoint<Form,RawHtml>
    {
        protected override Status Get()
        {
            Output = Raw.Html(string.Format("Posted value: {0}!", Input.Text));
            return Status.OK;
        }
    }
}