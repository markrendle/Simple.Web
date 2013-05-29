namespace Simple.Web
{
    public partial struct Status
    {
// ReSharper disable MemberHidesStaticFromOuterClass
        public static class Success
        {
            public static readonly Status OK = new Status(200, "OK");
            public static readonly Status Created = new Status(201, "Created");
            public static readonly Status Accepted = new Status(202, "Accepted");
            public static readonly Status NonAuthoritativeInformation = new Status(203, "Non-Authoritative Information");
            public static readonly Status NoContent = new Status(204, "No Content");
            public static readonly Status ResetContent = new Status(205, "Reset Content");
            public static readonly Status PartialContent = new Status(206, "Partial Content");
        }
// ReSharper restore MemberHidesStaticFromOuterClass
    }
}