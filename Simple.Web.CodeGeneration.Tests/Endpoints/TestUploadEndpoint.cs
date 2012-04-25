namespace Simple.Web.CodeGeneration.Tests.Endpoints
{
    using System.Collections.Generic;

    class TestUploadEndpoint : IPost, IUploadFiles
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