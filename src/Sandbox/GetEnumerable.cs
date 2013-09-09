namespace Sandbox
{
    using System.Collections.Generic;
    using System.Linq;

    using Simple.Web;
    using Simple.Web.Behaviors;

    [UriTemplate("/enumerable")]
    public class GetEnumerable : IGet, IOutput<EnumerableModel>
    {
        public Status Get()
        {
            this.Output = new EnumerableModel { Messages = this.Message.ToArray() };
            return 200;
        }

        public IEnumerable<string> Message { get; set; }

        public EnumerableModel Output { get; set; }
    }

    public class EnumerableModel
    {
        public string[] Messages { get; set; }
    }
}
