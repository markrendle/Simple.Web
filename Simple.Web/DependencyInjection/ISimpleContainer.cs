namespace Simple.Web.DependencyInjection
{
    public interface ISimpleContainer
    {
        T Get<T>();
    }
}