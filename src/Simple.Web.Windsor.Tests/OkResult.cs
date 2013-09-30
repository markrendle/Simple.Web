namespace Simple.Web.Windsor.Tests
{
    public class OkResult : IResult
    {
        public Status Result
        {
            get { return Status.OK; }
        }
    }
}