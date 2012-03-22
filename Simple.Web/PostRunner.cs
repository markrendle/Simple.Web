namespace Simple.Web
{
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
    }
}