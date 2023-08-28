using Newtonsoft.Json.Linq;
using System;
using System.Xml;
namespace Engine.Shared
{
    public static class ExtensionMethods
    {
        public static int AttributeAsInt(this XmlNode node, string attributeName)
        {
            return Convert.ToInt32(node.AttributeAsString(attributeName));
        }

        public static string AttributeAsString(this XmlNode node, string attributeName)
        {
            XmlAttribute attribute = node.Attributes?[attributeName];
            if (attribute == null)
            {
                throw new ArgumentException($"The attribute '{attributeName}' does not exist");
            }
            return attribute.Value;
        }

        public static double AttributeAsDouble(this XmlNode node, string attributeName)
        {
            return Convert.ToDouble(node.AttributeAsString(attributeName));
        }
        public static bool AttributeAsBool(this XmlNode node, string attributeName)
        {
            return Convert.ToBoolean(node.AttributeAsString(attributeName));
        }

        public static string StringValueOf(this JObject jobj, string key)
        {
            return jobj[key].ToString();
        }
        public static string StringValueOf(this JToken jtok, string key)
        {
            return jtok[key].ToString();
        }
        public static int IntValueOf(this JToken jtok, string key)
        {
            return Convert.ToInt32(jtok[key]);
        }
    }
}
