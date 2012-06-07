namespace Simple.Web.Razor.Engine
{
    using System.Globalization;
    using System.Web.Razor.Generator;

    public class SetModelTypeCodeGenerator : SetBaseTypeCodeGenerator
    {
        public SetModelTypeCodeGenerator(string baseType) : base(baseType)
        {
        }

        protected override string ResolveType(CodeGeneratorContext context, string baseType)
        {
            return string.Format(CultureInfo.InvariantCulture, "Simple.Web.Razor.SimpleTemplateModelBase<{0}>", baseType);
        }
    }
    
    public class SetHandlerTypeCodeGenerator : SetBaseTypeCodeGenerator
    {
        public SetHandlerTypeCodeGenerator(string baseType) : base(baseType)
        {
        }

        protected override string ResolveType(CodeGeneratorContext context, string baseType)
        {
            return string.Format(CultureInfo.InvariantCulture, "Simple.Web.Razor.SimpleTemplateHandlerBase<{0}>", baseType);
        }
    }
}