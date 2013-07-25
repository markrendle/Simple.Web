namespace Simple.Web.Razor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Text;

    public abstract class SimpleTemplateBase
    {
        private readonly StringBuilder _output = new StringBuilder();

        private IDictionary<string, Action> _sections = new Dictionary<string, Action>();
        private IDictionary<string, string> _sectionOutputs = new Dictionary<string, string>();

        private string _renderedOutput;
        private string _childOutput;

        public SimpleTemplateBase()
        {
            this._output = new StringBuilder();
            this.ViewBag = new DynamicDictionary<string>();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public string Output
        {
            get { return _renderedOutput ?? this._output.ToString(); }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public IDictionary<string, string> Sections
        {
            get { return this._sectionOutputs; }
        }

        public TextWriter Writer { get; set; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public abstract void Execute();

        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void Write(object value)
        {
            _output.Append(value);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void WriteLiteral(object value)
        {
            _output.Append(value);
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
        internal virtual void SetSections(IDictionary<string, string> sections)
        {
            if (sections != null)
            {
                this._sectionOutputs = sections;
            }
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

        public virtual void DefineSection(string name, Action action)
        {
            if (this._sections == null)
            {
                this._sections = new Dictionary<string, Action>(1);
            }

            this._sections.Add(name, action);
        }

        public virtual bool IsSectionDefined(string name)
        {
            return this._sections != null && this._sections.ContainsKey(name);
        }

        public virtual object RenderSection(string name)
        {
            return this.RenderSection(name, true);
        }

        public virtual object RenderSection(string name, bool required)
        {
            var sectionRendered = this._sectionOutputs.ContainsKey(name);
            var sectionDefined = this.IsSectionDefined(name);

            if (required && !(sectionRendered || sectionDefined))
            {
                throw new ArgumentException(
                    string.Format("Section '{0}' was not specified and is required.", name));
            }

            if (sectionRendered && this._sectionOutputs[name] != null)
            {
                this.WriteLiteral(this._sectionOutputs[name]);
            }

            if (sectionDefined && this._sections[name] != null)
            {
                this._sections[name].Invoke();
            }

            return String.Empty;
        }

        public void Render()
        {
            this.Execute();

            if (this._sections != null)
            {
                this._renderedOutput = this._output.ToString();

                this._output.Clear();

                foreach (var section in this._sections)
                {
                    this._output.Clear();
                    section.Value.Invoke();
                    this._sectionOutputs.Add(section.Key, this._output.ToString());
                }
            }
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