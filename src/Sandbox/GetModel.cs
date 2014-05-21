using System.Collections.Generic;
using Simple.Web;
using Simple.Web.Behaviors;
using Simple.Web.MediaTypeHandling;

namespace Sandbox
{
    using Simple.Web.Links;

    [UriTemplate("/model")]
    public class GetModel : IGet, IOutput<Model>
    {
        public Model Output {
            get { return new Model(); }
        }

        public Status Get() {
            return 200;
        }
    }

    [UriTemplate("/models")]
    [RespondsWith(MediaType.Json)]
    public class GetAllModels : IGet, IOutput<IEnumerable<Model>>
    {
        public IEnumerable<Model> Output {
            get { return new[] {new Model("one"), new Model("two"), new Model("three"),}; }
        }

        public Status Get() {
            return 200;
        }
    }

    [UriTemplate("/submodels/{Id}")]
    [Canonical(typeof(SubModel), "/submodels/{Id}")]
    public class GetSubModel : IGet, IOutput<SubModel>
    {
        public Status Get()
        {
            Output = new SubModel(Id);
            return 200;
        }

        public string Id { get; set; }

        public SubModel Output { get; private set; }
    }

    public class Model
    {
        public Model(string property = null) {
            Property = property;
            Subs = new List<SubModel> {new SubModel(property)};
        }

        public string Property { get; set; }

        public List<SubModel> Subs { get; set; }  
    }

    public class SubModel
    {
        public string Id { get; set; }

        public SubModel(string id)
        {
            Id = id;
        }
    }
}