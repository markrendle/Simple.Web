namespace Simple.Web.Razor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Text;

    public abstract class SimpleTemplateBase
    {
        private readonly StringBuilder _output = new StringBuilder();

        private string _childOutput;
        private string _renderedOutput;
        private IDictionary<string, string> _sectionOutputs = new Dictionary<string, string>();
        private IDictionary<string, Action> _sections = new Dictionary<string, Action>();

        public SimpleTemplateBase()
        {
            _output = new StringBuilder();
            ViewBag = new DynamicDictionary<string>();
        }

        public string Layout { get; set; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public string Output
        {
            get { return _renderedOutput ?? _output.ToString(); }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public IDictionary<string, string> Sections
        {
            get { return _sectionOutputs; }
        }

        public dynamic ViewBag { get; private set; }

        public TextWriter Writer { get; set; }

        protected dynamic Handler { get; private set; }

        protected dynamic Model { get; private set; }

        public virtual void DefineSection(string name, Action action)
        {
            if (_sections == null)
            {
                _sections = new Dictionary<string, Action>(1);
            }

            _sections.Add(name, action);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public abstract void Execute();

        public virtual bool IsSectionDefined(string name)
        {
            return _sections != null && _sections.ContainsKey(name);
        }

        public void Render()
        {
            Execute();

            if (_sections != null)
            {
                _renderedOutput = _output.ToString();

                _output.Clear();

                foreach (var section in _sections)
                {
                    _output.Clear();
                    section.Value.Invoke();
                    _sectionOutputs.Add(section.Key, _output.ToString());
                }
            }
        }

        public virtual object RenderBody()
        {
            if (!string.IsNullOrWhiteSpace(_childOutput))
            {
                return _childOutput;
            }

            return null;
        }

        public virtual object RenderSection(string name)
        {
            return RenderSection(name, true);
        }

        public virtual object RenderSection(string name, bool required)
        {
            var sectionRendered = _sectionOutputs.ContainsKey(name);
            var sectionDefined = IsSectionDefined(name);

            if (required && !(sectionRendered || sectionDefined))
            {
                throw new ArgumentException(string.Format("Section '{0}' was not specified and is required.", name));
            }

            if (sectionRendered && _sectionOutputs[name] != null)
            {
                WriteLiteral(_sectionOutputs[name]);
            }

            if (sectionDefined && _sections[name] != null)
            {
                _sections[name].Invoke();
            }

            return String.Empty;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void Write(object value)
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
        public virtual void WriteLiteral(object value)
        {
            _output.Append(value);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal virtual void SetChildOutput(string childOutput)
        {
            _childOutput = childOutput;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal virtual void SetHandler(object handler)
        {
            Handler = handler;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal virtual void SetModel(object model)
        {
            Model = model;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal virtual void SetSections(IDictionary<string, string> sections)
        {
            if (sections != null)
            {
                _sectionOutputs = sections;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal virtual void SetViewBag(DynamicDictionary<string> viewBag)
        {
            ViewBag = viewBag;
        }
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