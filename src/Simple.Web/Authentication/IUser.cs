namespace Simple.Web.Authentication
{
    using System;

    /// <summary>
    /// Represents an authenticated user.
    /// </summary>
    public interface IUser
    {
        /// <summary>
        /// Gets the GUID that uniquely identifies the user in the system.
        /// </summary>
        Guid Guid { get; }

        /// <summary>
        /// Gets a value indicating whether this user is authenticated.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this user is authenticated; otherwise, <c>false</c>.
        /// </value>
        bool IsAuthenticated { get; }

        /// <summary>
        /// Gets the user's name.
        /// </summary>
        string Name { get; }
    }
}