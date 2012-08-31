using Simple.Web;
using Simple.Web.Behaviors;

namespace Sandbox
{
    [UriTemplate("/model")]
    public class GetModel : IGet, IOutput<Model>
    {
        public Status Get()
        {
            return 200;
        }

        public Model Output { get { return new Model(); } }
    }

    public class Model
    {
    }
}