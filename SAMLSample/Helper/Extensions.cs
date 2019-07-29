using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Linq;

namespace SAMLSample.Helper
{
    public static class Extensions
    {
        /// <summary>
        /// Checks if the stram has valid XML
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <returns>bool</returns>
        public static bool IsValidXml(this Stream stream)
        {
            try
            {
                new XmlDocument().Load(stream);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Converts XMLDocument to XDocument
        /// </summary>
        /// <param name="xmlDocument">XmlDocument</param>
        /// <returns>XDocument</returns>
        public static XDocument ToXDocument(this XmlDocument xmlDocument)
        {
            using (XmlNodeReader reader = new XmlNodeReader(xmlDocument))
            {
                reader.MoveToContent();
                return XDocument.Load(reader);
            }
        }

        /// <summary>
        /// Converts XDocument to XmlDocument
        /// </summary>
        /// <param name="xDocument">XDocument</param>
        /// <returns>XmlDocument</returns>
        public static XmlDocument ToXmlDocument(this XDocument xDocument)
        {
            XmlDocument xmlDocument = new XmlDocument();
            using (XmlReader reader = xDocument.CreateReader())
            {
                xmlDocument.Load(reader);
                return xmlDocument;
            }
        }
        
    }
}