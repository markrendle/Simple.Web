namespace Simple.Web.Behaviors
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents a handler which accepts uploaded files.
    /// </summary>
    [RequestBehavior(typeof(Implementations.SetFiles))]
    public interface IUploadFiles
    {
        /// <summary>
        /// Used by the framework to supply a list of uploaded files.
        /// </summary>
        /// <value>
        /// The files.
        /// </value>
        IEnumerable<IPostedFile> Files { set; }
    }
}