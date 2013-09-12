﻿using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;
using Inflector;
using Simple.Web.Helpers;
using Simple.Web.Links;
using Simple.Web.MediaTypeHandling;

namespace Simple.Web.Xml
{
    public abstract class XElementMediaTypeHandlerBase : MediaTypeHandlerBase<XElement>
    {
        protected override void AddWireFormattedLinks(XElement wireFormattedItem, IEnumerable<Link> itemLinks)
        {
            if (itemLinks == null)
            {
                return;
            }
            foreach (Link link in itemLinks)
            {
                wireFormattedItem.Add(link.ToXml());
            }
        }

        protected override Task<XElement> ReadInput(Stream inputStream)
        {
            return TaskHelper.Completed(XElement.Load(inputStream));
        }

        protected override Task WriteOutput(XElement output, Stream outputStream)
        {
            output.Save(outputStream);
            return TaskHelper.Completed();
        }

        protected override XElement WrapCollection(IList<XElement> collection, IEnumerable<Link> collectionLinks)
        {
            //todo should we wrap a single item?
            var xml = new XElement(collection[0].Name.LocalName.Pluralize());
            foreach (XElement element in collection)
            {
                xml.Add(element);
            }
            //todo add collection links?
            return xml;
        }
    }
}