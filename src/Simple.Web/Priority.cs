namespace Simple.Web
{
    /// <summary>
    /// Enumeration for things which need prioritising, such as Behaviours and URI resolution.
    /// </summary>
    public enum Priority
    {
        /// <summary>
        /// The highest priority. Things here happen first.
        /// </summary>
        Highest = -0x60000000,

        /// <summary>
        /// Higher than high, but not highest.
        /// </summary>
        Higher = -0x40000000,

        /// <summary>
        /// High.
        /// </summary>
        High = -0x20000000,

        /// <summary>
        /// Normal, the default level.
        /// </summary>
        Normal = 0,

        /// <summary>
        /// Low.
        /// </summary>
        Low = 0x20000000,

        /// <summary>
        /// Lower than low, but not lowest.
        /// </summary>
        Lower = 0x40000000,

        /// <summary>
        /// The lowest priority. Things here happen last.
        /// </summary>
        Lowest = 0x60000000
    }
}