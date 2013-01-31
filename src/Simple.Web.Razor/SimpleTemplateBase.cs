namespace Simple.Web.Razor
{
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Text;

    public abstract class SimpleTemplateBase
    {
        private readonly StringBuilder _output = new StringBuilder();
        private string _childOutput;

        public SimpleTemplateBase()
        {
            this._output = new StringBuilder();
            this.ViewBag = new DynamicDictionary<string>();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public string Output
        {
            get { return _output.ToString(); }
        }

        public TextWriter Writer { get; set; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public abstract void Execute();

        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void Write(object value)
        {
            _output.Append(value);
            Trace.Write(value);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void WriteLiteral(object value)
        {
            _output.Append(value);
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

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal virtual void SetChildOutput(string childOutput)
        {
            _childOutput = childOutput;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal virtual void SetViewBag(DynamicDictionary<string> viewBag)
        {
            this.ViewBag = viewBag;
        }

        public virtual object RenderBody()
        {
            if (!string.IsNullOrWhiteSpace(_childOutput))
            {
                return _childOutput;
            }

            return null;
        }

        protected dynamic Model { get; private set; }

        public string Layout { get; set; }

        public dynamic ViewBag { get; private set; }
    }

    public abstract class SimpleTemplateModelBase<TModel> : SimpleTemplateBase
    {
        public new TModel Model { get; private set; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal override void SetModel(object model)
        {
            Model = (TModel)model;
        }
    }

    public abstract class SimpleTemplateHandlerBase<THandler> : SimpleTemplateBase
    {
        public new THandler Handler { get; private set; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal override void SetHandler(object handler)
        {
            Handler = (THandler)handler;
        }
    }

    public abstract class SimpleTemplateHandlerModelBase<THandler, TModel> : SimpleTemplateModelBase<TModel>
    {
        public new THandler Handler { get; private set; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal override void SetHandler(object handler)
        {
            Handler = (THandler)handler;
        }
    }
}