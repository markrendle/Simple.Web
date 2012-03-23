namespace Simple.Web
{
    using System.Threading.Tasks;

    public interface IGet
    {
        Status Get();
    }

    public interface IAsyncGet
    {
        Task<Status> Get();
    }
}