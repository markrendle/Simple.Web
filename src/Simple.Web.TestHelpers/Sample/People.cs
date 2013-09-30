namespace Simple.Web.TestHelpers.Sample
{
    using System.Collections.Generic;

    using Simple.Web.Links;

    public class Person
    {
        public string Location { get; set; }

        public string Name { get; set; }
    }

    [Canonical(typeof(Person))]
    [UriTemplate("/person/{Name}")]
    public class PersonHandler
    {
    }

    [Canonical(typeof(IEnumerable<Person>))]
    [UriTemplate("/people")]
    public class PeopleHandler
    {
    }
}