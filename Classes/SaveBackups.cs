using IEnumerablePlus;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Media;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SnowrunnerMT {
	class SaveBackups {
		[DllImport("User32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

		[DllImport("User32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

		public Int32 ID { get; private set; }
		private IntPtr Handle;
		public uint BKey {
			get; private set;
		} = 0;
		public uint BModifiers {
			get; private set;
		} = 0;
		public uint RKey {
			get; private set;
		} = 0;
		public uint RModifiers {
			get; private set;
		} = 0;
		private uint BfsModifiers = 0;
		private uint RfsModifiers = 0;
		public String SavePath;
		public String BackupPath;
		public Boolean IsActive { get; private set; } = false;
		public String LastTime;

		public SaveBackups(IntPtr handle, Int32 id) {
			Handle = handle;
			ID = id;
		}
		~SaveBackups() {
			RegisterHotkeys(false);
		}
		public void ChangeID(Int32 id) {
			bool active=IsActive;
			RegisterHotkeys(false);
			ID = id;
			if(active)
				RegisterHotkeys(true);
		}

		public bool SetBackupHotkey(Keys key) {
			BKey = (uint)((int)key&UInt16.MaxValue);
			BModifiers = (uint)((int)key>>16 & 7);
			BfsModifiers = Convert.ToUInt32(String.Concat(Convert.ToString(BModifiers,2).PadLeft(3,'0').Reverse()),2) | 0x4000;
			if (RegisterHotKey(Handle, ID, BfsModifiers, BKey)) {
				UnregisterHotKey(Handle, ID);
				return true;
			}
			else {
				BModifiers = 0;
				BKey = 0;
				BfsModifiers = 0;
				return false;
			}
		}
		public bool SetRestoreHotkey(Keys key) {
			RKey = (uint)((int)key & UInt16.MaxValue);
			RModifiers = (uint)((int)key >> 16 & 7);
			RfsModifiers = Convert.ToUInt32(String.Concat(Convert.ToString(RModifiers, 2).PadLeft(3, '0').Reverse()), 2) | 0x4000;
			if (RegisterHotKey(Handle, ID, RfsModifiers, RKey)) {
				UnregisterHotKey(Handle, ID);
				return true;
			}
			else {
				RModifiers = 0;
				RKey = 0;
				RfsModifiers = 0;
				return false;
			}
		}
		public void RegisterHotkeys(bool state) {
			if (state) {
				if (RegisterHotKey(Handle, ID, BfsModifiers, BKey) && RegisterHotKey(Handle, -ID, RfsModifiers, RKey)) {
					IsActive = true;
				}
				else {
					UnregisterHotKey(Handle, ID);
					UnregisterHotKey(Handle, -ID);
					IsActive = false;
				}
			}
			else {
				UnregisterHotKey(Handle, ID);
				UnregisterHotKey(Handle, -ID);
				IsActive = false;
			}
		}
		public void BackupFiles() {
			LastTime = Timestamp();
			Directory.CreateDirectory(BackupPath);
			using (ZipArchive archive = ZipFile.Open(BackupPath + "\\backup." + LastTime+".zip", ZipArchiveMode.Create)) {
				Directory.EnumerateFiles(SavePath).Where(file => Path.GetFileName(file).Contains("_level_") || Path.GetFileName(file) == "CompleteSave.dat").ForEach(file => {
					string entrypath = file.Replace(SavePath + "\\", "");
					archive.CreateEntryFromFile(file, entrypath);
				});
			}
			SoundPlayer pl = new SoundPlayer(Properties.Resources.JobDone);
			pl.Play();
		}
		public void RestoreFiles() {		    
			Directory.GetFiles(SavePath,"*_level_*").ForEach(file => {
				File.Delete(file);
				System.Threading.Thread.Sleep(10);
			});
			using (ZipArchive archive = ZipFile.OpenRead(BackupPath + "\\backup." + LastTime + ".zip")) {
				for (int i = 0; i < archive.Entries.Count; i++) {
					string destinationPath = SavePath + "\\" + archive.Entries[i].FullName.Replace('/', '\\');
					Directory.CreateDirectory(Path.GetDirectoryName(destinationPath));
					archive.Entries[i].ExtractToFile(destinationPath, true);
				}
			}
			SoundPlayer pl = new SoundPlayer(Properties.Resources.JobDone);
			pl.Play();
		}

		public String GetBackupString() {
			String ret="";
				if ((BfsModifiers & 1) == 1) 
				ret += "Alt+";
			if ((BfsModifiers & 2) == 2)
				ret += "Ctrl+";
			if ((BfsModifiers & 4) == 4)
				ret += "Shift+";
			ret += (Keys)BKey;
			return ret;
		}
		public String GetRestoreString() {
			String ret = "";
			if ((RfsModifiers & 1) == 1)
				ret += "Alt+";
			if ((RfsModifiers & 2) == 2)
				ret += "Ctrl+";
			if ((RfsModifiers & 4) == 4)
				ret += "Shift+";
			ret += (Keys)RKey;
			return ret;
		}
		public static String Timestamp() {
			DateTime dt = DateTime.Now;
			return String.Join("-", dt.Year.ToString(), dt.Month.ToString().PadLeft(2, '0'), dt.Day.ToString().PadLeft(2, '0'), dt.Hour.ToString().PadLeft(2, '0'), dt.Minute.ToString().PadLeft(2, '0'), dt.Second.ToString().PadLeft(2, '0'));
		}
	}
}
