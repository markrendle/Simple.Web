namespace Simple.Web.CodeGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    /// <summary>
    /// Builds methods to run endpoints for an <see cref="IContext"/>.
    /// </summary>
    internal class EndpointRunnerBuilder
    {
        private readonly Type _type;
        private readonly IMethodLookup _methodLookup;
        private readonly List<Expression> _blocks = new List<Expression>(); 
        private readonly LabelTarget _end = Expression.Label("end");
        private readonly ParameterExpression _endpointParameter;
        private ParameterExpression _context;
        private readonly ParameterExpression _endpoint;
        private ParameterExpression _status;
        private ParameterExpression _task;

        public EndpointRunnerBuilder(Type type, IMethodLookup methodLookup = null)
        {
            if (type == null) throw new ArgumentNullException("type");
            _type = type;
            _methodLookup = methodLookup ?? new MethodLookup();
            _endpointParameter = Expression.Parameter(typeof(object), "obj");
            _endpoint = Expression.Variable(_type, "endpoint");
        }

        public Action<object, IContext> BuildRunner()
        {
            _status = Expression.Variable(typeof(Status), "status");
            _context = Expression.Parameter(typeof(IContext), "context");

            _blocks.Add(Expression.Assign(_endpoint, Expression.Convert(_endpointParameter, _type)));
            CreateSetupBlocks();
            _blocks.Add(BuildRunBlock());
            CreateResponseBlocks();

            CreateDisposeBlock();

            _blocks.Add(Expression.Label(_end));

            var block = Expression.Block(new[] {_endpoint, _status}, _blocks);

            return Expression.Lambda<Action<object, IContext>>(block, _endpointParameter, _context).Compile();
        }

        public AsyncRunner BuildAsyncRunner()
        {
            _task = Expression.Variable(typeof (Task<Status>), "task");
            _context = Expression.Parameter(typeof(IContext), "context");
            _blocks.Add(Expression.Assign(_endpoint, Expression.Convert(_endpointParameter, _type)));
            CreateSetupBlocks();
            _blocks.Add(BuildAsyncRunBlock());
            _blocks.Add(Expression.Label(_end));
            _blocks.Add(_task);
            var block = Expression.Block(new[] {_endpoint, _task}, _blocks);

            var start =
                Expression.Lambda<Func<object, IContext, Task<Status>>>(block, _endpointParameter, _context).Compile();

            _blocks.Clear();
            _context = Expression.Parameter(typeof(IContext), "context");
            _blocks.Add(Expression.Assign(_endpoint, Expression.Convert(_endpointParameter, _type)));
            _status = Expression.Parameter(typeof(Status), "status");

            CreateResponseBlocks();
            block = Expression.Block(new[] {_endpoint}, _blocks);

            var end = Expression.Lambda<Action<object, IContext, Status>>(block, _endpointParameter, _context, _status).Compile();

            return new AsyncRunner(start, end);
        }

        private void CreateResponseBlocks()
        {
            CreateWriteStatusBlock();
            CreateSetCookiesBlock();
            CreateNoCacheBlock();
            CreateRedirectBlock();
            CreateOutputBlocks();
        }

        private void CreateSetupBlocks()
        {
            CreateAuthenticateBlock();
            CreateSetContextBlock();
            CreateSetFilesBlock();
            CreateSetInputBlock();
        }

        private void CreateAuthenticateBlock()
        {
            if (typeof (IRequireAuthentication).IsAssignableFrom(_type))
            {
                _blocks.Add(BuildAuthenticateBlock());
            }
        }

        private void CreateSetContextBlock()
        {
            if (typeof (INeedContext).IsAssignableFrom(_type))
            {
                _blocks.Add(BuildSetContextBlock());
            }
        }

        private void CreateSetFilesBlock()
        {
            if (typeof (IUploadFiles).IsAssignableFrom(_type))
            {
                _blocks.Add(BuildSetFilesBlock());
            }
        }

        private void CreateSetInputBlock()
        {
            if (_type.GetInterface(typeof (IInput<>).Name) != null)
            {
                _blocks.Add(BuildSetInputBlock());
            }
        }

        private void CreateRunBlock()
        {
            _blocks.Add(BuildRunBlock());
        }

        private void CreateWriteStatusBlock()
        {
            _blocks.Add(BuildWriteStatus());
        }

        private void CreateSetCookiesBlock()
        {
            if (typeof (ISetCookies).IsAssignableFrom(_type))
            {
                _blocks.Add(BuildSetCookiesBlock());
            }
        }

        private void CreateNoCacheBlock()
        {
            if (typeof (INoCache).IsAssignableFrom(_type))
            {
                _blocks.Add(Expression.Call(_methodLookup.DisableCache, _context));
            }
        }

        private void CreateRedirectBlock()
        {
            if (typeof (IMayRedirect).IsAssignableFrom(_type))
            {
                _blocks.Add(BuildRedirectBlock());
            }
        }

        private void CreateOutputBlocks()
        {
            if (typeof (IOutputStream).IsAssignableFrom(_type))
            {
                _blocks.Add(Expression.Call(_methodLookup.WriteStreamResponse, _endpoint, _context));
            }
            else if (typeof (IOutput<RawHtml>).IsAssignableFrom(_type))
            {
                _blocks.Add(Expression.Call(_methodLookup.WriteRawHtml, _endpoint, _context));
            }
            else if (_type.GetInterface(typeof (IOutput<>).Name) != null)
            {
                _blocks.Add(BuildWriteOutputBlock());
            }
            else if (typeof (ISpecifyView).IsAssignableFrom(_type))
            {
                _blocks.Add(Expression.Call(_methodLookup.WriteView, _endpoint, _context));
            }
        }

        private void CreateDisposeBlock()
        {
            if (typeof(IDisposable).IsAssignableFrom(_type))
            {
                _blocks.Add(Expression.Call(_endpoint, typeof(IDisposable).GetMethod("Dispose")));
            }
        }

        private Expression BuildWriteStatus()
        {
            return Expression.Call(_methodLookup.WriteStatusCode, _status, _context);
        }

        private Expression BuildRunBlock()
        {
            var verb = HttpVerbAttribute.Get(_type.GetInterfaces().Single(HttpVerbAttribute.IsAppliedTo));
            var run = _type.GetMethod(verb.Method);
            return Expression.Assign(_status, Expression.Call(_endpoint, run));
        }

        private Expression BuildAsyncRunBlock()
        {
            var verb = HttpVerbAttribute.Get(_type.GetInterfaces().Single(HttpVerbAttribute.IsAppliedTo));
            var run = _type.GetMethod(verb.Method);
            return Expression.Assign(_task, Expression.Call(_endpoint, run));
        }

        private Expression BuildSetInputBlock()
        {
            var inputType = _type.GetInterface(typeof (IInput<>).Name).GetGenericArguments().Single();
            var setInput = _methodLookup.SetInput.MakeGenericMethod(inputType);
            return Expression.Call(setInput, _endpoint, _context);
        }

        private Expression BuildWriteOutputBlock()
        {
            var inputType = _type.GetInterface(typeof (IOutput<>).Name).GetGenericArguments().Single();
            var writeOutput = _methodLookup.WriteOutput.MakeGenericMethod(inputType);
            return Expression.Call(writeOutput, _endpoint, _context);
        }

        private Expression BuildSetContextBlock()
        {
            return Expression.Assign(Expression.Property(_endpoint, typeof (INeedContext).GetProperty("Context")), _context);
        }

        private Expression BuildSetFilesBlock()
        {
            return Expression.Call(_methodLookup.SetFiles, _endpoint, _context);
        }

        private Expression BuildAuthenticateBlock()
        {
            return Expression.IfThen(Expression.Not(Expression.Call(_methodLookup.CheckAuthentication, _endpoint, _context)), Expression.Return(_end));
        }

        private Expression BuildRedirectBlock()
        {
            return Expression.IfThen(Expression.Call(_methodLookup.Redirect, _endpoint, _status, _context), Expression.Return(_end));
        }

        private Expression BuildSetCookiesBlock()
        {
            return Expression.Call(_methodLookup.SetCookies, _endpoint, _context);
        }
    }

    internal class AsyncRunner
    {
        private readonly Func<object, IContext, Task<Status>> _start;
        private readonly Action<object, IContext, Status> _end;

        public AsyncRunner(Func<object, IContext, Task<Status>> start, Action<object, IContext, Status> end)
        {
            _start = start;
            _end = end;
        }

        public Action<object, IContext, Status> End
        {
            get { return _end; }
        }

        public Func<object, IContext, Task<Status>> Start
        {
            get { return _start; }
        }
    }
}