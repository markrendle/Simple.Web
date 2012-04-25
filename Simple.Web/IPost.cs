namespace Simple.Web
{
    using System.Threading.Tasks;

    [HttpVerb("POST")]
    public interface IPost
    {
        Status Post();
    }

    [HttpVerb("POST")]
    public interface IPostAsync
    {
        Task<Status> Post();
    }
}