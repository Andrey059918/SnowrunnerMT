using IEnumerablePlus;
using System;
using System.IO;
using System.Linq;
using System.Xml;

namespace SnowrunnerMT {
	public class XMLExporter {
		public Boolean OmitRoot = false;
		public UInt32 AttributesWithoutLBreak = 1;
		public String Indent = "	";
		public String LineBreak = "\r\n";

		private StreamWriter writer = null;

		public void Export(XmlDocument doc, String filename) {
			using (writer = new StreamWriter(filename)) {
				if (OmitRoot) {
					doc.DocumentElement.Cast<XmlNode>().ForEach(a => ExportNode(a, 0));
				}
				else {
					ExportNode(doc.DocumentElement, 0);
				}
			}
		}
		private void ExportNode(XmlNode node, Int32 indentLevel) {
			if (node.NodeType == XmlNodeType.Element) {
				for (int i = 0; i < indentLevel; i++) {
					writer.Write(Indent);
				}
				writer.Write("<" + node.Name);
				if (node.Attributes.Count > AttributesWithoutLBreak) {
					for (int i = 0; i < node.Attributes.Count; i++) {
						writer.Write(LineBreak);
						for (int j = 0; j < indentLevel + 1; j++) {
							writer.Write(Indent);
						}
						writer.Write(node.Attributes[i].Name + "=\"" + node.Attributes[i].Value + "\"");
					}
					writer.Write(LineBreak);
					for (int i = 0; i < indentLevel; i++) {
						writer.Write(Indent);
					}
				}
				else {
					for (int i = 0; i < node.Attributes.Count; i++) {
						writer.Write(" " + node.Attributes[i].Name + "=\"" + node.Attributes[i].Value + "\"");
					}
				}
				if (node.ChildNodes.Count == 0) {
					writer.Write("/>" + LineBreak);
				}
				else {
					writer.Write(">" + LineBreak);
					for (int i = 0; i < node.ChildNodes.Count; i++) {
						ExportNode(node.ChildNodes[i], indentLevel + 1);
					}
					for (int i = 0; i < indentLevel; i++) {
						writer.Write(Indent);
					}
					writer.Write("</" + node.Name + ">" + LineBreak);
				}
			}
		}
	}

}
