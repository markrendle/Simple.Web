namespace Simple.Web.MediaTypeHandling
{
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    using Simple.Web.Helpers;
    using Simple.Web.Links;

    public abstract class MediaTypeHandlerBase<TWireFormat> : IMediaTypeHandler
        where TWireFormat : class
    {
        public Task<T> Read<T>(Stream inputStream)
        {
            return ReadInput(inputStream).ContinueWith(t => FromWireFormat<T>(t.Result)).Unwrap();
        }

        public Task Write<T>(IContent content, Stream outputStream)
        {
            if (content.Model != null)
            {
                TWireFormat output = null;

                // handle enumerable
                var enumerable = content.Model as IEnumerable;
                if (enumerable != null && typeof(T) != typeof(string))
                {
                    bool skipLinksLookup = false;
                    var formatted = new List<TWireFormat>();
                    foreach (object item in enumerable)
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
                        TWireFormat wireFormat = ToWireFormat(item);
                        if (wireFormat != null)
                        {
                            AddWireFormattedLinks(wireFormat, itemLinks);
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
                    output = ToWireFormat((T)content.Model);
                    if (output != null)
                    {
                        AddWireFormattedLinks(output, content.Links);
                    }
                }
                if (output != null)
                {
                    return WriteOutput(output, outputStream);
                }
            }
            return TaskHelper.Completed();
        }

        protected abstract void AddWireFormattedLinks(TWireFormat wireFormattedItem, IEnumerable<Link> itemLinks);

        protected abstract Task<T> FromWireFormat<T>(TWireFormat wireFormat);

        protected abstract Task<TWireFormat> ReadInput(Stream inputStream);

        protected abstract TWireFormat ToWireFormat(object item);

        protected abstract TWireFormat ToWireFormat<T>(T item);

        protected abstract TWireFormat WrapCollection(IList<TWireFormat> collection, IEnumerable<Link> collectionLinks);

        protected abstract Task WriteOutput(TWireFormat output, Stream outputStream);
    }
}