namespace Simple.Web
{
    using System.Threading.Tasks;

    [HttpVerb("PATCH")]
    public interface IPatch
    {
        Status Patch();
    }

    [HttpVerb("PATCH")]
    public interface IPatchAsync
    {
        Task<Status> Patch();
    }
}