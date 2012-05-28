namespace Simple.Web
{
    using System.Threading.Tasks;

    [HttpVerb("DELETE")]
    public interface IDelete
    {
        Status Delete();
    }

    [HttpVerb("DELETE")]
    public interface IDeleteAsync
    {
        Task<Status> Delete();
    }
}