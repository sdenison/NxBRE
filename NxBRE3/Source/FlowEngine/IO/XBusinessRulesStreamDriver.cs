namespace NxBRE.FlowEngine.IO {
	using System;
	using System.IO;
	using System.Reflection;
	using System.Text;
	using System.Xml;
	using System.Xml.XPath;
	using System.Xml.Xsl;
	
	using NxBRE.FlowEngine;
	using NxBRE.Util;

	/// <summary>
	/// Driver for loading rules streams valid against xBusinessRules.xsd (simplified NxBRE grammar).
	/// The native NxBRE rules file will be generated by XSLT.
	/// </summary>
	/// <author>David Dossot</author>
	public class XBusinessRulesStreamDriver : AbstractRulesDriver {
		Stream xmlStream;
		
		public XBusinessRulesStreamDriver(Stream xmlStream) {
			if (xmlStream == null)
				throw new BRERuleFatalException("Null is not a valid XML source");

			this.xmlStream = xmlStream;
		}
		
		protected override XmlValidatingReader GetReader() {
			//Loading the XSL file in a XSLTransform object
			XslTransform transform=new XslTransform();
			
			transform.Load(new XmlTextReader(Parameter.GetEmbeddedResourceStream("transformXRules.xsl")),null,null);
			
		 	//We have the xmlSource in hand, transform it to the native NXBRE XSD Format
			MemoryStream xsltResult = new MemoryStream();
			XmlValidatingReader xmlSourceReader = (XmlValidatingReader) GetXmlInputReader(xmlStream, "xBusinessRules.xsd");
			transform.Transform(new XPathDocument(xmlSourceReader),
			                    null,
			                    xsltResult,
			                    null);
			xmlSourceReader.Close();
			
			xsltResult.Seek(0, SeekOrigin.Begin);
			
			return (XmlValidatingReader) GetXmlInputReader(xsltResult, "businessRules.xsd");
		}
	}
}
