namespace Simple.Web
{
    using System;

    public interface IModified
    {
        DateTime? IfModifiedSince { set; }
        DateTime? LastModified { get; }
    }
}