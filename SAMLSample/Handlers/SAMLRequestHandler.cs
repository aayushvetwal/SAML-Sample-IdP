using System;
using System.IO;
using System.Web;
using System.Xml;
using SAMLSample.Helper;
using System.Xml.Linq;
using System.Text;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using SAMLSample.SamlLibrary;

namespace SAMLSample.Handlers
{
    public class SAMLRequestHandler : IHttpHandler
    {
        /// <summary>
        /// You will need to configure this handler in the Web.config file of your 
        /// web and register it with IIS before being able to use it. For more information
        /// see the following link: https://go.microsoft.com/?linkid=8101007
        /// </summary>

        #region Private Variables

        string _methodName = string.Empty;

        #endregion

        #region IHttpHandler Members

        public bool IsReusable
        {
            // Return false in case your Managed Handler cannot be reused for another request.
            // Usually this would be false in case you have some state information preserved per request.
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/xml; encoding = 'utf-8'";

            XmlDocument document = new XmlDocument();
            using (Stream stream = context.Request.InputStream)
            {
                if (stream.IsValidXml())
                {
                    if (stream.Position > 0)
                    {
                        stream.Position = 0;
                    }
                    using (XmlReader xmlReader = XmlReader.Create(stream))
                    {
                        document.Load(xmlReader);
                    }
                }
            }

            try
            {
                XDocument xDocument = document.ToXDocument();
                if (xDocument != null)
                {
                    _methodName = xDocument.Element("methodname").Value;
                }
            }
            catch { }


            switch (_methodName)
            {
                case "getsamltoken":
                    //byte[] bytes = Encoding.ASCII.GetBytes(SAMLGen.GetSamlToken().InnerXml);
                    byte[] bytes = Encoding.ASCII.GetBytes(GetSamlResponse());
                    using (Stream strm = context.Response.OutputStream)
                    {
                        strm.Write(bytes, 0, bytes.Length);
                    }
                    break;
                default:
                    break;
            }

        }

        /// <summary>
        /// Returns Saml Response
        /// </summary>
        /// <returns>string</returns>
        private string GetSamlResponse()
        {
            Dictionary<string, string> samlAttributes = GetSamlAttributes();

            // get the certificate
            String CertPath = System.Web.Hosting.HostingEnvironment.MapPath(@"~/App_Data/CoverMyMeds.pfx");
            X509Certificate2 signingCert = new X509Certificate2(CertPath, "4CoverMyMeds");

            return SAML20Assertion.CreateSAML20Response("SAMLSample", 5, "Audience", "SAML", "Recipient", samlAttributes, signingCert);
        }

        /// <summary>
        /// Gets Saml attributes
        /// </summary>
        /// <returns>Dictionary</returns>
        private Dictionary<string, string> GetSamlAttributes()
        {
            return new Dictionary<string, string>
            {
                { "Issuer", "SAMLSample" },
                { "id", "sampleId" },
                { "roles", "role1,role2,role3" },
                { "isAuthenticated", "1"},
            };
        }


        #endregion
    }

    public class SAMLGen
    {
        public static XmlDocument GetSamlToken()
        {
            XmlDocument samlDoc = new XmlDocument();
            samlDoc.Load("C:/Users/Aayush/Desktop/SamlSample.xml");
            return samlDoc;

            //XmlDocument doc = new XmlDocument();
            //XmlNode node = doc.CreateElement("sample");
            //node.InnerText = "Sample data";
            //doc.AppendChild(node);
            //return doc;

        }
    }
}
