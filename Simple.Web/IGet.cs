namespace Simple.Web
{
    using System.Threading.Tasks;

    public interface IGet
    {
        Status Get();
    }

    public interface IGetAsync
    {
        Task<Status> Get();
    }
}