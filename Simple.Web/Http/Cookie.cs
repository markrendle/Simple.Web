namespace Simple.Web.Http
{
    using System;
    using System.Text;
    using System.Web;

    internal class Cookie
    {
        private readonly string _name;
        private readonly string _value;

        public Cookie(string name, string value)
        {
            _name = name;
            _value = value;
        }

        public string Name
        {
            get { return _name; }
        }

        public string Value
        {
            get { return _value; }
        }

        public TimeSpan TimeOut { get; set; }
        public bool HttpOnly { get; set; }
        public bool Secure { get; set; }
        public string Path { get; set; }
        public string Domain { get; set; }

        public string ToHeaderString()
        {
            Path = Path ?? "/";
            var builder = new StringBuilder();
            builder.AppendFormat("{0}={1}", Name, HttpUtility.UrlEncode(Value, Encoding.Default));
            if (!string.IsNullOrWhiteSpace(Domain))
            {
                builder.AppendFormat("; Domain={0}", Domain);
            }
            builder.AppendFormat("; Path={0}", Path);
            if (TimeOut != TimeSpan.Zero)
            {
                builder.AppendFormat("; Expires={0}", (DateTime.UtcNow + TimeOut).ToString("R"));
            }

            if (Secure)
            {
                builder.Append("; Secure");
            }
            if (HttpOnly)
            {
                builder.Append("; HttpOnly");
            }

            return builder.ToString();
        }
    }
}