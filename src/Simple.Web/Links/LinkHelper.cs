namespace Simple.Web.Links
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using Helpers;

    /// <summary>
    /// Helper methods for working with RESTful links.
    /// </summary>
    public static class LinkHelper
    {
        private static readonly ConcurrentDictionary<Type, ILinkBuilder> LinkBuilders = new ConcurrentDictionary<Type, ILinkBuilder>();
        private static readonly object RootLinksSync = new object();
        private static List<Link> _rootLinks = null; 

        /// <summary>
        /// Gets the links for a model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>A readonly <see cref="ICollection&lt;Link&gt;"/> containing all available links for the model.</returns>
        public static ICollection<Link> GetLinksForModel(object model)
        {
            if (model == null) throw new ArgumentNullException("model");

            return LinkBuilders.GetOrAdd(model.GetType(), CreateBuilder).LinksForModel(model);
        }

        /// <summary>
        /// Gets the canonical link for a model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>A <see cref="Link"/> object representing the canonical URI for the model, if one is found.</returns>
        public static Link GetCanonicalLinkForModel(object model)
        {
            if (model == null) throw new ArgumentNullException("model");

            return LinkBuilders.GetOrAdd(model.GetType(), CreateBuilder).CanonicalForModel(model);
        }

        private static ILinkBuilder CreateBuilder(Type modelType)
        {
            var linkList = new List<Link>();
            foreach (var type in ExportedTypeHelper.FromCurrentAppDomain(LinkAttributeBase.Exists))
            {
                var attributesForModel = LinkAttributeBase.Get(type, modelType);
                if (attributesForModel.Count == 0) continue;
                linkList.AddRange(attributesForModel.Select(a => CreateLink(type, a)));
            }

            if (linkList.Count > 0)
            {
                return new LinkBuilder(linkList);
            }

            return LinkBuilder.Empty;
        }

        private static Link CreateLink(Type type, LinkAttributeBase linkAttribute)
        {
            string uriTemplate;
            if (string.IsNullOrWhiteSpace(linkAttribute.UriTemplate))
            {
                try
                {
                    uriTemplate = UriTemplateAttribute.GetAllTemplates(type).Single();
                }
                catch (InvalidOperationException)
                {
                    throw new InvalidOperationException("Must specify a UriTemplate for LinkAttribute where more than one UriTemplateAttribute is used.");
                }
            }
            else
            {
                uriTemplate = linkAttribute.UriTemplate;
            }

            return new Link(type, uriTemplate, linkAttribute.GetRel(), linkAttribute.Type, linkAttribute.Title);
        }

        public static IEnumerable<Link> GetRootLinks()
        {
            if (_rootLinks == null)
            {
                lock (RootLinksSync)
                {
                    if (_rootLinks == null)
                    {
                        var handlerTypes =
                            ExportedTypeHelper.FromCurrentAppDomain(t => Attribute.IsDefined(t, typeof (RootAttribute)));
                        _rootLinks = handlerTypes.Select(handlerType =>
                            CreateLink(handlerType, (RootAttribute)Attribute.GetCustomAttribute(handlerType, typeof(RootAttribute))))
                            .ToList();
                    }
                }
            }

            return _rootLinks.AsEnumerable();
        }
    }

}