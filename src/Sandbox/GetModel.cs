namespace Sandbox
{
    using System.Collections.Generic;

    using Simple.Web;
    using Simple.Web.Behaviors;
    using Simple.Web.MediaTypeHandling;

    [UriTemplate("/model")]
    public class GetModel : IGet, IOutput<Model>
    {
        public Model Output
        {
            get { return new Model(); }
        }

        public Status Get()
        {
            return 200;
        }
    }

    [UriTemplate("/models")]
    [RespondsWith(MediaType.Json)]
    public class GetAllModels : IGet, IOutput<IEnumerable<Model>>
    {
        public IEnumerable<Model> Output
        {
            get { return new[] { new Model("one"), new Model("two"), new Model("three") }; }
        }

        public Status Get()
        {
            return 200;
        }
    }

    public class Model
    {
        public Model(string property = null)
        {
            Property = property;
        }

        public string Property { get; set; }
    }
}