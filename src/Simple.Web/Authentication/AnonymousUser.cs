namespace Simple.Web.Authentication
{
    using System;

    internal class AnonymousUser : IUser
    {
        public static readonly IUser Instance = new AnonymousUser();

        private AnonymousUser()
        {
        }

        public Guid Guid
        {
            get { return Guid.Empty; }
        }

        public bool IsAuthenticated
        {
            get { return false; }
        }

        public string Name
        {
            get { return "Anonymous"; }
        }
    }
}