namespace Sandbox
{
    using Simple.Web;

    [UriTemplate("/submit")]
    public class PostForm : IPost, IInput<Form>, IOutput<Form>
    {
        public Status Post()
        {
            Output = Input;
            return Status.OK;
        }

        public Form Input { get; set; }

        public Form Output { get; set; }
    }
}