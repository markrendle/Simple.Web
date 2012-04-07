namespace Simple.Web
{
    using System.IO;

    public interface IOutput<out TOutput>
    {
        TOutput Output { get; }
    }

    public interface IOutputStream : IOutput<Stream>
    {
        string ContentType { get; }
    }
}