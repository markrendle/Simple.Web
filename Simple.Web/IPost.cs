namespace Simple.Web
{
    using System.Threading.Tasks;

    public interface IPost
    {
        Status Post();
    }

    public interface IPostAsync
    {
        Task<Status> Post();
    }
}