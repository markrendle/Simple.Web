namespace Simple.Web.Razor
{
    using System;

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
            return _value == null ? string.Empty : _value.ToString();
        }

        public string ToString(IFormatProvider formatProvider)
        {
            return Convert.ToString(_value, formatProvider);
        }
    }
}