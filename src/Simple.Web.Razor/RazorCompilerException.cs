namespace Simple.Web.Razor
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    public class RazorCompilerException : Exception
    {
        private readonly List<CompilerError> _errors;

        public RazorCompilerException(IEnumerable<CompilerError> errors)
        {
            _errors = errors.ToList();
        }

        public RazorCompilerException(string error)
        {
            _errors = new List<CompilerError>(1) { new CompilerError("uknown", 0, 0, "unknown", error) };
        }

        public ReadOnlyCollection<CompilerError> Errors
        {
            get { return _errors.AsReadOnly(); }
        }

        public override string Message
        {
            get { return string.Join(Environment.NewLine, _errors); }
        }
    }
}