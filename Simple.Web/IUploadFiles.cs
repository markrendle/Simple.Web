namespace Simple.Web
{
    using System.Collections.Generic;

    public interface IUploadFiles
    {
        IEnumerable<IPostedFile> Files { set; }
    }
}