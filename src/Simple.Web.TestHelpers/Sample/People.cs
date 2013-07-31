namespace Simple.Web.TestHelpers.Sample
{
    using System.Collections.Generic;
    using Links;

    public class Person
    {
        public string Name { get; set; }
        public string Location { get; set; }
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