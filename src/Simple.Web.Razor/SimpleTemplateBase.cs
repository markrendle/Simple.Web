namespace Simple.Web.Razor
{
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;

    public abstract class SimpleTemplateBase
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        public TextWriter Writer { get; set; }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public abstract void Execute();

        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void Write(object value)
        {
            Writer.Write(value);
            Trace.Write(value);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void WriteLiteral(object value)
        {
            Writer.Write(value);
            Trace.Write(value);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void WriteAttribute(string attributeName, AttributePart prefix, AttributePart suffix, params AttributeValue[] values)
        {
            WriteLiteral(prefix);
            foreach (AttributeValue t in values)
            {
            	Write(t);
            }
        	WriteLiteral(suffix);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal virtual void SetHandler(object handler)
        {
            Handler = handler;
        }

        protected dynamic Handler { get; private set; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal virtual void SetModel(object model)
        {
            Model = model;
        }

        protected dynamic Model { get; private set; }
    }

	public abstract class SimpleTemplateModelBase<TModel> : SimpleTemplateBase
    {
        private TModel _model;

        public new TModel Model
        {
            get { return _model; }
            //set { _model = value; }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal override void SetModel(object model)
        {
            _model = (TModel)model;
        }
    }
    
    public abstract class SimpleTemplateHandlerBase<THandler> : SimpleTemplateBase
    {
        private THandler _handler;

        public new THandler Handler
        {
            get { return _handler; }
            //set { _handler = value; }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal override void SetHandler(object handler)
        {
            _handler = (THandler)handler;
        }
    }
    
    public abstract class SimpleTemplateHandlerModelBase<THandler, TModel> : SimpleTemplateBase
    {
        private THandler _handler;

        public new THandler Handler
        {
            get { return _handler; }
            //set { _handler = value; }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal override void SetHandler(object handler)
        {
            _handler = (THandler)handler;
        }

        private TModel _model;

        public new TModel Model
        {
            get { return _model; }
            //set { _model = value; }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal override void SetModel(object model)
        {
            _model = (TModel)model;
        }
    }
}