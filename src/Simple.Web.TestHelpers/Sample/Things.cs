namespace Simple.Web.TestHelpers.Sample
{
    using Simple.Web.Links;

    public class Thing
    {
        public string Path { get; set; }
    }

    [LinksFrom(typeof(Thing), "/things?path={Path}", Rel = "self")]
    public class ThingHandler
    {
    }
}