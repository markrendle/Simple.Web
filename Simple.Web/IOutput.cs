namespace Simple.Web
{
    public interface IOutput<out TOutput>
    {
        TOutput Output { get; }
    }
}