namespace Simple.Web
{
    public interface IWebEnvironment
    {
        string AppRoot { get; }
        IPathUtility PathUtility { get; }
        IFileUtility FileUtility { get; }
    }
}