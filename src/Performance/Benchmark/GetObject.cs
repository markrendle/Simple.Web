using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Simple.Web;
using Simple.Web.Behaviors;

namespace Benchmark
{
    [UriTemplate("/objects/{Id}")]
    public class GetObject : IGet, IOutput<Model>
    {
        private static readonly Model Model = new Model();
        public int Id { get; set; }
        public Status Get()
        {
            return Status.OK;
        }

        public Model Output {
            get { return Model; }
        }
    }

    public class Model
    {
        public int Id { get { return 1; } }
    }

    [UriTemplate("/")]
    public class Index : IGet, IOutput<RawHtml>
    {
        public Status Get()
        {
            return Status.OK;
        }

        public RawHtml Output {
            get { return "<h1>Benchmark</h1>"; }
        }
    }
}