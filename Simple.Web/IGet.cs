namespace Simple.Web
{
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a synchronous handler for a GET operation.
    /// </summary>
    [HttpVerb("GET")]
    public interface IGet
    {
        Status Get();
    }

    /// <summary>
    /// Represents an asynchronous handler for a GET operation.
    /// </summary>
    [HttpVerb("GET")]
    public interface IGetAsync
    {
        Task<Status> Get();
    }
}