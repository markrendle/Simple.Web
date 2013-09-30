namespace Simple.Web
{
    using System;
    using System.Security.Principal;

    using Simple.Web.Authentication;

    /// <summary>
    /// Convenience implementation of the <see cref="IUser"/> interface. Feel free to make your own.
    /// </summary>
    public class User : IUser
    {
        private readonly Guid _guid;
        private readonly bool _isAuthenticated;
        private readonly string _name;

        /// <summary>
        /// Initializes a new instance of the <see cref="User"/> class.
        /// </summary>
        /// <param name="guid">The GUID.</param>
        /// <param name="name">The name.</param>
        public User(Guid guid, string name)
        {
            _guid = guid;
            _name = name;
            _isAuthenticated = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="User"/> class.
        /// </summary>
        /// <param name="principal">The principal.</param>
        public User(IPrincipal principal)
        {
            _name = principal.Identity.Name;
            _isAuthenticated = principal.Identity.IsAuthenticated;
        }

        /// <summary>
        /// Gets the GUID that uniquely identifies the user in the system.
        /// </summary>
        public Guid Guid
        {
            get { return _guid; }
        }

        /// <summary>
        /// Gets a value indicating whether this user is authenticated.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this user is authenticated; otherwise, <c>false</c>.
        /// </value>
        public bool IsAuthenticated
        {
            get { return _isAuthenticated; }
        }

        /// <summary>
        /// Gets the user's name.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }
    }
}