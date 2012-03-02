namespace Simple.Web
{
    using System;
    using System.IO;

    public interface IInputDeserializer
    {
        object Deserialize(Stream stream, Type type);
    }
}