namespace Sandbox
{
    using System.Collections.Generic;
    using System.Linq;

    using Simple.Web;
    using Simple.Web.Behaviors;

    [UriTemplate("/enumerable")]
    public class GetEnumerable : IGet, IOutput<EnumerableModel>
    {
        public IEnumerable<string> Message { get; set; }

        public EnumerableModel Output { get; set; }

        public Status Get()
        {
            Output = new EnumerableModel { Messages = Message.ToArray() };
            return 200;
        }
    }

    public class EnumerableModel
    {
        public string[] Messages { get; set; }
    }
}