namespace Simple.Web.Behaviors
{
    using System;

    [RequestBehavior(typeof(Implementations.SetIfModifiedSince))]
    [ResponseBehavior(typeof(Implementations.SetLastModified))]
    public interface IModified
    {
        DateTime? IfModifiedSince { set; }
        DateTime? LastModified { get; }
    }
}