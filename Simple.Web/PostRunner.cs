namespace Simple.Web
{
    using System.IO;
    using System.Threading.Tasks;

    sealed class PostRunner : EndpointRunner
    {
        private readonly IPost _post;

        public PostRunner(IPost post) : base(post)
        {
            _post = post;
        }

        public override Status Run()
        {
            return _post.Post();
        }

        public override void BeforeRun(IContext context, ContentTypeHandlerTable contentTypeHandlerTable)
        {
            var contentTypeHandler = contentTypeHandlerTable.GetContentTypeHandler(context.Request.ContentType);
            using (var reader = new StreamReader(context.Request.InputStream))
            {
                Input = contentTypeHandler.Read(reader, InputType);
            }
        }
    }

    sealed class PostAsyncRunner : AsyncEndpointRunner
    {
        private readonly IPostAsync _post;

        public PostAsyncRunner(IPostAsync post) : base(post)
        {
            _post = post;
        }

        public override Task<Status> Run()
        {
            return _post.Post();
        }

        public override void BeforeRun(IContext context, ContentTypeHandlerTable contentTypeHandlerTable)
        {
            var contentTypeHandler = contentTypeHandlerTable.GetContentTypeHandler(context.Request.ContentType);
            using (var reader = new StreamReader(context.Request.InputStream))
            {
                Input = contentTypeHandler.Read(reader, InputType);
            }
        }
    }
}