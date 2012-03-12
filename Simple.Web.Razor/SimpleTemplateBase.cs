namespace Simple.Web.Razor
{
    using System.IO;

    public abstract class SimpleTemplateBase
    {
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