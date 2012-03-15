namespace Simple.Web.Razor
{
    using System.Dynamic;
    using System.IO;

    public abstract class SimpleTemplateBase
    {
        private dynamic _var = new ExpandoObject();

        public TextWriter Writer { get; set; }
        public abstract void Execute();

        public virtual void Write(object value)
        {
            Writer.Write(value);
        }

        public virtual void WriteLiteral(object value)
        {
            Writer.Write(value);
        }

        public virtual void SetModel(object model)
        {
            
        }

        internal protected dynamic Var
        {
            get { return _var; }
            set { _var = value; }
        }
    }

    public abstract class SimpleTemplateBase<TModel> : SimpleTemplateBase
    {
        private TModel _model;

        public TModel Model
        {
            get { return _model; }
            set { _model = value; }
        }

        public override void SetModel(object model)
        {
            _model = (TModel)model;
        }
    }
}