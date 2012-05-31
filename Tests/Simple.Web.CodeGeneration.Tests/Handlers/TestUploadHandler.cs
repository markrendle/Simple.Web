namespace Simple.Web.CodeGeneration.Tests.Handlers
{
    using System.Collections.Generic;
    using Behaviors;

    class TestUploadHandler : IPost, IUploadFiles
    {
        public Status Post()
        {
            return 200;
        }

        public IEnumerable<IPostedFile> Files
        {
            set { }
        }
    }
}