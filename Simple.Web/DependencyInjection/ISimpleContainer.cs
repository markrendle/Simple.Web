namespace Simple.Web.DependencyInjection
{
    /// <summary>
    /// Interface to implement for Dependency Injection.
    /// </summary>
    /// <remarks>Wrap this around your favorite IoC container.</remarks>
    public interface ISimpleContainer
    {
        /// <summary>
        /// Gets an instance of <c>T</c>.
        /// </summary>
        /// <typeparam name="T">The type of thing to construct.</typeparam>
        /// <returns>A new instance of T.</returns>
        T Get<T>();
    }
}