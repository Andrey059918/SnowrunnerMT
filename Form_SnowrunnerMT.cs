using IEnumerablePlus;
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace SnowrunnerMT {
	public partial class Form_SnowrunnerMT : Form {
		private Dictionary<String, String> Localization;
		private String InitialPath = "", BackupHash = "", CurrentHash = "";
		private String SMTConfigVersion = ".1.4.0", SMTTweaksVersion = ".1.4.0", SMTLocalesVersion = ".1.5.0";
		private List<String> SMTPaths = new List<string>() { "SMT_Files\\1_Original", "SMT_Files\\2_Current", "SMT_Files\\3_Edited", "SMT_Files\\4_ModsBefore", "SMT_Files\\5_ModsAfter" };
		private ColorDialog ColDlg;
		public String ErrorFile;

		private List<TweakGroup> TweaksByDirectory;
		private List<TweakGroup> TweaksByParameter;
		private Tweaks AllTweaks;

		private Boolean HotkeyRecord1 = false, HotkeyRecord2 = false;
		private SaveBackups SaveBackups_Current;
		private Dictionary<Int32, SaveBackups> SaveBackups_All;


		public Form_SnowrunnerMT() {
			InitializeComponent();
			CenterToScreen();
			Locale_Prepare();
			Init_LoadConfig();
			Tweaks_Prepare();
			SaveBackups_Prepare();
			Init_SetEvents();
			ColDlg = new ColorDialog();
		}

		private void Locale_Prepare() {
			Localization = new Dictionary<string, string>();
			Directory.CreateDirectory("SMT_Files");
			foreach (var t in new String[] { "ru-RU", "en-US" }) {
				if (!File.Exists("SMT_Files\\" + t + SMTLocalesVersion + ".xml")) {
					File.WriteAllText("SMT_Files\\" + t + SMTLocalesVersion + ".xml", Properties.Resources.ResourceManager.GetObject(t, CultureInfo.InvariantCulture).ToString());
				}
			}

			String Culture = CultureInfo.CurrentCulture.Name;
			if (File.Exists("SMT_Files\\" + Culture + SMTLocalesVersion + ".xml")) {
				Culture = "SMT_Files\\" + Culture + SMTLocalesVersion + ".xml";
			}
			else {
				Culture = "SMT_Files\\en-US" + SMTLocalesVersion + ".xml";
			}
			XmlDocument doc = new XmlDocument();
			doc.Load(Culture);
			Locale_ReadXml(doc.DocumentElement);

			CT_TabButton.Text = Locale_GetString("Button_CommonTweaks");
			ST_TabButton.Text = Locale_GetString("Button_SpecialTweaks");
			SB_TabButton.Text = Locale_GetString("Button_SaveBackups");
			Button_GameDir.Text = Locale_GetString("Button_GameDir");
			Button_Apply.Text = Locale_GetString("Button_Apply");
			Button_Backup.Text = Locale_GetString("Button_Backup");
			Button_Restore.Text = Locale_GetString("Button_Restore");
			Button_ModsBefore.Text = Locale_GetString("Button_ModsBefore");
			Button_ModsAfter.Text = Locale_GetString("Button_ModsAfter");
			RadioButton_Original.Text = Locale_GetString("RButton_Original");
			RadioButton_Current.Text = Locale_GetString("RButton_Current");
			RadioButton_NoMods.Text = Locale_GetString("RButton_NoMods");
			RadioButton_Mods.Text = Locale_GetString("RButton_Mods");
			SB_AddButton.Text = Locale_GetString("Button_AddSlot");
			SB_BackupButton.Text = Locale_GetString("Button_BackupHK");
			SB_RestoreButton.Text = Locale_GetString("Button_RestoreHK");
			SB_PathButton.Text = Locale_GetString("Button_BackupPath");
			GroupBox_Tweaks.Text = Locale_GetString("GroupBox_Files");
			GroupBox_Mods.Text = Locale_GetString("GroupBox_Mods");
		}
		private void Locale_ReadXml(XmlNode node) {
			if (node.Attributes?.Count > 0) {
				for (int i = 0; i < node.Attributes.Count; i++) {
					Localization.Add(node.Attributes[i].Name, node.Attributes[i].Value);
				}
			}
			else if (node.ChildNodes?.Count > 0) {
				foreach (XmlNode nd in node.ChildNodes) {
					Locale_ReadXml(nd);
				}
			}
		}
		private String Locale_GetString(String key) {
			if (Localization.ContainsKey(key)) {
				return Localization[key];
			}
			return "(w/o lang)" + key;
		}

		private void Init_SetEvents() {
			Shown += (s, a) => {
				Init_SetSettings();
				CommonTweaks_GetValues();
			};
			Button_GameDir.Click += (s, a) => Init_SetSettings();
			Button_Apply.Click += (s, a) => Tweaks_Process();
			Button_Backup.Click += (s, a) => {
				FS_UnpackDirectory(SMTPaths[0]);
			};
			Button_Restore.Click += (s, a) => FS_PackDirectory(SMTPaths[0]);
			RadioButton_Current.CheckedChanged += (s, a) => CommonTweaks_GetValues();

			Button_ModsBefore.Click += (s, a) => {
				Directory.CreateDirectory(SMTPaths[3]);
				Process.Start(SMTPaths[3]);
			};
			Button_ModsAfter.Click += (s, a) => {
				Directory.CreateDirectory(SMTPaths[4]);
				Process.Start(SMTPaths[4]);
			};

			CT_TabButton.Click += (s, a) => {
				Panel_SaveBackups.SendToBack();
				FlowPanel_SpecialTweaks.SendToBack();
				CT_TabButton.Activated = 3;
				ST_TabButton.Activated = 0;
				SB_TabButton.Activated = 0;
			};
			ST_TabButton.Click += (s, a) => {
				Panel_SaveBackups.SendToBack();
				FlowPanel_CommonTweaks.SendToBack();
				CT_TabButton.Activated = 0;
				ST_TabButton.Activated = 3;
				SB_TabButton.Activated = 0;
			};
			SB_TabButton.Click += (s, a) => {
				FlowPanel_CommonTweaks.SendToBack();
				FlowPanel_SpecialTweaks.SendToBack();
				CT_TabButton.Activated = 0;
				ST_TabButton.Activated = 0;
				SB_TabButton.Activated = 3;
			};

			KeyDown += SaveBackups_KeyDown;
		}
		private void Init_LoadConfig() {
			SaveBackups_All = new Dictionary<int, SaveBackups>();
			if (File.Exists("SMT_Files\\config" + SMTConfigVersion + ".xml")) {
				XmlDocument doc = new XmlDocument();
				doc.Load("SMT_Files\\config" + SMTConfigVersion + ".xml");
				InitialPath = doc.DocumentElement.ChildNodes[0].Attributes[0].Value;
				BackupHash = doc.DocumentElement.ChildNodes[0].Attributes[1].Value;
				CurrentHash = doc.DocumentElement.ChildNodes[0].Attributes[2].Value;
				if (doc.DocumentElement.ChildNodes.Count > 1) {
					XmlNode sb = doc.DocumentElement.ChildNodes[1];
					XmlNode sbcn;
					String[] val;
					for (int i = 0; i < sb.ChildNodes?.Count; i++) {
						sbcn = sb.ChildNodes[i];
						SaveBackups_All.Add(i + 1, new SaveBackups(this.Handle, i + 1));
						SaveBackups_All[i + 1].SavePath = sbcn.Attributes["from"].Value;
						SaveBackups_All[i + 1].BackupPath = sbcn.Attributes["to"].Value;
						val = sbcn.Attributes["bhotkey"].Value.Split('-');
						SaveBackups_All[i + 1].SetBackupHotkey((Keys)(Convert.ToInt32(val[0]) << 16 | Convert.ToInt32(val[1])));
						val = sbcn.Attributes["rhotkey"].Value.Split('-');
						SaveBackups_All[i + 1].SetRestoreHotkey((Keys)(Convert.ToInt32(val[0]) << 16 | Convert.ToInt32(val[1])));
						SaveBackups_All[i + 1].LastTime = sbcn.Attributes["last"].Value;
						SaveBackups_All[i + 1].RegisterHotkeys(Convert.ToBoolean(sbcn.Attributes["active"].Value));
					}
				}
			}
		}
		private void Init_SaveConfig() {
			XmlDocument doc = new XmlDocument();
			XmlElement rt = doc.CreateElement("root");
			doc.AppendChild(rt);
			XmlElement gi = doc.CreateElement("GameInstance");
			rt.AppendChild(gi);
			gi.SetAttribute("path", InitialPath);
			gi.SetAttribute("hash1", BackupHash);
			gi.SetAttribute("hash2", CurrentHash);
			XmlElement sb = doc.CreateElement("SaveBackups");
			rt.AppendChild(sb);
			XmlElement sbentry;
			SaveBackups_All.ForEach(a => {
				sbentry = doc.CreateElement("Entry");
				sb.AppendChild(sbentry);
				sbentry.SetAttribute("from", a.Value.SavePath);
				sbentry.SetAttribute("to", a.Value.BackupPath);
				sbentry.SetAttribute("bhotkey", ((int)a.Value.BModifiers).ToString() + "-" + ((int)a.Value.BKey).ToString());
				sbentry.SetAttribute("rhotkey", ((int)a.Value.RModifiers).ToString() + "-" + ((int)a.Value.RKey).ToString());
				sbentry.SetAttribute("active", a.Value.IsActive.ToString());
				sbentry.SetAttribute("last", a.Value.LastTime);
			});


			XMLExporter exp = new XMLExporter();
			exp.Export(doc, "SMT_Files\\config" + SMTConfigVersion + ".xml");
		}
		private void Init_SetSettings() {
			Init_EnabledElements(false, true);
			if (!File.Exists(InitialPath)) {
				Init_FindInRegistry();
			}
			if (!File.Exists(InitialPath)) {
				Init_FindInDirectory();
			}
			if (!File.Exists(InitialPath)) {
				MessageBox.Show(Locale_GetString("MBoxText_InvalidPath"), Locale_GetString("MBoxTitle_InvalidPath"), MessageBoxButtons.OK, MessageBoxIcon.Error);
				Init_EnabledElements(true, false);
				return;
			}
			else {
				Button_GameDir.Text = InitialPath;
				DialogResult dlg = DialogResult.None;
				if (BackupHash == "") {
					dlg = MessageBox.Show(Locale_GetString("MBoxText_FirstLaunch"), Locale_GetString("MBoxTitle_FirstLaunch"), MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
				}
				else if (BackupHash != FS_GetHash()) {
					dlg = MessageBox.Show(Locale_GetString("MBoxText_GameUpdate"), Locale_GetString("MBoxTitle_GameUpdate"), MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
				}
				else if (!Directory.Exists(SMTPaths[0])) {
					FS_UnpackDirectory(SMTPaths[0]);
					BackupHash = FS_GetHash();
					if (CurrentHash != FS_GetHash()) {
						FS_CopyDirectory(SMTPaths[0], SMTPaths[1]);
						CurrentHash = FS_GetHash();
					}
				}

				if (dlg == DialogResult.OK) {
					FS_UnpackDirectory(SMTPaths[0]);
					BackupHash = FS_GetHash();
					if (CurrentHash != FS_GetHash()) {
						FS_CopyDirectory(SMTPaths[0], SMTPaths[1]);
						CurrentHash = FS_GetHash();
					}
				}
				else if (CurrentHash != FS_GetHash() || !Directory.Exists(SMTPaths[0])) {
					FS_UnpackDirectory(SMTPaths[1]);
					CurrentHash = FS_GetHash();
				}
				Init_SaveConfig();
				Init_EnabledElements(true, true);
			}
		}
		private void Init_FindInRegistry() {
			RegistryKey MuiCache = Registry.CurrentUser.OpenSubKey(@"Software\Classes\Local Settings\Software\Microsoft\Windows\Shell\MuiCache");
			MuiCache.GetValueNames().ForEach(val => {
				if (MuiCache.GetValue(val).ToString() == "SnowRunner" && val.EndsWith(".FriendlyAppName")) {
					String locale = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(val)));
					if (File.Exists(locale + @"\preload\paks\client\initial.pak")) {
						DialogResult dlg = MessageBox.Show(locale + "\\preload\\paks\\client\\initial.pak\n" + Locale_GetString("MBoxText_IsCorrectPath"), Locale_GetString("MBoxTitle_IsCorrectPath"), MessageBoxButtons.YesNo);
						if (dlg == DialogResult.Yes) {
							InitialPath = locale + @"\preload\paks\client\initial.pak";
						}
					}
				}
			});
		}
		private void Init_FindInDirectory() {
			FolderBrowserDialog dlg = new FolderBrowserDialog() { Description = Locale_GetString("FolderBrowser_SelectPath") };
			if (dlg.ShowDialog() == DialogResult.OK) {
				Directory.EnumerateDirectories(dlg.SelectedPath).ForEach(dir => {
					if (File.Exists(dir + @"\preload\paks\client\initial.pak")) {
						InitialPath = dir + @"\preload\paks\client\initial.pak";
					}
				});
			}
		}
		private void Init_EnabledElements(bool free, bool initial) {
			Enabled = free;
			if (initial) {
				Button_GameDir.Enabled = false;
				GroupBox_Tweaks.Enabled = true;
			}
			else {
				Button_GameDir.Enabled = true;
				GroupBox_Tweaks.Enabled = false;
			}
			if (Directory.Exists(SMTPaths[0])) {
				RadioButton_Original.Enabled = true;
				Button_Restore.Enabled = true;
			}
			else {
				RadioButton_Original.Enabled = false;
				Button_Restore.Enabled = false;
			}
			if (Directory.Exists(SMTPaths[1])) {
				RadioButton_Current.Enabled = true;
			}
			else {
				RadioButton_Current.Enabled = false;
			}
			if (RadioButton_Original.Enabled || RadioButton_Current.Enabled) {
				Button_Apply.Enabled = true;
			}
			else {
				Button_Apply.Enabled = false;
			}
			if (RadioButton_Current.Enabled && !RadioButton_Original.Enabled) {
				RadioButton_Current.Checked = true;
			}
			else if (!RadioButton_Current.Enabled && RadioButton_Original.Enabled) {
				RadioButton_Original.Checked = true;
			}
		}

		private void FS_UnpackFiles(String dir, Action act = null) {
			using (ZipArchive archive = ZipFile.OpenRead(InitialPath)) {
				for (int i = 0; i < archive.Entries.Count; i++) {
					if (archive.Entries[i].Name.EndsWith(".xml")) {
						string destinationPath = Application.StartupPath + "\\" + dir + "\\" + archive.Entries[i].FullName.Replace('/', '\\');
						Directory.CreateDirectory(Path.GetDirectoryName(destinationPath));
						archive.Entries[i].ExtractToFile(destinationPath);
						act?.Invoke();
					}
				}
			}
		}
		private void FS_PackFiles(String path, String dir, Action act = null) {
			var files = Directory.EnumerateFiles(path).ToList();
			if (files.Count >= 0) {
				using (ZipArchive archive = ZipFile.Open(InitialPath, ZipArchiveMode.Update)) {
					files.ForEach(file => {
						string entrypath = file.Replace(dir + "\\", "");
						archive.GetEntry(entrypath)?.Delete();
						archive.CreateEntryFromFile(file, entrypath);
						act?.Invoke();
					});
				}
			}
			Directory.EnumerateDirectories(path).ForEach(a => FS_PackFiles(a, dir, act));
		}
		private void FS_CopyFiles(string sourceDirName, string destDirName, Action act = null) {
			DirectoryInfo dir = new DirectoryInfo(sourceDirName);
			if (!dir.Exists) {
				return;
			}
			Directory.CreateDirectory(destDirName);

			FileInfo[] files = dir.GetFiles();
			foreach (FileInfo file in files) {
				string temppath = Path.Combine(destDirName, file.Name);
				file.CopyTo(temppath, true);
				act?.Invoke();
			}

			DirectoryInfo[] dirs = dir.GetDirectories();
			foreach (DirectoryInfo subdir in dirs) {
				string temppath = Path.Combine(destDirName, subdir.Name);
				FS_CopyFiles(subdir.FullName, temppath, act);
			}
		}
		private void FS_UnpackDirectory(String path) {
			Init_EnabledElements(false, true);
			textPBar1.VisualMode = ProgressBarDisplayMode.TextAndPercentage;
			textPBar1.Text = Locale_GetString("Progress_Unpack");
			textPBar1.Maximum = ZipFile.OpenRead(InitialPath).Entries.Where(a => a.Name.EndsWith(".xml")).Count();
			FS_DeleteDirectory(path);
			FS_UnpackFiles(path, () => textPBar1.Value++);
			textPBar1.VisualMode = ProgressBarDisplayMode.NoText;
			textPBar1.Value = 0;
			Init_EnabledElements(true, true);
		}
		private void FS_PackDirectory(String pth) {
			Init_EnabledElements(false, true);
			textPBar1.VisualMode = ProgressBarDisplayMode.TextAndPercentage;
			textPBar1.Text = Locale_GetString("Progress_Pack");
			textPBar1.Maximum = Directory.EnumerateFiles(pth, "*", SearchOption.AllDirectories).Count();
			FS_PackFiles(pth, pth, () => textPBar1.Value++);
			textPBar1.VisualMode = ProgressBarDisplayMode.NoText;
			textPBar1.Value = 0;
			Init_EnabledElements(true, true);
		}
		private void FS_CopyDirectory(String pth1, String pth2) {
			Init_EnabledElements(false, true);
			textPBar1.VisualMode = ProgressBarDisplayMode.TextAndPercentage;
			textPBar1.Text = Locale_GetString("Progress_Copy");
			textPBar1.Maximum = Directory.EnumerateFiles(pth1, "*", SearchOption.AllDirectories).Count();
			FS_CopyFiles(pth1, pth2, () => textPBar1.Value++);
			textPBar1.VisualMode = ProgressBarDisplayMode.NoText;
			textPBar1.Value = 0;
			Init_EnabledElements(true, true);
		}
		public bool FS_DeleteDirectory(string path) {
			bool errors = false;
			DirectoryInfo dir = new DirectoryInfo(path);
			if (!dir.Exists)
				return false;

			foreach (FileInfo fi in dir.EnumerateFiles()) {
				try {
					fi.IsReadOnly = false;
					fi.Delete();

					while (fi.Exists) {
						Thread.Sleep(10);
						fi.Refresh();
					}
				}
				catch {
					errors = true;
				}
			}

			foreach (DirectoryInfo di in dir.EnumerateDirectories()) {
				try {
					FS_DeleteDirectory(di.FullName);
				}
				catch {
					errors = true;
				}
			}

			dir.Delete();
			while (dir.Exists) {
				Thread.Sleep(10);
				dir.Refresh();
			}

			return !errors;

		}
		private String FS_GetHash() {
			using (var md5 = MD5.Create()) {
				using (var stream = File.OpenRead(InitialPath)) {
					return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLower();
				}
			}
		}

		private void Tweaks_Prepare() {
			TweaksByDirectory = new List<TweakGroup>();
			TweaksByParameter = new List<TweakGroup>();
			AllTweaks = new Tweaks(this);
			if (!File.Exists("SMT_Files\\tweaks" + SMTTweaksVersion + ".xml")) {
				File.WriteAllText("SMT_Files\\tweaks" + SMTTweaksVersion + ".xml", Properties.Resources.Tweaks);
			}

			XmlDocument doc = new XmlDocument();
			doc.Load("SMT_Files\\tweaks" + SMTTweaksVersion + ".xml");
			XmlNode parSpecific = doc.DocumentElement.ChildNodes[0];
			for (int i = 0; i < parSpecific.ChildNodes.Count; i++) {
				XmlNode tweakGroup = parSpecific.ChildNodes[i];
				TweaksByParameter.Add(new TweakGroup(tweakGroup.Attributes["parameter"].Value));
				for (int j = 0; j < tweakGroup.ChildNodes.Count; j++) {
					XmlNode tweak = tweakGroup.ChildNodes[j];
					TweaksByParameter[i].Tweaks.Add(new Tweak(tweak.Attributes["directory"].Value, tweakGroup.Attributes["type"].Value, AllTweaks.Common.Count));
					AllTweaks.Common.Add(new TweakToApply(tweak.Attributes["directory"].Value, tweakGroup.Attributes["parameter"].Value, tweakGroup.Attributes["type"].Value));
				}
			}
			XmlNode dirSpecific = doc.DocumentElement.ChildNodes[1];
			for (int i = 0; i < dirSpecific.ChildNodes.Count; i++) {
				XmlNode tweakGroup = dirSpecific.ChildNodes[i];
				TweaksByDirectory.Add(new TweakGroup(tweakGroup.Attributes["directory"].Value));
				for (int j = 0; j < tweakGroup.ChildNodes.Count; j++) {
					XmlNode tweak = tweakGroup.ChildNodes[j];
					TweaksByDirectory[i].Tweaks.Add(new Tweak(tweak.Attributes["parameter"].Value, tweak.Attributes["type"].Value, AllTweaks.Common.Count));
					AllTweaks.Common.Add(new TweakToApply(tweakGroup.Attributes["directory"].Value, tweak.Attributes["parameter"].Value, tweak.Attributes["type"].Value));
				}
			}

			SpecialTweaks_AddTrailerConnection();
			SpecialTweaks_AddTrucksCountry();
			SpecialTweaks_AddPaintjobs();

			TweaksByParameter.ForEach((a, n) => {
				CommonTweaks_AddGroup(a, n % 2 == 0);
			});
			TweaksByDirectory.ForEach((a, n) => {
				CommonTweaks_AddGroup(a, n % 2 == TweaksByParameter.Count % 2);
			});
			FlowPanel_CommonTweaks.Controls[0].Margin = new Padding(5, 5, 5, 0);
			FlowPanel_CommonTweaks.Controls[FlowPanel_CommonTweaks.Controls.Count - 1].Margin = new Padding(5, 0, 5, 5);
		}
		private void CommonTweaks_AddGroup(TweakGroup group, bool gray) {
			Panel grouppanel = new Panel() {
				Width = FlowPanel_CommonTweaks.ClientSize.Width - 10,
				Height = 25,
				Margin = new Padding(5, 0, 5, 0),
				BackColor = gray ? Color.FromArgb(230, 230, 230) : Color.FromArgb(240, 240, 240)
			};
			Label grouplabel = new Label() {
				Text = Locale_GetString(group.GroupName),
				AutoSize = true,
				Location = new Point(5, 5)
			};
			Label minlabel = new Label() {
				Text = Locale_GetString("Label_MinValue"),
				AutoSize = true,
				Location = new Point(275, 5),
				Visible = false
			};
			Label maxlabel = new Label() {
				Text = Locale_GetString("Label_MaxValue"),
				AutoSize = true,
				Location = new Point(350, 5),
				Visible = false
			};
			Label actionlabel = new Label() {
				Text = Locale_GetString("Label_Action"),
				AutoSize = true,
				Location = new Point(425, 5),
				Visible = false
			};
			Label paramlabel = new Label() {
				Text = Locale_GetString("Label_Parameters"),
				AutoSize = true,
				Location = new Point(530, 5),
				Visible = false
			};
			FlowLayoutPanel tweakspanel = new FlowLayoutPanel() {
				AutoSize = true,
				AutoSizeMode = AutoSizeMode.GrowAndShrink,
				MinimumSize = new Size(FlowPanel_CommonTweaks.ClientSize.Width - 45, 0),
				FlowDirection = FlowDirection.TopDown,
				WrapContents = false,
				Location = new Point(23, 25),
				Visible = false
			};
			grouppanel.Click += (sender, args) => {
				tweakspanel.Visible = !tweakspanel.Visible;
				minlabel.Visible = tweakspanel.Visible;
				maxlabel.Visible = tweakspanel.Visible;
				actionlabel.Visible = tweakspanel.Visible;
				paramlabel.Visible = tweakspanel.Visible;
				grouppanel.Height = tweakspanel.Visible ? tweakspanel.Height + 25 : 25;
				Width = 1084 + 17 * (FlowPanel_CommonTweaks.VerticalScroll.Visible ? 1 : 0);
			};
			grouplabel.Click += (sender, args) => {
				tweakspanel.Visible = !tweakspanel.Visible;
				minlabel.Visible = tweakspanel.Visible;
				maxlabel.Visible = tweakspanel.Visible;
				actionlabel.Visible = tweakspanel.Visible;
				paramlabel.Visible = tweakspanel.Visible;
				grouppanel.Height = tweakspanel.Visible ? tweakspanel.Height + 25 : 25;
				Width = 1084 + 17 * (FlowPanel_CommonTweaks.VerticalScroll.Visible ? 1 : 0);
			};

			FlowPanel_CommonTweaks.Controls.Add(grouppanel);
			grouppanel.Controls.Add(grouplabel);
			grouppanel.Controls.Add(minlabel);
			grouppanel.Controls.Add(maxlabel);
			grouppanel.Controls.Add(actionlabel);
			grouppanel.Controls.Add(paramlabel);
			grouppanel.Controls.Add(tweakspanel);

			group.Tweaks.ForEach(tweak => {
				tweakspanel.Controls.Add(CommonTweaks_AddTweak(tweak));
			});
		}
		private Panel CommonTweaks_AddTweak(Tweak tweak) {
			Panel tweakpanel = new Panel() {
				Width = FlowPanel_CommonTweaks.ClientSize.Width - 25,
				Margin = Padding.Empty,
				Height = 30
			};
			Label tweaklabel = new Label() {
				Text = Locale_GetString(tweak.TweakName),
				Location = new Point(4, 4),
				AutoSize = true
			};
			Label minvalue = new Label() {
				Location = new Point(250, 4),
				AutoSize = true
			};
			Label maxvalue = new Label() {
				Location = new Point(325, 4),
				AutoSize = true
			};
			ComboBox tweakmode = new ComboBox() {
				DropDownStyle = ComboBoxStyle.DropDownList,
				Width = 100,
				Location = new Point(400, 2)
			};
			if (tweak.TweakType == "Real" || tweak.TweakType == "Integer") {
				tweakmode.Items.AddRange(new string[] { "noth", "add", "subt", "mult", "div", "fix", "rand", "asc", "desc" }.Select(a => Locale_GetString("Action_" + a)).ToArray());
			}
			else if (tweak.TweakType == "Boolean") {
				tweakmode.Items.AddRange(new string[] { "noth", "btrue", "bfalse", "brand", "basc", "bdesc" }.Select(a => Locale_GetString("Action_" + a)).ToArray());
			}
			tweakmode.SelectedIndex = 0;
			NumericUpDown par1 = new NumericUpDown() {
				Width = 100,
				Maximum = UInt32.MaxValue,
				DecimalPlaces = 2,
				Location = new Point(505, 2),
				Enabled = false
			};
			NumericUpDown par2 = new NumericUpDown() {
				Width = 100,
				Maximum = UInt32.MaxValue,
				DecimalPlaces = 2,
				Location = new Point(610, 2),
				Enabled = false
			};

			if (tweak.TweakType == "Real" || tweak.TweakType == "Integer") {
				tweakmode.SelectedIndexChanged += (sender, args) => {
					AllTweaks.Common[tweak.ApplyIndex].Action = tweakmode.SelectedIndex;
					par1.Enabled = tweakmode.SelectedIndex > 0;
					par2.Enabled = tweakmode.SelectedIndex > 5;
				};
			}
			else if (tweak.TweakType == "Boolean") {
				tweakmode.SelectedIndexChanged += (sender, args) => {
					AllTweaks.Common[tweak.ApplyIndex].Action = tweakmode.SelectedIndex;
					par1.Enabled = tweakmode.SelectedIndex > 2;
				};
				par2.Visible = false;
			}
			par1.ValueChanged += (s, a) => AllTweaks.Common[tweak.ApplyIndex].Val1 = (Double)par1.Value;
			par2.ValueChanged += (s, a) => AllTweaks.Common[tweak.ApplyIndex].Val2 = (Double)par2.Value;
			tweakpanel.Controls.Add(par1);
			tweakpanel.Controls.Add(par2);
			AllTweaks.Common[tweak.ApplyIndex].MaxChanged += () => {
				if (tweak.TweakType == "Real") {
					maxvalue.Text = AllTweaks.Common[tweak.ApplyIndex].Max.ToString("F3").TrimEnd(',', '0');
				}
				else if (tweak.TweakType == "Integer") {
					maxvalue.Text = AllTweaks.Common[tweak.ApplyIndex].Max.ToString("F0");
				}
				else if (tweak.TweakType == "Boolean") {
					maxvalue.Text = AllTweaks.Common[tweak.ApplyIndex].Max > 0.5 ? "true" : "false";
				}
			};
			AllTweaks.Common[tweak.ApplyIndex].MinChanged += () => {
				if (tweak.TweakType == "Real") {
					minvalue.Text = AllTweaks.Common[tweak.ApplyIndex].Min.ToString("F3").TrimEnd(',', '0');
					if (minvalue.Text == "")
						minvalue.Text = "0";
				}
				else if (tweak.TweakType == "Integer") {
					minvalue.Text = AllTweaks.Common[tweak.ApplyIndex].Min.ToString("F0");
				}
				else if (tweak.TweakType == "Boolean") {
					minvalue.Text = AllTweaks.Common[tweak.ApplyIndex].Min < 0.5 ? "false" : "true";
				}
			};
			tweakpanel.Controls.Add(tweaklabel);
			tweakpanel.Controls.Add(tweakmode);
			tweakpanel.Controls.Add(minvalue);
			tweakpanel.Controls.Add(maxvalue);

			return tweakpanel;
		}
		private void CommonTweaks_GetValues() {
			ErrorFile = "SMT_Files\\Errors." + SaveBackups.Timestamp()+".log";
			AllTweaks.Common.ForEach(tw => {
				tw.Values = new List<Double>();
			});

			if (RadioButton_Original.Checked && Directory.Exists(SMTPaths[0])) {
				AllTweaks.GetValuesRecursive(SMTPaths[0], SMTPaths[0]);
				AllTweaks.RemoveNegatives();
			}
			else if (RadioButton_Current.Checked && Directory.Exists(SMTPaths[1])) {
				AllTweaks.GetValuesRecursive(SMTPaths[1], SMTPaths[1]);
				AllTweaks.RemoveNegatives();
			}

			AllTweaks.Common.ForEach(twe => {
				if (twe.Values.Count == 1) {
					twe.Max = twe.Values[0];
					twe.Min = twe.Values[0];
				}
				else if (twe.Values.Count > 1) {
					twe.Max = twe.Values.Max();
					twe.Min = twe.Values.Min();
				}
			});
		}
		private void SpecialTweaks_AddTrailerConnection() {
			Panel tweakpanel = new Panel() {
				Width = FlowPanel_SpecialTweaks.ClientSize.Width - 10,
				Height = 80,
				Margin = new Padding(5, 5, 5, 0),
				BackColor = Color.FromArgb(230, 230, 230)
			};
			Label tweaklabel = new Label() {
				Text = Locale_GetString("Group_TrailerConnection"),
				AutoSize = true,
				Location = new Point(5, 5)
			};
			CheckBox scouttotruck = new CheckBox() {
				Text = Locale_GetString("CheckBox_TrailerConnectionScoutToTruck"),
				AutoSize = true,
				Location = new Point(400, 30)
			};
			CheckBox trucktoscout = new CheckBox() {
				Text = Locale_GetString("CheckBox_TrailerConnectionTruckToScout"),
				AutoSize = true,
				Location = new Point(400, 55)
			};
			CheckBox hightolow = new CheckBox() {
				Text = Locale_GetString("CheckBox_TrailerConnectionSaddle"),
				AutoSize = true,
				Location = new Point(400, 5)
			};

			FlowPanel_SpecialTweaks.Controls.Add(tweakpanel);
			tweakpanel.Controls.Add(tweaklabel);
			tweakpanel.Controls.Add(hightolow);
			tweakpanel.Controls.Add(scouttotruck);
			tweakpanel.Controls.Add(trucktoscout);


			AllTweaks.Spec.Add(new SpecialTweaks("trailerconnection"));
			AllTweaks.Spec[0].Directories.Add("trailers");
			hightolow.CheckedChanged += (s, a) => {
				if (hightolow.Checked) {
					AllTweaks.Spec[0].Parameters.Add("htl");
				}
				else {
					AllTweaks.Spec[0].Parameters.Remove("htl");
				}
			};
			scouttotruck.CheckedChanged += (s, a) => {
				if (scouttotruck.Checked) {
					AllTweaks.Spec[0].Parameters.Add("stt");
				}
				else {
					AllTweaks.Spec[0].Parameters.Remove("stt");
				}
			};
			trucktoscout.CheckedChanged += (s, a) => {
				if (trucktoscout.Checked) {
					AllTweaks.Spec[0].Parameters.Add("tts");
				}
				else {
					AllTweaks.Spec[0].Parameters.Remove("tts");
				}
			};

		}
		private void SpecialTweaks_AddTrucksCountry() {
			Panel tweakpanel = new Panel() {
				Width = FlowPanel_SpecialTweaks.ClientSize.Width - 10,
				Height = 55,
				Margin = new Padding(5, 0, 5, 0),
				BackColor = Color.FromArgb(240, 240, 240)
			};
			Label tweaklabel = new Label() {
				Text = Locale_GetString("Group_TruckRegionLock"),
				AutoSize = true,
				Location = new Point(5, 5)
			};
			CheckBox ustoall = new CheckBox() {
				Text = Locale_GetString("CheckBox_TruckRegionLockUS"),
				AutoSize = true,
				Location = new Point(400, 30)
			};
			CheckBox rutoall = new CheckBox() {
				Text = Locale_GetString("CheckBox_TruckRegionLockRU"),
				AutoSize = true,
				Location = new Point(400, 5)
			};

			FlowPanel_SpecialTweaks.Controls.Add(tweakpanel);
			tweakpanel.Controls.Add(tweaklabel);
			tweakpanel.Controls.Add(ustoall);
			tweakpanel.Controls.Add(rutoall);


			AllTweaks.Spec.Add(new SpecialTweaks("regionlock"));
			AllTweaks.Spec[1].Directories.Add("trucks");
			ustoall.CheckedChanged += (s, a) => {
				if (ustoall.Checked) {
					AllTweaks.Spec[1].Parameters.Add("us");
				}
				else {
					AllTweaks.Spec[1].Parameters.Remove("us");
				}
			};
			rutoall.CheckedChanged += (s, a) => {
				if (rutoall.Checked) {
					AllTweaks.Spec[1].Parameters.Add("ru");
				}
				else {
					AllTweaks.Spec[1].Parameters.Remove("ru");
				}
			};

		}
		private void SpecialTweaks_AddPaintjobs() {
			Panel tweakpanel = new Panel() {
				Width = FlowPanel_SpecialTweaks.ClientSize.Width - 10,
				Height = 288,
				Margin = new Padding(5, 0, 5, 0),
				BackColor = Color.FromArgb(230, 230, 230)
			};
			Label tweaklabel = new Label() {
				Text = Locale_GetString("Group_PaintJobs"),
				AutoSize = true,
				Location = new Point(5, 5)
			};
			FlowLayoutPanel colorpanel = new FlowLayoutPanel() {
				Width = tweakpanel.Size.Width - 10,
				Height = 74,
				Location = new Point(5, 25),
				BackColor = Color.White,
				FlowDirection = FlowDirection.LeftToRight,
				AutoScroll = true
			};
			FlowLayoutPanel setspanel = new FlowLayoutPanel() {
				Width = tweakpanel.Size.Width - 10,
				Height = 174,
				Location = new Point(5, 105),
				BackColor = Color.White,
				FlowDirection = FlowDirection.LeftToRight,
				AutoScroll = true
			};
			Button addcolor = new Button() {
				Size = new Size(70, 70),
				Text =Locale_GetString("Button_AddColor"),
				Margin = new Padding(2),
				FlatStyle=0
			};
			addcolor.Click += (s, a) => {
				if (ColDlg.ShowDialog() == DialogResult.OK) {
					Color cl = ColDlg.Color;
					colorpanel.Controls.Add(SpecialTweaks_AddColor(cl,setspanel));
					AllTweaks.Spec[2].Parameters.Add(cl.R + " " + cl.G + " " + cl.B+" 0 0 0 0");
				}
			};
			AllTweaks.Spec.Add(new SpecialTweaks("colorsets"));
			AllTweaks.Spec[2].Directories.Add("customization_presets");

			FlowPanel_SpecialTweaks.Controls.Add(tweakpanel);
			tweakpanel.Controls.Add(tweaklabel);
			tweakpanel.Controls.Add(colorpanel);
			tweakpanel.Controls.Add(setspanel);
			colorpanel.Controls.Add(addcolor);
		}
		private Panel SpecialTweaks_AddColor(Color clr,FlowLayoutPanel sets) {
			Panel colorpanel = new Panel() {
				Width = 70,
				Height = 70,
				Margin = new Padding(2),
				BackColor = clr
			};
			CheckBox solid = new CheckBox() {
				Text = "",
				AutoSize = true,
				Location = new Point(5, 5),

			};
			CheckBox lay1 = new CheckBox() {
				Text = "",
				AutoSize = true,
				Location = new Point(5, 20)
			};
			CheckBox lay2 = new CheckBox() {
				Text = "",
				AutoSize = true,
				Location = new Point(5, 35)
			};
			CheckBox lay3 = new CheckBox() {
				Text = "",
				AutoSize = true,
				Location = new Point(5, 50)
			};
			new ToolTip().SetToolTip(solid, Locale_GetString("ToolTip_Solid"));
			new ToolTip().SetToolTip(lay1, Locale_GetString("ToolTip_Primary"));
			new ToolTip().SetToolTip(lay2, Locale_GetString("ToolTip_Secondary"));
			new ToolTip().SetToolTip(lay3, Locale_GetString("ToolTip_Details"));
			lay1.Click += (s, a) => {
				SpecialTweaks_RecheckColor(clr, solid, lay1, lay2, lay3);
				SpecialTweaks_AddColorsets(sets);
			};
			lay2.Click += (s, a) => {
				SpecialTweaks_RecheckColor(clr, solid, lay1, lay2, lay3);
				SpecialTweaks_AddColorsets(sets);
			};
			lay3.Click += (s, a) => {
				SpecialTweaks_RecheckColor(clr, solid, lay1, lay2, lay3);
				SpecialTweaks_AddColorsets(sets);
			};
			solid.Click += (s, a) => {
				SpecialTweaks_RecheckColor(clr, solid, lay1, lay2, lay3);
				SpecialTweaks_AddColorsets(sets);
			};
			colorpanel.Controls.Add(solid);
			colorpanel.Controls.Add(lay1);
			colorpanel.Controls.Add(lay2);
			colorpanel.Controls.Add(lay3);

			return colorpanel;
		}
		private void SpecialTweaks_RecheckColor(Color clr,CheckBox ch1, CheckBox ch2, CheckBox ch3, CheckBox ch4) {
			Int32 index = AllTweaks.Spec[2].Parameters.FindIndex(str => str.StartsWith(clr.R + " " + clr.G + " " + clr.B));
			String st = clr.R + " " + clr.G + " " + clr.B;
			st += (ch1.Checked ? " 1" : " 0") + (ch2.Checked ? " 1" : " 0") + (ch3.Checked ? " 1" : " 0") + (ch4.Checked ? " 1" : " 0");
			AllTweaks.Spec[2].Parameters[index]=st;
		}
		private void SpecialTweaks_AddColorsets(FlowLayoutPanel sets) {
			AllTweaks.Spec[2].Values.Clear();
			sets.Controls.Clear();
			List<List<Byte>> splits= AllTweaks.Spec[2].Parameters.Select(a=>a.Split(' ').Select(b=>Convert.ToByte(b)).ToList()).ToList();
			List<Bitmap> bmps = new List<Bitmap>();
			splits.Where(a => a[3] == 1).ForEach(a => {
				bmps.Add(ProcessImageMask(Properties.Resources.Layers, Color.FromArgb(a[0], a[1], a[2])));
				AllTweaks.Spec[2].Values.Add($"({a[0]};{a[1]};{a[2]})");
				AllTweaks.Spec[2].Values.Add($"({a[0]};{a[1]};{a[2]})");
				AllTweaks.Spec[2].Values.Add($"({a[0]};{a[1]};{a[2]})");
			});
			splits.Where(a => a[4] == 1).ForEach(a => {
				splits.Where(b => b[5] == 1).ForEach(b => {
					splits.Where(c => c[6] == 1).ForEach(c => {
						if (a[0] != b[0] || a[0] != c[0] || b[0] != c[0]
						|| a[1] != b[1] || a[1] != c[1] || b[1] != c[1]
						|| a[2] != b[2] || a[2] != c[2] || b[2] != c[2]){
							bmps.Add(CombineImages(
								ProcessImageMask(Properties.Resources.Layer1, Color.FromArgb(a[0],a[1],a[2])),
								ProcessImageMask(Properties.Resources.Layer2, Color.FromArgb(b[0], b[1], b[2])),
								ProcessImageMask(Properties.Resources.Layer3, Color.FromArgb(c[0], c[1], c[2]))));
							AllTweaks.Spec[2].Values.Add($"({a[0]};{a[1]};{a[2]})");
							AllTweaks.Spec[2].Values.Add($"({b[0]};{b[1]};{b[2]})");
							AllTweaks.Spec[2].Values.Add($"({c[0]};{c[1]};{c[2]})");
						}
					});
				});
			});
			bmps.ForEach(a => {
				PictureBox pb = new PictureBox() {
					Width = 87,
					Height = 87,
					Image = a,
					Margin=Padding.Empty
				};
				sets.Controls.Add(pb);
			});
		}
		private Bitmap ProcessImageMask(Bitmap mask, Color clr) {
			Bitmap bmp = new Bitmap(mask);
			unsafe {
				BitmapData bitmapData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, bmp.PixelFormat);

				int bytesPerPixel = System.Drawing.Bitmap.GetPixelFormatSize(bmp.PixelFormat) / 8;
				int heightInPixels = bitmapData.Height;
				int widthInBytes = bitmapData.Width * bytesPerPixel;
				byte* PtrFirstPixel = (byte*)bitmapData.Scan0;

				Parallel.For(0, heightInPixels, y =>
				{
					byte* currentLine = PtrFirstPixel + (y * bitmapData.Stride);
					for (int x = 0; x < widthInBytes; x = x + bytesPerPixel) {
						if (currentLine[x + 3] > 0) {
							currentLine[x] = (byte)(clr.B * currentLine[x]/255);
							currentLine[x + 1] = (byte)(clr.G * currentLine[x+1] / 255);
							currentLine[x + 2] = (byte)(clr.R * currentLine[x+2] / 255);
						}
					}
				});
				bmp.UnlockBits(bitmapData);
			}
			return bmp;
		}
		private Bitmap CombineImages(params Bitmap[] par) {
			Bitmap bm = new Bitmap(85, 82, PixelFormat.Format32bppArgb);
			Graphics g = Graphics.FromImage(bm);
			for (int i = 0; i < par.Length; i++) {
				g.DrawImage(par[i],Point.Empty);
			}
			return bm;
		}

		private void Tweaks_Process() {
			ErrorFile = "SMT_Files\\Errors." + SaveBackups.Timestamp() + ".log";
			FS_DeleteDirectory(SMTPaths[2]);
			if (RadioButton_Mods.Checked&&Directory.Exists(SMTPaths[3])) {
				FS_CopyDirectory(SMTPaths[3], SMTPaths[2]);
				AllTweaks.ProcessRecursive(SMTPaths[2], SMTPaths[2]);
				AllTweaks.RemoveNegatives();
			}
			if (RadioButton_Original.Checked && Directory.Exists(SMTPaths[0])) {
				AllTweaks.ProcessRecursive(SMTPaths[0], SMTPaths[0]);
				AllTweaks.RemoveNegatives();
			}
			else if (RadioButton_Current.Checked && Directory.Exists(SMTPaths[1])) {
				AllTweaks.ProcessRecursive(SMTPaths[1], SMTPaths[1]);
				AllTweaks.RemoveNegatives();
			}
			if (RadioButton_Mods.Checked && Directory.Exists(SMTPaths[4])) {
				FS_CopyDirectory(SMTPaths[4], SMTPaths[2]);
			}

			FS_PackDirectory(SMTPaths[2]);
			FS_CopyDirectory(SMTPaths[2], SMTPaths[1]);
			BackupHash = FS_GetHash();
			CurrentHash = BackupHash;
			Init_SaveConfig();
			SoundPlayer pl = new SoundPlayer(Properties.Resources.JobDone);
			pl.Play();
		}

		private void SaveBackups_Prepare() {
			SaveBackups_Current = new SaveBackups(this.Handle, SaveBackups_All.Count+1);

			String pth = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\My Games\\SnowRunner\\base\\storage";
			SB_ProfileBox.Items.Add(Locale_GetString("ComboBox_SelectProfile"));
			SB_ProfileBox.SelectedIndex = 0;
			Directory.EnumerateDirectories(pth).ForEach(a => {
				if (Path.GetFileName(a) != "backupSlots") {
					SB_ProfileBox.Items.Add(Path.GetFileName(a).PadRight(68, ' ') + Directory.GetLastWriteTime(a).ToString());
				}
			});
			SB_ProfileBox.SelectedIndexChanged += (s, a) => {
				if (SB_ProfileBox.SelectedIndex!=0)
				SaveBackups_Current.SavePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\My Games\\SnowRunner\\base\\storage\\" + SB_ProfileBox.SelectedItem?.ToString().Substring(0,68).TrimEnd(' ');
			};
			SB_PathButton.Click += (s, a) => {
				FolderBrowserDialog dlg = new FolderBrowserDialog() { Description = Locale_GetString("FolderBrowser_BackupPath") };
				if (dlg.ShowDialog() == DialogResult.OK) {
					SaveBackups_Current.BackupPath = dlg.SelectedPath;
					SB_PathButton.Text = dlg.SelectedPath;
				}
			};
			SB_BackupButton.Click += (s, a) => {
				SB_BackupButton.Enabled = false;
				HotkeyRecord1 = true;
			};
			SB_RestoreButton.Click += (s, a) => {
				SB_RestoreButton.Enabled = false;
				HotkeyRecord2 = true;
			};
			SB_AddButton.Click += (s, a) => {
				SaveBackups_Current.RegisterHotkeys(true);
				if (SaveBackups_Current.IsActive) {
					SaveBackups_All.Add(SaveBackups_Current.ID, SaveBackups_Current);
					SaveBackups_Current = new SaveBackups(this.Handle, SaveBackups_All.Count + 1);
					SB_BackupButton.Text = Locale_GetString("Button_BackupHK");
					SB_RestoreButton.Text = Locale_GetString("Button_RestoreHK");
					SB_PathButton.Text = Locale_GetString("Button_BackupPath");
					FlowPanel_SaveBackups.Controls.Clear();
					SaveBackups_All.ForEach(sbkp => SaveBackups_Display(sbkp.Value));
					Init_SaveConfig();
				}
				else {
					MessageBox.Show(Locale_GetString("MBoxText_RegHotkeys"), Locale_GetString("MBoxTitle_RegHotkeys"), MessageBoxButtons.OK);
				}
			};
			FlowPanel_SaveBackups.Controls.Clear();
			SaveBackups_All.ForEach(sbkp => SaveBackups_Display(sbkp.Value));
		}
		private void SaveBackups_KeyDown(object sender, KeyEventArgs args) {
			if (args.KeyCode != Keys.ShiftKey && args.KeyCode != Keys.ControlKey && args.KeyCode != Keys.Menu) {
				if (HotkeyRecord1) {
					if (SaveBackups_Current.SetBackupHotkey(args.KeyData)) {
						HotkeyRecord1 = false;
						SB_BackupButton.Enabled = true;
						SB_BackupButton.Text = SaveBackups_Current.GetBackupString();
					}
				}
				else if (HotkeyRecord2) {
					if (SaveBackups_Current.SetRestoreHotkey(args.KeyData)) {
						HotkeyRecord2 = false;
						SB_RestoreButton.Enabled = true;
						SB_RestoreButton.Text = SaveBackups_Current.GetRestoreString();
					}
				}
			}
		}
		private void SaveBackups_Display(SaveBackups item) {
			Panel grouppanel = new Panel() {
				Width = FlowPanel_SaveBackups.Width,
				Height = 80,
				Margin = new Padding(0, 0, 0, 5),
				BackColor = Color.FromArgb(230, 230, 230)
			};
			CheckBox chbox = new CheckBox() { 
				Text =Locale_GetString("Button_BackupHK")+": " + item.GetBackupString() + ";        " + Locale_GetString("Button_RestoreHK")+": " + item.GetRestoreString() + ";",
				AutoSize = true,
				Location = new Point(5, 5),
				Checked = item.IsActive
			};
			chbox.Click += (s, a) => {
				item.RegisterHotkeys(chbox.Checked);
				chbox.Checked = item.IsActive;
			};
			Label savepth = new Label() {
				Text = Locale_GetString("Label_From") + item.SavePath,
				AutoSize = true,
				Location = new Point(5, 30)
			};
			Label backpth = new Label() {
				Text = Locale_GetString("Label_To") + item.BackupPath,
				AutoSize = true,
				Location = new Point(5, 55)
			};
			Label remove = new Label() {
				Text = "X",
				AutoSize = true,
				Location = new Point(grouppanel.Right-20, 5),
				Anchor=AnchorStyles.Top|AnchorStyles.Right
			};
			remove.Click += (s, a) => {
				SaveBackups_All[item.ID].RegisterHotkeys(false);
				SaveBackups_All.Remove(item.ID);
				for (int i = 0; i < SaveBackups_All.Count; i++) {
					SaveBackups_All.ElementAt(i).Value.ChangeID(i + 1);
					SaveBackups_All.ChangeKey(SaveBackups_All.ElementAt(i).Key, i + 1);
				}
				SaveBackups_Current.ChangeID(SaveBackups_All.Count + 1);

				FlowPanel_SaveBackups.Controls.Clear();
				SaveBackups_All.ForEach(sbkp => SaveBackups_Display(sbkp.Value));
				Init_SaveConfig();
			};
			FlowPanel_SaveBackups.Controls.Add(grouppanel);
			grouppanel.Controls.Add(chbox);
			grouppanel.Controls.Add(savepth);
			grouppanel.Controls.Add(backpth);
			grouppanel.Controls.Add(remove);
		}
		protected override void WndProc(ref Message m) {
			if (m.Msg == 0x0312) {
				int id = (int)m.WParam;
				if (id > 0) {
					SaveBackups_All[id].BackupFiles();
				}
				else if (id < 0) {
					SaveBackups_All[-id].RestoreFiles();
				}
				Init_SaveConfig();
			}
			base.WndProc(ref m);
		}
	}
	
}
