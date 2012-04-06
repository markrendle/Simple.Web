namespace Simple.Web.Razor
{
    using System;
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

        public virtual void SetModel(object model)
        {
            
        }

        internal protected dynamic Var
        {
            get { return _var; }
            set { _var = value; }
        }
    }

    public class AttributePart
    {
        private readonly object _value;
        private readonly int _position;

        private AttributePart(object value, int position)
        {
            _value = value;
            _position = position;
        }

        public static implicit operator AttributePart(Tuple<string,int> source)
        {
            return new AttributePart(source.Item1, source.Item2);
        }

        public static implicit operator AttributePart(Tuple<object,int> source)
        {
            return new AttributePart(source.Item1, source.Item2);
        }

        public override string ToString()
        {
            return _value.ToString();
        }

        public string ToString(IFormatProvider formatProvider)
        {
            return Convert.ToString(_value, formatProvider);
        }
    }

    public class AttributeValue
    {
        private readonly AttributePart _prefix;
        private readonly AttributePart _value;
        private readonly bool _literal;

        private AttributeValue(AttributePart prefix, AttributePart value, bool literal)
        {
            _prefix = prefix;
            _value = value;
            _literal = literal;
        }

        public static implicit operator AttributeValue(Tuple<Tuple<string, int>, Tuple<object, int>, bool> value)
        {
            return new AttributeValue(value.Item1, value.Item2, value.Item3);
        }

        public static implicit operator AttributeValue(Tuple<Tuple<string, int>, Tuple<string, int>, bool> value)
        {
            return new AttributeValue(value.Item1, value.Item2, value.Item3);
        }

        public override string ToString()
        {
            return _prefix.ToString() + _value;
        }

        public string ToString(IFormatProvider formatProvider)
        {
            return _prefix.ToString(formatProvider) + _value.ToString(formatProvider);
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