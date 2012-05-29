namespace Simple.Web
{
    using System;
    using System.Security.Principal;

    public class User : IUser
    {
        private readonly Guid _guid;
        private readonly string _name;
        private readonly bool _isAuthenticated;

        public User(Guid guid, string name)
        {
            _guid = guid;
            _name = name;
            _isAuthenticated = true;
        }

        public User(IPrincipal principal)
        {
            _name = principal.Identity.Name;
            _isAuthenticated = principal.Identity.IsAuthenticated;
        }

        public bool IsAuthenticated
        {
            get { return _isAuthenticated; }
        }

        public Guid Guid
        {
            get { return _guid; }
        }

        public string Name
        {
            get { return _name; }
        }
    }
}