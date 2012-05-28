namespace Simple.Web
{
    using System.Threading.Tasks;

    [HttpVerb("PUT")]
    public interface IPut
    {
        Status Put();
    }

    [HttpVerb("PUT")]
    public interface IPutAsync
    {
        Task<Status> Put();
    }
}