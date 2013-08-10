using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Simple.Web.Helpers;
using Simple.Web.Links;

namespace Simple.Web.MediaTypeHandling
{
    public abstract class MediaTypeHandlerBase<TWireFormat> : IMediaTypeHandler
        where TWireFormat : class
    {
        public Task<T> Read<T>(Stream inputStream)
        {
            return Task<T>.Factory.StartNew(() =>
                {
                    var wireFormat = ReadInput(inputStream);
                    var result = FromWireFormat<T>(wireFormat);
                    return result;
                });
        }

        protected abstract TWireFormat ReadInput(Stream inputStream);

        protected abstract T FromWireFormat<T>(TWireFormat wireFormat);

        public Task Write<T>(IContent content, Stream outputStream)
        {
            if (content.Model != null)
            {
                TWireFormat output = null;

                // handle enumerable
                var enumerable = content.Model as IEnumerable<T>;
                if (enumerable != null)
                {
                    var skipLinksLookup = false;
                    var formatted = new List<TWireFormat>();
                    foreach (var item in enumerable)
                    {
                        ICollection<Link> itemLinks = null;
                        if (!skipLinksLookup)
                        {
                            itemLinks = LinkHelper.GetLinksForModel(item);
                            if (itemLinks.Count == 0)
                            {
                                skipLinksLookup = true;
                                itemLinks = null;
                            }
                        }
                        var wireFormat = ToWireFormat(item, itemLinks);
                        if (wireFormat != null)
                        {
                            formatted.Add(wireFormat);
                        }
                    }
                    if (formatted.Count > 0)
                    {
                        output = WrapCollection(formatted, content.Links);
                    }
                }
                else
                {
                    output = ToWireFormat((T) content.Model, content.Links);
                }
                if (output != null)
                {
                    return WriteOutput(output, outputStream);
                }
            }
            return TaskHelper.Completed();
        }

        protected abstract TWireFormat ToWireFormat<T>(T item, IEnumerable<Link> itemLinks);

        protected abstract TWireFormat WrapCollection(IList<TWireFormat> collection,
                                                      IEnumerable<Link> collectionLinks);

        protected abstract Task WriteOutput(TWireFormat output, Stream outputStream);
    }
}