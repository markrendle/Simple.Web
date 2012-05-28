namespace Simple.Web.CodeGeneration
{
    using System.Reflection;

    public interface IMethodLookup
    {
        MethodInfo CheckAuthentication { get; }
        MethodInfo SetFiles { get; }
        MethodInfo SetRequestCookies { get; }
        MethodInfo SetResponseCookies { get; }
        MethodInfo SetInput { get; }
        MethodInfo SetInputETag { get; }
        MethodInfo SetOutputETag { get; }
        MethodInfo SetLastModified { get; }
        MethodInfo SetIfModifiedSince { get; }
        MethodInfo WriteStatusCode { get; }
        MethodInfo SetCache { get; }
        MethodInfo Redirect { get; }
        MethodInfo WriteStreamResponse { get; }
        MethodInfo WriteRawHtml { get; }
        MethodInfo WriteOutput { get; }
        MethodInfo WriteView { get; }
    }
}