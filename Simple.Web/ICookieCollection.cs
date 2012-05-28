namespace Simple.Web
{
    using System.Collections.Generic;

    public interface ICookieCollection
    {
        /// <summary>
        /// Gets a string array containing all the keys (cookie names) in the cookie collection.
        /// </summary>
        /// <returns>
        /// An array of cookie names.
        /// </returns>
        IEnumerable<string> Keys { get; }

        /// <summary>
        /// Gets the number of cookies container id the collection.
        /// </summary>
        /// <returns>
        /// The number of cookies contained in the collection.
        /// </returns>
        int Count { get; }

        /// <summary>
        /// Gets the cookie with the specified numerical index from the cookie collection.
        /// </summary>
        /// <returns>
        /// The <see cref="T:Simple.Web.ICookie"/> specified by <paramref name="index"/>.
        /// </returns>
        /// <param name="index">The index of the cookie to retrieve from the collection. </param>
        ICookie this[int index] { get; }

        /// <summary>
        /// Gets the cookie with the specified name from the cookie collection.
        /// </summary>
        /// <returns>
        /// The <see cref="T:Simple.Web.ICookie"/> specified by <paramref name="name."/>
        /// </returns>
        /// <param name="name">Title of cookie to retrieve. </param>
        ICookie this[string name] { get; }

        /// <summary>
        /// Returns the key (name) of the cookie at the specified numerical index.
        /// </summary>
        /// <returns>
        /// The name of the cookie specified by <paramref name="index"/>.
        /// </returns>
        /// <param name="index">The index of the key to retrieve from the collection. </param>
        string GetKey(int index);

        /// <summary>
        /// Returns the <see cref="T:Simple.Web.ICookie"/> item with the specified index from the cookie collection.
        /// </summary>
        /// <returns>
        /// The <see cref="T:Simple.Web.ICookie"/> specified by <paramref name="index"/>.
        /// </returns>
        /// <param name="index">The index of the cookie to return from the collection. </param>
        ICookie Get(int index);

        /// <summary>
        /// Returns the cookie with the specified name from the cookie collection.
        /// </summary>
        /// <returns>
        /// The <see cref="T:Simple.Web.ICookie"/> specified by <paramref name="name"/>.
        /// </returns>
        /// <param name="name">The name of the cookie to retrieve from the collection. </param>
        ICookie Get(string name);

        /// <summary>
        /// Updates the value of an existing cookie in a cookie collection.
        /// </summary>
        /// <param name="cookie">The <see cref="T:Simple.Web.ICookie"/> object to update. </param>
        void Set(ICookie cookie);

        /// <summary>
        /// Removes the cookie with the specified name from the collection.
        /// </summary>
        /// <param name="name">The name of the cookie to remove from the collection. </param>
        void Remove(string name);

        /// <summary>
        /// Clears all cookies from the cookie collection.
        /// </summary>
        void Clear();

        /// <summary>
        /// Adds the specified cookie to the cookie collection.
        /// </summary>
        /// <param name="cookie">The <see cref="T:Simple.Web.ICookie"/> to add to the collection. </param>
        void Add(ICookie cookie);

        /// <summary>
        /// Creates a new instance of the correct cookie type for this collection.
        /// </summary>
        /// <returns>A new cookie.</returns>
        ICookie New(string name);
    }
}