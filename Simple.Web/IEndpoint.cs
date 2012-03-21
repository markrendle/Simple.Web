using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.Web
{
    public interface IEndpoint
    {
        Status Run();
    }

    public interface IInputEndpoint : IEndpoint
    {
        object Input { set; }
        Type InputType { get; }
    }

    public interface IOutputEndpoint : IEndpoint
    {
        object Output { get; }
        Type OutputType { get; }
    }

    public interface IInputEndpoint<in TInput> : IInputEndpoint
    {
        new TInput Input { set; }
    }

    public interface IOutputEndpoint<out TOutput> : IOutputEndpoint
    {
        new TOutput Output { get; }
    }

    public struct Status : IEquatable<Status>
    {
        public static readonly Status OK = new Status(200, "OK");

        private readonly int _httpStatusCode;
        private readonly string _httpStatusDescription;

        public Status(int httpStatusCode) : this(httpStatusCode, null)
        {
        }

        public Status(int httpStatusCode, string httpStatusDescription) : this()
        {
            _httpStatusCode = httpStatusCode;
            _httpStatusDescription = httpStatusDescription;
        }

        public static implicit operator Status(int httpStatus)
        {
            return new Status(httpStatus);
        }

        public int Code
        {
            get { return _httpStatusCode; }
        }

        public string Description
        {
            get { return _httpStatusDescription; }
        }

        public bool Equals(Status other)
        {
            return _httpStatusCode == other._httpStatusCode;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof (Status)) return false;
            return Equals((Status) obj);
        }

        public override int GetHashCode()
        {
            return _httpStatusCode;
        }

        public static bool operator ==(Status left, Status right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Status left, Status right)
        {
            return !left.Equals(right);
        }
    }
}
