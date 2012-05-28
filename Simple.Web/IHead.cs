namespace Simple.Web
{
    using System.Threading.Tasks;

    [HttpVerb("HEAD")]
    public interface IHead
    {
        Status Head();
    }

    [HttpVerb("HEAD")]
    public interface IHeadAsync
    {
        Task<Status> Head();
    }
}