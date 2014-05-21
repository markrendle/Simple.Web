namespace Simple.Web.MediaTypeHandling
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Settings for media type handling
    /// </summary>
    public static class MediaTypeHandlers
    {
        internal static readonly Dictionary<string, Type> PreferredTypes = new Dictionary<string, Type>();

        /// <summary>
        /// Explicitly set media type handlers
        /// </summary>
        /// <param name="mediaTypes">One or more media types to set the handler for</param>
        /// <returns>A fluent utility class for specifying the type</returns>
        /// <exception cref="ArgumentException"></exception>
        public static FluentFor For(params string[] mediaTypes)
        {
            if (mediaTypes.Length == 0) throw new ArgumentException("No mediaTypes specified", "mediaTypes");
            return new FluentFor(mediaTypes);
        }

        /// <summary>
        /// Clears the internal list
        /// </summary>
        public static void Clear()
        {
            PreferredTypes.Clear();
        }
 
        /// <summary>
        /// Fluent utility class
        /// </summary>
        public class FluentFor
        {
            private readonly string[] _mediaTypes;

            internal FluentFor(string[] mediaTypes)
            {
                _mediaTypes = mediaTypes;
            }

            /// <summary>
            /// Specify the <see cref="IMediaTypeHandler"/> to use for this media type
            /// </summary>
            /// <typeparam name="T">An implementation of <see cref="IMediaTypeHandler"/></typeparam>
            public void Use<T>() where T : IMediaTypeHandler
            {
                foreach (var mediaType in _mediaTypes)
                {
                    PreferredTypes[mediaType] = typeof (T);
                }
            }
        }
    }
}