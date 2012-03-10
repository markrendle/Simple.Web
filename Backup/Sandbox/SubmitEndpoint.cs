namespace Sandbox
{
    using Simple.Web;

    public class SubmitEndpoint : PostEndpoint<Form,string>
    {
        public override string UriTemplate
        {
            get { return "/submit"; }
        }

        protected override string Get()
        {
            return string.Format("Posted value: {0}!", Model.Text);
        }
    }
}