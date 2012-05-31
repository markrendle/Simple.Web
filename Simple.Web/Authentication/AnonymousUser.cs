namespace Simple.Web.Authentication
{
    using System;

    class AnonymousUser : IUser
    {
        public static readonly IUser Instance = new AnonymousUser();
        private AnonymousUser()
        {
            
        }

        public Guid Guid
        {
            get { return Guid.Empty; }
        }

        public string Name
        {
            get { return "Anonymous"; }
        }

        public bool IsAuthenticated
        {
            get { return false; }
        }
    }
}