namespace Simple.Web.CodeGeneration
{
    using System.Reflection;

    public interface IMethodLookup
    {
        MethodInfo CheckAuthentication { get; }
        MethodInfo SetFiles { get; }
        MethodInfo SetInput { get; }
        MethodInfo WriteStatusCode { get; }
        MethodInfo SetCookies { get; }
        MethodInfo DisableCache { get; }
        MethodInfo Redirect { get; }
        MethodInfo WriteStreamResponse { get; }
        MethodInfo WriteRawHtml { get; }
        MethodInfo WriteOutput { get; }
        MethodInfo WriteView { get; }
    }
}