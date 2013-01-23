namespace Simple.Web.Razor
{
    using System;

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
}