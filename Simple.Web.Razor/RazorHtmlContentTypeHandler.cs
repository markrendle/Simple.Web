namespace Simple.Web.Razor
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Dynamic;
    using System.IO;
    using System.Reflection;

    [ContentTypes(ContentType.Html, ContentType.XHtml)]
    public class RazorHtmlContentTypeHandler : IContentTypeHandler
    {
        public object Read(StreamReader streamReader, Type inputType)
        {
            throw new NotImplementedException();
        }

        public void Write(IOutputEndpoint endpoint, TextWriter textWriter)
        {
            var specifyView = endpoint as ISpecifyView;
            Type viewType;
            if (specifyView != null)
            {
                viewType = new RazorViews().GetViewType(specifyView.ViewPath);
            }
            else
            {
                viewType = new RazorViews().GetViewTypeForModelType(endpoint.GetType());
            }

            RenderView(endpoint, textWriter, viewType);
        }

        internal static void RenderView(IOutputEndpoint endpoint, TextWriter textWriter, Type viewType)
        {
            var instance = (SimpleTemplateBase) Activator.CreateInstance(viewType);
            instance.SetModel(endpoint.Output);

            var viewData = new ExpandoObject() as IDictionary<string, object>;

            foreach (var property in endpoint.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                try
                {
                    viewData[property.Name] = property.GetValue(endpoint, null);
                }
                catch (TargetInvocationException)
                {
                    Trace.TraceError("Property {0} not available for View in {1}", property.Name, endpoint.GetType().Name);
                }
            }

            instance.Var = viewData;
            instance.Writer = textWriter;
            instance.Execute();
        }
    }
}