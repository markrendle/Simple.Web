namespace Simple.Web
{
    public interface IGet
    {
        Status Get();
    }

    public interface IGet<out TOutput> : IGet
    {
        TOutput Output { get; }
    }
}