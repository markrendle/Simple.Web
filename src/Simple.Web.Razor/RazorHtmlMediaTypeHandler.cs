namespace Simple.Web.Razor
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using Helpers;
    using MediaTypeHandling;

    [MediaTypes(MediaType.Html, MediaType.XHtml)]
    public class RazorHtmlMediaTypeHandler : IMediaTypeHandler
    {
        private static readonly Func<SimpleTemplateBase, bool> HasLayout = stb => !String.IsNullOrWhiteSpace(stb.Layout);

        private static readonly RazorViews _razorViews = new RazorViews();
        private static readonly DynamicDictionary<string> _viewBag = new DynamicDictionary<string>();

        public object Read(Stream inputStream, Type inputType)
        {
            throw new NotImplementedException();
        }

        public Task Write(IContent content, Stream outputStream)
        {
            var handlerType = content.Handler != null ? content.Handler.GetType() : null;
            var modelType = content.Model != null ? content.Model.GetType() : null;
            var viewType = _razorViews.GetViewType(handlerType, modelType);

            if (viewType == null)
            {
                throw new ViewNotFoundException();
            }

            byte[] buffer;
            using (var writer = new StringWriter())
            {
                RenderView(content, writer, viewType);
                buffer = Encoding.UTF8.GetBytes(writer.ToString());
            }

            return outputStream.WriteAsync(buffer, 0, buffer.Length);
        }

        internal static void RenderView(IContent content, TextWriter textWriter, Type viewType)
        {
            var view = InflateType(viewType, content.Handler, content.Model);

            var hasLayout = HasLayout(view);
            var output = view.Output;
            var sections = view.Sections;

            while (hasLayout)
            {
                var parentType = _razorViews.GetViewType(view.Layout);
                var parentView = InflateType(parentType, null, null, output, sections);

                output = parentView.Output;
                sections = parentView.Sections;

                if (!HasLayout(parentView))
                {
                    break;
                }
            }

            textWriter.Write(output);
        }

        private static SimpleTemplateBase InflateType(Type viewType, object handler, object model, string childOutput = null, IDictionary<string, string> sections = null)
        {
            var instance = (SimpleTemplateBase)Activator.CreateInstance(viewType);

            instance.SetChildOutput(childOutput);
            instance.SetSections(sections);
            instance.SetViewBag(_viewBag);
            instance.SetHandler(handler);
            instance.SetModel(model);
            instance.Render();

            return instance;
        }
    }
}