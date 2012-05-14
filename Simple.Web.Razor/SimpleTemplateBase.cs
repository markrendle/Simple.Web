namespace Simple.Web.Razor
{
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
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
            Trace.Write(value);
        }

        public virtual void WriteLiteral(object value)
        {
            Writer.Write(value);
            Trace.Write(value);
        }

        //public virtual void WriteAttribute(object value)
        //{
        //    Writer.Write(value);
        //    Trace.Write(value);
        //}

        public virtual void WriteAttribute(string attributeName, AttributePart prefix, AttributePart suffix, params AttributeValue[] values)
        {
            WriteLiteral(prefix);
            for (int i = 0; i < values.Length; i++)
            {
                Write(values[i]);
            }
            WriteLiteral(suffix);
        }

        public virtual void SetHandler(object handler)
        {
            
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

    public abstract class SimpleTemplateModelBase<TModel> : SimpleTemplateBase
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
    
    public abstract class SimpleTemplateHandlerBase<THandler> : SimpleTemplateBase
    {
        private THandler _handler;

        public THandler Handler
        {
            get { return _handler; }
            set { _handler = value; }
        }

        public override void SetHandler(object handler)
        {
            _handler = (THandler)handler;
        }
    }
    
    public abstract class SimpleTemplateHandlerModelBase<THandler, TModel> : SimpleTemplateBase
    {
        private THandler _handler;

        public THandler Handler
        {
            get { return _handler; }
            set { _handler = value; }
        }

        public override void SetHandler(object handler)
        {
            _handler = (THandler)handler;
        }

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