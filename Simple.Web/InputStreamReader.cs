using System.Collections.Generic;
using System.Text;

namespace Simple.Web
{
    class InputStreamReader
    {
        private static readonly Dictionary<string, IInputDeserializer> Deserializers = new Dictionary
            <string, IInputDeserializer>
                                                                                           {
                                                                                               {
                                                                                                   "application/x-www-form-urlencoded",
                                                                                                   new FormDeserializer()
                                                                                                   }
                                                                                           };

        public static IInputDeserializer GetDeserializer(string contentType)
        {
            return Deserializers[contentType];
        }
    }
}
