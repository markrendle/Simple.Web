namespace Sandbox
{
    using Sandbox.Models;

    using Simple.Web;
    using Simple.Web.Behaviors;

    [UriTemplate("/submit")]
    public class PostForm : IPost, IInput<Form>, IOutput<Form>
    {
        public Form Input { get; set; }

        public string InputETag
        {
            set { }
        }

        public Form Output { get; set; }

        public string OutputETag
        {
            get { return null; }
        }

        public Status Post()
        {
            Output = Input;
            return Status.OK;
        }
    }
}