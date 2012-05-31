namespace Simple.Web.Behaviors
{
    using System.Collections.Generic;

    [RequestBehavior(typeof(Implementations.SetFiles))]
    public interface IUploadFiles
    {
        IEnumerable<IPostedFile> Files { set; }
    }
}