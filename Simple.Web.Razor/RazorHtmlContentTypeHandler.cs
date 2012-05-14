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
            if (content.ViewPath != null)
            {
                viewType = new RazorViews().GetViewType(content.ViewPath);
            }
            else if (content.Model != null)
            {
                viewType = new RazorViews().GetViewTypeForModelType(content.Model.GetType());
            }
            else
            {
                throw new ViewNotFoundException();
            }

            RenderView(content, textWriter, viewType);
        }

        internal static void RenderView(IContent handler, TextWriter textWriter, Type viewType)
        {
            var instance = (SimpleTemplateBase) Activator.CreateInstance(viewType);
            instance.SetModel(handler.Model);

            var viewData = new ExpandoObject() as IDictionary<string, object>;

            foreach (var pair in handler.Variables)
            {
                viewData.Add(pair);
            }

            instance.Var = viewData;
            instance.Writer = textWriter;
            instance.Execute();
        }
    }
}