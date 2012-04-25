namespace Simple.Web
{
    using System.Threading.Tasks;

    [HttpVerb("GET")]
    public interface IGet
    {
        Status Get();
    }

    [HttpVerb("GET")]
    public interface IGetAsync
    {
        Task<Status> Get();
    }
}