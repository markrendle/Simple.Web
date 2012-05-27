namespace Simple.Web
{
    using System;

    public class CacheOptions
    {
        public static readonly CacheOptions DisableCaching = new CacheOptions();

        private readonly TimeSpan? _slidingExpiry;

        public TimeSpan? SlidingExpiry
        {
            get { return _slidingExpiry; }
        }

        public DateTime? AbsoluteExpiry
        {
            get { return _absoluteExpiry; }
        }

        private readonly DateTime? _absoluteExpiry;

        private CacheOptions()
        {
            
        }

        public bool Disable
        {
            get { return !(_slidingExpiry.HasValue || _absoluteExpiry.HasValue); }
        }

        public CacheOptions(DateTime absoluteExpiry)
        {
            _absoluteExpiry = absoluteExpiry;
        }

        public CacheOptions(TimeSpan slidingExpiry)
        {
            _slidingExpiry = slidingExpiry;
        }
    }
}