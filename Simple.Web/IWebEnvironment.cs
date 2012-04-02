namespace Simple.Web
{
    using System.Collections.Generic;

    public interface IWebEnvironment
    {
        string AppRoot { get; }
        IPathUtility PathUtility { get; }
        IFileUtility FileUtility { get; }
        string GetContentTypeFromFileExtension(string file, IList<string> acceptedTypes);
    }
}