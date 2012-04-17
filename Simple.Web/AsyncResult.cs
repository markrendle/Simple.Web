namespace Simple.Web
{
    using System;
    using System.Threading;

    abstract class AsyncResult : IAsyncResult
    {
        public bool IsCompleted { get; protected set; }
        public WaitHandle AsyncWaitHandle { get; protected set; }
        public object AsyncState { get; protected set; }
        public bool CompletedSynchronously { get; protected set; }
    }
}