namespace Simple.Web
{
    using System.Collections.Generic;
    using DependencyInjection;

    public interface IConfiguration
    {
        ISet<string> PublicFolders { get; }
        IDictionary<string, string> PublicFileMappings { get; }
        ISimpleContainer Container { get; set; }
    }
}