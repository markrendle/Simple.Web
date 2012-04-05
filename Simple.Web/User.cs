namespace Simple.Web
{
    using System.Security.Principal;

    public class User : IUser
    {
        private readonly string _name;
        private readonly bool _isAuthenticated;

        public User(IPrincipal principal)
        {
            _name = principal.Identity.Name;
            _isAuthenticated = principal.Identity.IsAuthenticated;
        }

        public bool IsAuthenticated
        {
            get { return _isAuthenticated; }
        }

        public string Name
        {
            get { return _name; }
        }
    }
}