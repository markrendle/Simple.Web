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

        public string Domain { get; set; }

        public bool HttpOnly { get; set; }

        public string Name
        {
            get { return _name; }
        }

        public string Path { get; set; }

        public bool Secure { get; set; }

        public TimeSpan TimeOut { get; set; }

        public string Value
        {
            get { return _value; }
        }

        public string ToHeaderString()
        {
            Path = Path ?? "/";
            var builder = new StringBuilder();
            builder.AppendFormat("{0}={1}", Name, HttpUtility.UrlEncode(Value, Encoding.UTF8));
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