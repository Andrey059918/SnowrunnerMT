using IEnumerablePlus;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Xml;

namespace SnowrunnerMT {
	class Tweaks {
		private Random rnd;
		public List<TweakToApply> Common;
		public List<SpecialTweaks> Spec;
		private Form_SnowrunnerMT frm;

		public Tweaks(Form_SnowrunnerMT form) {
			rnd = new Random();
			Common = new List<TweakToApply>();
			Spec = new List<SpecialTweaks>();
			frm = form;
		}

		public void GetValuesRecursive(String path, String dir) {
			XmlDocument doc;
			List<TweakToApply> DirTweaks = Common.Where(tw => tw.DirectoryName.Split('-').Any(a => a == Path.GetFileName(path))).ToList();
			if (DirTweaks.Count > 0) {
				Directory.EnumerateFiles(path).ToList().ForEach(file => {
						doc = new XmlDocument();
						doc.LoadXml("<root>" + File.ReadAllText(file) + "</root>");
						var t=ReadAttributes(doc.DocumentElement, DirTweaks);
						if (t.Count > 0) {
							File.AppendAllText(frm.ErrorFile, file+"\n");
							File.AppendAllLines(frm.ErrorFile, t);
						}
				});
			}

			Directory.EnumerateDirectories(path).ToList().ForEach(a => GetValuesRecursive(a, dir));
		}
		public void ProcessRecursive(String path, String dir) {
			XmlDocument doc = new XmlDocument();
			List<TweakToApply> CommonTweaks = Common.Where(tw => tw.DirectoryName.Split('-').Any(a => a == Path.GetFileName(path)) && tw.Action != 0).ToList();
			List<SpecialTweaks> SpecList = Spec.Where(tw => tw.Directories.Contains(Path.GetFileName(path)) && tw.Parameters.Count > 0).ToList();
			Directory.EnumerateFiles(path).ToList().ForEach(file => {
				try {
					if (CommonTweaks.Count > 0 || SpecList.Count > 0) {
						doc.LoadXml("<root>" + File.ReadAllText(file) + "</root>");
						if (SpecList.Count > 0) {
							Special_Modify(doc, SpecList);
						}
						if (CommonTweaks.Count > 0) {
							CommonTweaks.ForEach(tw => tw.Values = new List<Double>());
							var t=ReadAttributes(doc.DocumentElement, CommonTweaks);
							if (t.Count > 0) {
								File.AppendAllText(frm.ErrorFile, file + "\n");
								File.AppendAllLines(frm.ErrorFile, t);
							}
							CommonTweaks.ForEach(tw => {
								switch (tw.TweakType) {
									case "Integer":
										ModifyNumber(tw);
										break;
									case "Real":
										ModifyNumber(tw);
										break;
									case "Boolean":
										ModifyBool(tw);
										break;
								}
							});
							WriteAttributes(doc.DocumentElement, CommonTweaks);
						}
						Directory.CreateDirectory(path.Replace(dir, "SMT_Files\\3_Edited"));
						XMLExporter exp = new XMLExporter() { OmitRoot = true };
						exp.Export(doc, file.Replace(dir, "SMT_Files\\3_Edited"));
					}
				}
				catch (Exception e) {
					File.AppendAllText(frm.ErrorFile, $"{SaveBackups.Timestamp()} - {e.Message} - File {file}");
				}
			});

			Directory.EnumerateDirectories(path).ToList().ForEach(a => ProcessRecursive(a, dir));
		}
		public List<String> ReadAttributes(XmlNode node, List<TweakToApply> tweaks) {
			List<String> exc = new List<String>();
			TweakToApply attrtweak;
			if (node.Attributes != null) {
				var attrs = node.Attributes.Cast<XmlAttribute>().ToList();
				attrs.ForEach(attr => {
					try {
						attrtweak = tweaks.Find(tw => tw.ParameterName == attr.Name);
						if (attrtweak != null) {
							if (attr.Value != "") {
								attrtweak.Values.Add(attr.Value.ToLower() == "true" ? 1 : attr.Value.ToLower() == "false" ? 0 : Convert.ToDouble(attr.Value.Replace('.', ',')));
							}
							else {
								attrtweak.Values.Add(0);
							}
						}
					}
					catch {
						exc.Add($"{attr.Name}={attr.Value}");
					}
				});
			}
			node.ChildNodes.Cast<XmlNode>().ForEach(nd =>exc.AddRange(ReadAttributes(nd, tweaks)));
			return exc.Select(a=>a=node.Name+"\\"+a).ToList();
		}
		public void WriteAttributes(XmlNode node, List<TweakToApply> tweaks) {
			TweakToApply attrtweak;
			if (node.Attributes != null) {
				var attrs = node.Attributes.Cast<XmlAttribute>().ToList();
				attrs.ForEach(attr => {
					attrtweak = tweaks.Find(tw => tw.ParameterName == attr.Name);
					if (attrtweak != null) {
						if (attrtweak.TweakType == "Real") {
							attr.Value = attrtweak.Values[0].ToString("F3").TrimEnd(',', '0').Replace(',', '.');
							if (attr.Value == "") {
								attr.Value = "0";
							}
						}
						else if (attrtweak.TweakType == "Integer") {
							attr.Value = attrtweak.Values[0].ToString("F0");
						}
						else if (attrtweak.TweakType == "Boolean") {
							attr.Value = attrtweak.Values[0] > 0.5 ? "true" : "false";
						}
						attrtweak.Values.RemoveAt(0);
					}
				});
			}

			node.ChildNodes.Cast<XmlNode>().ForEach(nd => WriteAttributes(nd, tweaks));
		}
		public void ModifyNumber(TweakToApply tta) {
			Double max = Math.Max(tta.Val1, tta.Val2);
			Double min = Math.Min(tta.Val1, tta.Val2);
			Int32 cnt = tta.Values.Count - 1;
			Double a;
			for (int i = 0; i < tta.Values.Count; i++) {
				a = tta.Values[0];
				tta.Values.RemoveAt(0);
				switch (tta.Action) {
					case 1:
						a += tta.Val1;
						break;
					case 2:
						a -= tta.Val1;
						break;
					case 3:
						a *= tta.Val1;
						break;
					case 4:
						a /= tta.Val1;
						break;
					case 5:
						a = tta.Val1;
						break;
					case 6:
						a = tta.TweakType == "Real" ? rnd.NextDouble() * (max - min) + tta.Val1 : rnd.Next((Int32)min, (Int32)max);
						break;
					case 7:
						a = cnt == 0 ? max : ((max - min) * i / cnt + min);
						break;
					case 8:
						a = cnt == 0 ? min : ((max - min) * (cnt - i) / cnt + min);
						break;

				}
				tta.Values.Add(a);
			}
		}
		public void ModifyBool(TweakToApply tta) {
			Int32 cnt = tta.Values.Count - 1;
			Double a;
			for (int i = 0; i < tta.Values.Count; i++) {
				a = tta.Values[0];
				tta.Values.RemoveAt(0);
				switch (tta.Action) {
					case 1:
						a = 1;
						break;
					case 2:
						a = 0;
						break;
					case 3:
						a = rnd.Next(0, 100) < (Int32)tta.Val1 ? 1 : 0;
						break;
					case 4:
						a = cnt == 0 ? 0 : (100D * i / cnt) < tta.Val1 ? 1 : 0;
						break;
					case 5:
						a = cnt == 0 ? 1 : (100D * i / cnt) < tta.Val1 ? 0 : 1;
						break;
				}
				tta.Values.Add(a);
			}
		}

		public void Special_Modify(XmlDocument doc, List<SpecialTweaks> tweaks) {
			Int32 index = tweaks.FindIndex(a => a.Name == "trailerconnection");
			if (index >= 0) {
				if (tweaks[index].Parameters.Contains("htl")) {
					DoHighSaddleToLow(doc.DocumentElement);
				}
				if (tweaks[index].Parameters.Contains("stt")) {
					DoScoutToTrucks(doc.DocumentElement);
				}
				if (tweaks[index].Parameters.Contains("tts")) {
					DoTruckToScouts(doc.DocumentElement);
				}
			}
			index = tweaks.FindIndex(a => a.Name == "regionlock");
			if (index >= 0) {
				if (tweaks[index].Parameters.Contains("us")) {
					DoUsToAll(doc.DocumentElement);
				}
				if (tweaks[index].Parameters.Contains("ru")) {
					DoRuToAll(doc.DocumentElement);
				}
			}
			index = tweaks.FindIndex(a => a.Name == "colorsets");
			if (index>=0) {
				DoColorSets(doc.DocumentElement);
			}

		}
		public void DoHighSaddleToLow(XmlNode node) {
			if (node.Name == "RequiredAddon" && node.Attributes["_template"].Value == "SaddleHigh") {
				node.Attributes["_template"].Value = "SaddleLow, SaddleHigh";
			}
			else if (node.Name == "InstallSocket" && node.Attributes["Type"].Value == "LargeSemitrailer") {
				node.Attributes["Type"].Value = "Semitrailer";
			}
			else {
				node.ChildNodes.Cast<XmlNode>().ForEach(nd => DoHighSaddleToLow(nd));
			}
		}
		public void DoScoutToTrucks(XmlNode node) {
			List<XmlNode> nods = node.ChildNodes.Cast<XmlNode>().Where(a => a.Name == "InstallSocket").ToList();
			if (nods.Count == 1 && nods[0].Attributes["Type"].Value == "ScautTrailer") {
				XmlNode newnode = nods[0].CloneNode(true);
				node.AppendChild(newnode);
				newnode.Attributes["Type"].Value = "Trailer";
			}
			else {
				node.ChildNodes.Cast<XmlNode>().ForEach(nd => DoScoutToTrucks(nd));
			}
		}
		public void DoTruckToScouts(XmlNode node) {
			List<XmlNode> nods = node.ChildNodes.Cast<XmlNode>().Where(a => a.Name == "InstallSocket").ToList();
			if (nods.Count == 1 && nods[0].Attributes["Type"].Value == "Trailer") {
				XmlNode newnode = nods[0].CloneNode(true);
				node.AppendChild(newnode);
				newnode.Attributes["Type"].Value = "ScautTrailer";
			}
			else {
				node.ChildNodes.Cast<XmlNode>().ForEach(nd => DoTruckToScouts(nd));
			}
		}
		public void DoUsToAll(XmlNode node) {
			List<XmlNode> nods = node.ChildNodes.Cast<XmlNode>().Where(a => a.Name == "GameData").ToList();
			if (nods.Count == 1 && nods[0].Attributes["Country"].Value == "US") {
				nods[0].Attributes["Country"].Value = "";
			}
			else {
				node.ChildNodes.Cast<XmlNode>().ForEach(nd => DoUsToAll(nd));
			}
		}
		public void DoRuToAll(XmlNode node) {
			List<XmlNode> nods = node.ChildNodes.Cast<XmlNode>().Where(a => a.Name == "GameData").ToList();
			if (nods.Count == 1 && nods[0].Attributes["Country"].Value == "RU") {
				nods[0].Attributes["Country"].Value = "";
			}
			else {
				node.ChildNodes.Cast<XmlNode>().ForEach(nd => DoRuToAll(nd));
			}
		}
		public void DoColorSets(XmlNode node) {
			if (node.Name == "Truck") {
				for (int i = node.ChildNodes.Count; i > 0; i--) {
					if (node.ChildNodes[i - 1].Attributes.Count < 6) {
						node.RemoveChild(node.ChildNodes[i - 1]);
					}
				}
				for (int i = 0; i < node.ChildNodes.Count; i++) {
						node.ChildNodes[i ].Attributes[0].Value = i.ToString();
				}
				XmlElement nd = node as XmlElement;
				XmlElement cust;
				for (int i = 0; i < Spec[2].Values.Count / 3; i++) {
					cust = nd.OwnerDocument.CreateElement("CustomizationPreset");
					nd.AppendChild(cust);
					cust.SetAttribute("Id", (node.ChildNodes.Count-1).ToString());
					cust.SetAttribute("TintColor1", Spec[2].Values[i * 3]);
					cust.SetAttribute("TintColor2", Spec[2].Values[i * 3 + 1]);
					cust.SetAttribute("TintColor3", Spec[2].Values[i * 3 + 2]);
					cust.SetAttribute("MaterialOverrideName", "skin_00");
				}
			}
			else {
				node.ChildNodes.Cast<XmlNode>().ForEach(nd => DoColorSets(nd));
			}
		}
			
		public void ReadSockets(XmlNode node, SpecialTweaks tw) {
			if (node.Name == "GameData") {
				node.ChildNodes.Cast<XmlNode>().ForEach(a => {
					if (a.Name == "WinchSocket" || a.Name == "CraneSocket") {
						tw.Values.Add(a.Attributes["Pos"].Value);
						node.RemoveChild(a);
					}
				});
				var str=tw.Values.Select(b=>b.Split(';').Select(c=>Convert.ToSingle(c.Trim('(',')',' '))).ToList()).ToList();
				PointF min = new PointF(str.Min(a => a[0]), str.Min(a => a[2]));
				PointF max = new PointF(str.Max(a => a[0]), str.Max(a => a[2]));
				List<Single> Y = new List<float>();
				str.Sort((x, y) => x[1] == y[1] ? 0 : x[1] == y[1] ? 1 : -1);
				

			}
		}


		
		public void RemoveNegatives() {
			Common.ForEach(tw => {
				tw.Values.ForEach(val => {
					if (val < 0) {
						val = 0;
					}
				});
			});
		}
	}
}
