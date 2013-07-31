using System;
using System.IO;
using System.Threading.Tasks;
using Simple.Web.DependencyInjection;
using Simple.Web.MediaTypeHandling;
using Simple.Web.Serialization;

namespace Simple.Web.Xml
{
	[MediaTypes(MediaType.Xml, "application/*+xml")]
	internal class ExplicitXmlMediaTypeHandler : IMediaTypeHandler
	{
		public Task<T> Read<T>(Stream inputStream)
		{
			//ISimpleContainerScope container = SimpleWeb.Configuration.Container.BeginScope();
			//container.Get<IConvertXmlFor<>>()
			throw new NotImplementedException();
		}

		public Task Write<T>(IContent content, Stream outputStream)
		{
			throw new NotImplementedException();
		}
	}
}