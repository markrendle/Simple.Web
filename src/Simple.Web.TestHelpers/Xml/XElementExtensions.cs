namespace Simple.Web.TestHelpers.Xml
{
    using System.Collections.Generic;
    using System.Xml.Linq;

    using Xunit.Sdk;

    public static class XElementExtensions
    {
        public static void ShouldEqual(this XElement actual, XElement expected)
        {
            if (actual == null)
            {
                if (expected == null)
                {
                    return;
                }
                throw new EqualException(expected, null);
            }
            actual.Name.ShouldEqual(expected.Name);
            actual.Attributes().ShouldEqual(expected.Attributes());
            if (expected.HasElements != actual.HasElements)
            {
                throw new EqualException(expected, actual);
            }
            if (actual.HasElements)
            {
                actual.Elements().ShouldEqual(expected.Elements());
            }
            else
            {
                if (actual.Value != expected.Value)
                {
                    throw new EqualException(expected, actual);
                }
            }
        }

        public static void ShouldEqual(this IEnumerable<XElement> actual, IEnumerable<XElement> expected)
        {
            var actualList = new List<XElement>(actual);
            var expectedList = new List<XElement>(expected);
            //order matters? depends on schema, so currently we have to assume that it does.
            int i = 0;
            for (; (i < actualList.Count) && (i < expectedList.Count); i++)
            {
                actualList[i].ShouldEqual(expectedList[i]);
            }
            if (expectedList.Count != actualList.Count)
            {
                object e = i < expectedList.Count ? expectedList[i] : null;
                object a = i < actualList.Count ? actualList[i] : null;
                throw new EqualException(e, a);
                //throw new EqualException(expectedList, actualList);
            }
        }

        public static void ShouldEqual(this XElement actual, string expectedXml)
        {
            XElement expected = XElement.Parse(expectedXml);
            actual.ShouldEqual(expected);
        }
    }
}