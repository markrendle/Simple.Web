namespace Simple.Web.Razor
{
    using System;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using Helpers;
    using MediaTypeHandling;

    [MediaTypes(MediaType.Html, MediaType.XHtml)]
    public class RazorHtmlMediaTypeHandler : IMediaTypeHandler
    {
        public object Read(Stream inputStream, Type inputType)
        {
            throw new NotImplementedException();
        }

        public Task Write(IContent content, Stream outputStream)
        {
            var razorViews = new RazorViews();
            var handlerType = content.Handler != null ? content.Handler.GetType() : null;
            var modelType = content.Model != null ? content.Model.GetType() : null;
            var viewType = razorViews.GetViewType(handlerType, modelType);

            if (viewType == null)
            {
                throw new ViewNotFoundException();
            }

            byte[] buffer;
            using (var writer = new StringWriter())
            {
                RenderView(content, writer, viewType);
                buffer = Encoding.Default.GetBytes(writer.ToString());
            }

            return outputStream.WriteAsync(buffer, 0, buffer.Length);
        }

        internal static void RenderView(IContent content, TextWriter textWriter, Type viewType)
        {
            var instance = (SimpleTemplateBase) Activator.CreateInstance(viewType);
            instance.SetHandler(content.Handler);
            instance.SetModel(content.Model);

            instance.Writer = textWriter;
            instance.Execute();
        }
    }
}