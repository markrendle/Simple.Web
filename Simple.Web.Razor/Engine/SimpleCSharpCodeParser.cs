namespace Simple.Web.Razor.Engine
{
    using System.Web.Razor.Generator;
    using System.Web.Razor.Parser;

    internal class SimpleCSharpCodeParser : CSharpCodeParser
    {
        public SimpleCSharpCodeParser()
        {
            MapDirectives(ModelDirective, "model");
            MapDirectives(HandlerDirective, "handler");
        }

        private void ModelDirective()
        {
            AssertDirective("model");
            AcceptAndMoveNext();

            BaseTypeDirective("Model keyword must be followed by type name", CreateBaseTypeCodeGeneratorFromModel);
        }

        private static SpanCodeGenerator CreateBaseTypeCodeGeneratorFromModel(string modelTypeName)
        {
            return new SetBaseTypeCodeGenerator(string.Format("Simple.Web.Razor.SimpleTemplateModelBase<{0}>", modelTypeName));
        }

        private void HandlerDirective()
        {
            AssertDirective("handler");
            AcceptAndMoveNext();

            BaseTypeDirective("Handler keyword must be followed by type name", CreateBaseTypeCodeGeneratorFromHandler);
        }

        private static SpanCodeGenerator CreateBaseTypeCodeGeneratorFromHandler(string handlerTypeName)
        {
            return new SetBaseTypeCodeGenerator(string.Format("Simple.Web.Razor.SimpleTemplateHandlerBase<{0}>", handlerTypeName));
        }
    }
}