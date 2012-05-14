namespace Simple.Web.Razor
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Dynamic;
    using System.IO;
    using System.Reflection;
    using ContentTypeHandling;

    [ContentTypes(ContentType.Html, ContentType.XHtml)]
    public class RazorHtmlContentTypeHandler : IContentTypeHandler
    {
        public object Read(StreamReader streamReader, Type inputType)
        {
            throw new NotImplementedException();
        }

        public void Write(IContent content, TextWriter textWriter)
        {
            Type viewType;
            var razorViews = new RazorViews();
            if (content.ViewPath != null)
            {
                viewType = razorViews.GetViewType(content.ViewPath);
            }
            else
            {
                var handlerType = content.Handler != null ? content.Handler.GetType() : null;
                var modelType = content.Model != null ? content.Model.GetType() : null;
                viewType = razorViews.GetViewTypeForHandlerAndModelType(handlerType, modelType);
            }

            if (viewType == null)
            {
                throw new ViewNotFoundException();
            }

            RenderView(content, textWriter, viewType);
        }

        internal static void RenderView(IContent content, TextWriter textWriter, Type viewType)
        {
            var instance = (SimpleTemplateBase) Activator.CreateInstance(viewType);
            instance.SetHandler(content.Handler);
            instance.SetModel(content.Model);

            var viewData = new ExpandoObject() as IDictionary<string, object>;

            foreach (var pair in content.Variables)
            {
                viewData.Add(pair);
            }

            instance.Var = viewData;
            instance.Writer = textWriter;
            instance.Execute();
        }
    }
}