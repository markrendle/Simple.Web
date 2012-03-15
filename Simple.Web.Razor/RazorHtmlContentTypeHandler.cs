namespace Simple.Web.Razor
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.IO;
    using System.Reflection;

    public class RazorHtmlContentTypeHandler : IContentTypeHandler
    {
        public object Read(StreamReader streamReader, Type inputType)
        {
            throw new NotImplementedException();
        }

        public void Write(IEndpoint endpoint, TextWriter textWriter)
        {
            if (endpoint.Output == null) return;

            var viewType = new RazorViews().GetViewTypeForModelType(endpoint.GetType());

            RenderView(endpoint, textWriter, viewType);
        }

        internal static void RenderView(IEndpoint endpoint, TextWriter textWriter, Type viewType)
        {
            var instance = (SimpleTemplateBase) Activator.CreateInstance(viewType);
            instance.SetModel(endpoint.Output);

            var viewData = new ExpandoObject() as IDictionary<string, object>;

            foreach (var property in endpoint.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                viewData[property.Name] = property.GetValue(endpoint, null);
            }

            instance.Var = viewData;
            instance.Writer = textWriter;
            instance.Execute();
        }
    }
}