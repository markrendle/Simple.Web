namespace Simple.Web
{
    using System;

    internal interface IEndpointRunner
    {
        object Endpoint { get; }
        bool HasInput { get; }
        bool HasOutput { get; }
        object Input { set; }
        object Output { get; }
        Type InputType { get; }

        void BeforeRun(IContext context, ContentTypeHandlerTable contentTypeHandlerTable);
    }
}