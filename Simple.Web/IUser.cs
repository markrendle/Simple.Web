namespace Simple.Web
{
    using System;

    public interface IUser
    {
        Guid Guid { get; }
        string Name { get; }
        bool IsAuthenticated { get; }
    }
}