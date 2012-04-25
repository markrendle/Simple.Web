namespace Simple.Web.CodeGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    /// <summary>
    /// Builds methods to run endpoints for an <see cref="IContext"/>.
    /// </summary>
    internal class EndpointRunnerFactory
    {
        private readonly Type _type;
        private readonly IMethodLookup _methodLookup;
        private readonly List<Expression> _blocks = new List<Expression>(); 
        private readonly LabelTarget _end = Expression.Label("end");

        public EndpointRunnerFactory(Type type, IMethodLookup methodLookup = null)
        {
            if (type == null) throw new ArgumentNullException("type");
            _type = type;
            _methodLookup = methodLookup ?? new MethodLookup();
        }

        public Action<object, IContext> BuildRunner()
        {
            var endpointParameter = Expression.Parameter(typeof (object), "obj");
            var context = Expression.Parameter(typeof (IContext), "context");
            var endpoint = Expression.Variable(_type, "endpoint");
            var status = Expression.Variable(typeof (Status), "status");

            _blocks.Add(Expression.Assign(endpoint, Expression.Convert(endpointParameter, _type)));

            CreateAuthenticateBlock(context, endpoint);
            CreateSetContextBlock(context, endpoint);
            CreateSetFilesBlock(context, endpoint);
            CreateSetInputBlock(context, endpoint);
            CreateRunBlock(status, endpoint);
            CreateWriteStatusBlock(context, status);
            CreateSetCookiesBlock(context, endpoint);
            CreateNoCacheBlock(context);
            CreateRedirectBlock(context, status, endpoint);
            CreateOutputBlocks(context, endpoint);

            _blocks.Add(Expression.Label(_end));

            var block = Expression.Block(new[] {endpoint, context, status}, _blocks);

            return Expression.Lambda<Action<object, IContext>>(block, endpointParameter, context).Compile();
        }

        private void CreateAuthenticateBlock(ParameterExpression context, ParameterExpression endpoint)
        {
            if (typeof (IRequireAuthentication).IsAssignableFrom(_type))
            {
                _blocks.Add(BuildAuthenticateBlock(endpoint, context));
            }
        }

        private void CreateSetContextBlock(ParameterExpression context, ParameterExpression endpoint)
        {
            if (typeof (INeedContext).IsAssignableFrom(_type))
            {
                _blocks.Add(BuildSetContextBlock(endpoint, context));
            }
        }

        private void CreateSetFilesBlock(ParameterExpression context, ParameterExpression endpoint)
        {
            if (typeof (IUploadFiles).IsAssignableFrom(_type))
            {
                _blocks.Add(BuildSetFilesBlock(endpoint, context));
            }
        }

        private void CreateSetInputBlock(ParameterExpression context, ParameterExpression endpoint)
        {
            if (_type.GetInterface(typeof (IInput<>).Name) != null)
            {
                _blocks.Add(BuildSetInputBlock(endpoint, context));
            }
        }

        private void CreateRunBlock(ParameterExpression status, ParameterExpression endpoint)
        {
            _blocks.Add(BuildRunBlock(endpoint, status));
        }

        private void CreateWriteStatusBlock(ParameterExpression context, ParameterExpression status)
        {
            _blocks.Add(BuildWriteStatus(status, context));
        }

        private void CreateSetCookiesBlock(ParameterExpression context, ParameterExpression endpoint)
        {
            if (typeof (ISetCookies).IsAssignableFrom(_type))
            {
                _blocks.Add(BuildSetCookiesBlock(endpoint, context));
            }
        }

        private void CreateNoCacheBlock(ParameterExpression context)
        {
            if (typeof (INoCache).IsAssignableFrom(_type))
            {
                _blocks.Add(Expression.Call(_methodLookup.DisableCache, context));
            }
        }

        private void CreateRedirectBlock(ParameterExpression context, ParameterExpression status, ParameterExpression endpoint)
        {
            if (typeof (IMayRedirect).IsAssignableFrom(_type))
            {
                _blocks.Add(BuildRedirectBlock(endpoint, status, context));
            }
        }

        private void CreateOutputBlocks(ParameterExpression context, ParameterExpression endpoint)
        {
            if (typeof (IOutputStream).IsAssignableFrom(_type))
            {
                _blocks.Add(Expression.Call(_methodLookup.WriteStreamResponse, endpoint, context));
            }
            else if (typeof (IOutput<RawHtml>).IsAssignableFrom(_type))
            {
                _blocks.Add(Expression.Call(_methodLookup.WriteRawHtml, endpoint, context));
            }
            else if (_type.GetInterface(typeof (IOutput<>).Name) != null)
            {
                _blocks.Add(BuildWriteOutputBlock(endpoint, context));
            }
            else if (typeof (ISpecifyView).IsAssignableFrom(_type))
            {
                _blocks.Add(Expression.Call(_methodLookup.WriteView, endpoint, context));
            }
        }

        private Expression BuildWriteStatus(ParameterExpression status, ParameterExpression context)
        {
            return Expression.Call(_methodLookup.WriteStatusCode, status, context);
        }

        private Expression BuildRunBlock(ParameterExpression endpoint, ParameterExpression status)
        {
            var verb = HttpVerbAttribute.Get(_type.GetInterfaces().Single(HttpVerbAttribute.IsAppliedTo));
            var run = _type.GetMethod(verb.Method);
            return Expression.Assign(status, Expression.Call(endpoint, run));
        }

        private Expression BuildSetInputBlock(ParameterExpression endpoint, ParameterExpression context)
        {
            var inputType = _type.GetInterface(typeof (IInput<>).Name).GetGenericArguments().Single();
            var setInput = _methodLookup.SetInput.MakeGenericMethod(inputType);
            return Expression.Call(setInput, endpoint, context);
        }

        private Expression BuildWriteOutputBlock(ParameterExpression endpoint, ParameterExpression context)
        {
            var inputType = _type.GetInterface(typeof (IOutput<>).Name).GetGenericArguments().Single();
            var writeOutput = _methodLookup.WriteOutput.MakeGenericMethod(inputType);
            return Expression.Call(writeOutput, endpoint, context);
        }

        private Expression BuildSetContextBlock(ParameterExpression endpoint, ParameterExpression context)
        {
            return Expression.Assign(Expression.Property(endpoint, typeof (INeedContext).GetProperty("Context")), context);
        }

        private Expression BuildSetFilesBlock(ParameterExpression endpoint, ParameterExpression context)
        {
            return Expression.Call(_methodLookup.SetFiles, endpoint, context);
        }

        private Expression BuildAuthenticateBlock(ParameterExpression endpoint, ParameterExpression context)
        {
            return Expression.IfThen(Expression.Not(Expression.Call(_methodLookup.CheckAuthentication, endpoint, context)), Expression.Return(_end));
        }

        private Expression BuildRedirectBlock(ParameterExpression endpoint, ParameterExpression status, ParameterExpression context)
        {
            return Expression.IfThen(Expression.Call(_methodLookup.Redirect, endpoint, status, context), Expression.Return(_end));
        }

        private Expression BuildSetCookiesBlock(ParameterExpression endpoint, ParameterExpression context)
        {
            return Expression.Call(_methodLookup.SetCookies, endpoint, context);
        }
    }
}