namespace SnowrunnerMT {
	partial class Form_SnowrunnerMT {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.Button_Apply = new System.Windows.Forms.Button();
			this.FlowPanel_CommonTweaks = new System.Windows.Forms.FlowLayoutPanel();
			this.RadioButton_Original = new System.Windows.Forms.RadioButton();
			this.RadioButton_Current = new System.Windows.Forms.RadioButton();
			this.Button_Restore = new System.Windows.Forms.Button();
			this.Button_GameDir = new System.Windows.Forms.Button();
			this.Button_Backup = new System.Windows.Forms.Button();
			this.RadioButton_Mods = new System.Windows.Forms.RadioButton();
			this.RadioButton_NoMods = new System.Windows.Forms.RadioButton();
			this.Button_ModsAfter = new System.Windows.Forms.Button();
			this.Button_ModsBefore = new System.Windows.Forms.Button();
			this.GroupBox_Tweaks = new System.Windows.Forms.GroupBox();
			this.GroupBox_Mods = new System.Windows.Forms.GroupBox();
			this.FlowPanel_SpecialTweaks = new System.Windows.Forms.FlowLayoutPanel();
			this.SB_Panel = new System.Windows.Forms.Panel();
			this.SB_RestoreButton = new System.Windows.Forms.Button();
			this.SB_BackupButton = new System.Windows.Forms.Button();
			this.SB_AddButton = new System.Windows.Forms.Button();
			this.SB_ProfileBox = new System.Windows.Forms.ComboBox();
			this.SB_PathButton = new System.Windows.Forms.Button();
			this.FlowPanel_SaveBackups = new System.Windows.Forms.FlowLayoutPanel();
			this.Panel_SaveBackups = new System.Windows.Forms.Panel();
			this.textPBar1 = new SnowrunnerMT.TextPBar();
			this.SB_TabButton = new SnowrunnerMT.TabButton();
			this.ST_TabButton = new SnowrunnerMT.TabButton();
			this.CT_TabButton = new SnowrunnerMT.TabButton();
			this.GroupBox_Tweaks.SuspendLayout();
			this.GroupBox_Mods.SuspendLayout();
			this.SB_Panel.SuspendLayout();
			this.Panel_SaveBackups.SuspendLayout();
			this.SuspendLayout();
			// 
			// Button_Apply
			// 
			this.Button_Apply.Location = new System.Drawing.Point(6, 65);
			this.Button_Apply.Name = "Button_Apply";
			this.Button_Apply.Size = new System.Drawing.Size(278, 26);
			this.Button_Apply.TabIndex = 1;
			this.Button_Apply.Text = "Apply tweaks";
			this.Button_Apply.UseVisualStyleBackColor = true;
			// 
			// FlowPanel_CommonTweaks
			// 
			this.FlowPanel_CommonTweaks.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.FlowPanel_CommonTweaks.AutoScroll = true;
			this.FlowPanel_CommonTweaks.BackColor = System.Drawing.Color.White;
			this.FlowPanel_CommonTweaks.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.FlowPanel_CommonTweaks.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
			this.FlowPanel_CommonTweaks.ForeColor = System.Drawing.SystemColors.ControlText;
			this.FlowPanel_CommonTweaks.Location = new System.Drawing.Point(305, 44);
			this.FlowPanel_CommonTweaks.Margin = new System.Windows.Forms.Padding(0);
			this.FlowPanel_CommonTweaks.Name = "FlowPanel_CommonTweaks";
			this.FlowPanel_CommonTweaks.Size = new System.Drawing.Size(754, 539);
			this.FlowPanel_CommonTweaks.TabIndex = 2;
			this.FlowPanel_CommonTweaks.WrapContents = false;
			// 
			// RadioButton_Original
			// 
			this.RadioButton_Original.AutoSize = true;
			this.RadioButton_Original.Checked = true;
			this.RadioButton_Original.Location = new System.Drawing.Point(6, 19);
			this.RadioButton_Original.Name = "RadioButton_Original";
			this.RadioButton_Original.Size = new System.Drawing.Size(120, 17);
			this.RadioButton_Original.TabIndex = 3;
			this.RadioButton_Original.TabStop = true;
			this.RadioButton_Original.Text = "Apply to original files";
			this.RadioButton_Original.UseVisualStyleBackColor = true;
			// 
			// RadioButton_Current
			// 
			this.RadioButton_Current.AutoSize = true;
			this.RadioButton_Current.Location = new System.Drawing.Point(6, 42);
			this.RadioButton_Current.Name = "RadioButton_Current";
			this.RadioButton_Current.Size = new System.Drawing.Size(120, 17);
			this.RadioButton_Current.TabIndex = 4;
			this.RadioButton_Current.Text = "Apply to current files";
			this.RadioButton_Current.UseVisualStyleBackColor = true;
			// 
			// Button_Restore
			// 
			this.Button_Restore.Location = new System.Drawing.Point(6, 129);
			this.Button_Restore.Name = "Button_Restore";
			this.Button_Restore.Size = new System.Drawing.Size(278, 26);
			this.Button_Restore.TabIndex = 5;
			this.Button_Restore.Text = "Restore original files";
			this.Button_Restore.UseVisualStyleBackColor = true;
			// 
			// Button_GameDir
			// 
			this.Button_GameDir.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.Button_GameDir.Location = new System.Drawing.Point(12, 12);
			this.Button_GameDir.Name = "Button_GameDir";
			this.Button_GameDir.Size = new System.Drawing.Size(1047, 26);
			this.Button_GameDir.TabIndex = 6;
			this.Button_GameDir.Text = "Select game directory";
			this.Button_GameDir.UseVisualStyleBackColor = true;
			// 
			// Button_Backup
			// 
			this.Button_Backup.Location = new System.Drawing.Point(6, 97);
			this.Button_Backup.Name = "Button_Backup";
			this.Button_Backup.Size = new System.Drawing.Size(278, 26);
			this.Button_Backup.TabIndex = 7;
			this.Button_Backup.Text = "Save current files as original";
			this.Button_Backup.UseVisualStyleBackColor = true;
			// 
			// RadioButton_Mods
			// 
			this.RadioButton_Mods.AutoSize = true;
			this.RadioButton_Mods.Location = new System.Drawing.Point(6, 42);
			this.RadioButton_Mods.Name = "RadioButton_Mods";
			this.RadioButton_Mods.Size = new System.Drawing.Size(80, 17);
			this.RadioButton_Mods.TabIndex = 9;
			this.RadioButton_Mods.TabStop = true;
			this.RadioButton_Mods.Text = "Install mods";
			this.RadioButton_Mods.UseVisualStyleBackColor = true;
			// 
			// RadioButton_NoMods
			// 
			this.RadioButton_NoMods.AutoSize = true;
			this.RadioButton_NoMods.Checked = true;
			this.RadioButton_NoMods.Location = new System.Drawing.Point(6, 19);
			this.RadioButton_NoMods.Name = "RadioButton_NoMods";
			this.RadioButton_NoMods.Size = new System.Drawing.Size(107, 17);
			this.RadioButton_NoMods.TabIndex = 8;
			this.RadioButton_NoMods.TabStop = true;
			this.RadioButton_NoMods.Text = "Don\'t install mods";
			this.RadioButton_NoMods.UseVisualStyleBackColor = true;
			// 
			// Button_ModsAfter
			// 
			this.Button_ModsAfter.Location = new System.Drawing.Point(6, 97);
			this.Button_ModsAfter.Name = "Button_ModsAfter";
			this.Button_ModsAfter.Size = new System.Drawing.Size(278, 26);
			this.Button_ModsAfter.TabIndex = 11;
			this.Button_ModsAfter.Text = "Mods after tweaks (open folder)";
			this.Button_ModsAfter.UseVisualStyleBackColor = true;
			// 
			// Button_ModsBefore
			// 
			this.Button_ModsBefore.Location = new System.Drawing.Point(6, 65);
			this.Button_ModsBefore.Name = "Button_ModsBefore";
			this.Button_ModsBefore.Size = new System.Drawing.Size(278, 26);
			this.Button_ModsBefore.TabIndex = 10;
			this.Button_ModsBefore.Text = "Mods before tweaks (open folder)";
			this.Button_ModsBefore.UseVisualStyleBackColor = true;
			// 
			// GroupBox_Tweaks
			// 
			this.GroupBox_Tweaks.BackColor = System.Drawing.SystemColors.Control;
			this.GroupBox_Tweaks.Controls.Add(this.RadioButton_Original);
			this.GroupBox_Tweaks.Controls.Add(this.Button_Apply);
			this.GroupBox_Tweaks.Controls.Add(this.RadioButton_Current);
			this.GroupBox_Tweaks.Controls.Add(this.Button_Restore);
			this.GroupBox_Tweaks.Controls.Add(this.Button_Backup);
			this.GroupBox_Tweaks.Location = new System.Drawing.Point(12, 251);
			this.GroupBox_Tweaks.Name = "GroupBox_Tweaks";
			this.GroupBox_Tweaks.Size = new System.Drawing.Size(290, 161);
			this.GroupBox_Tweaks.TabIndex = 12;
			this.GroupBox_Tweaks.TabStop = false;
			this.GroupBox_Tweaks.Text = "Files";
			// 
			// GroupBox_Mods
			// 
			this.GroupBox_Mods.Controls.Add(this.RadioButton_NoMods);
			this.GroupBox_Mods.Controls.Add(this.Button_ModsAfter);
			this.GroupBox_Mods.Controls.Add(this.RadioButton_Mods);
			this.GroupBox_Mods.Controls.Add(this.Button_ModsBefore);
			this.GroupBox_Mods.Location = new System.Drawing.Point(12, 418);
			this.GroupBox_Mods.Name = "GroupBox_Mods";
			this.GroupBox_Mods.Size = new System.Drawing.Size(290, 129);
			this.GroupBox_Mods.TabIndex = 13;
			this.GroupBox_Mods.TabStop = false;
			this.GroupBox_Mods.Text = "Mods";
			// 
			// FlowPanel_SpecialTweaks
			// 
			this.FlowPanel_SpecialTweaks.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.FlowPanel_SpecialTweaks.AutoScroll = true;
			this.FlowPanel_SpecialTweaks.BackColor = System.Drawing.Color.White;
			this.FlowPanel_SpecialTweaks.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.FlowPanel_SpecialTweaks.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
			this.FlowPanel_SpecialTweaks.Location = new System.Drawing.Point(305, 44);
			this.FlowPanel_SpecialTweaks.Margin = new System.Windows.Forms.Padding(0);
			this.FlowPanel_SpecialTweaks.Name = "FlowPanel_SpecialTweaks";
			this.FlowPanel_SpecialTweaks.Size = new System.Drawing.Size(754, 539);
			this.FlowPanel_SpecialTweaks.TabIndex = 16;
			this.FlowPanel_SpecialTweaks.WrapContents = false;
			// 
			// SB_Panel
			// 
			this.SB_Panel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
			this.SB_Panel.Controls.Add(this.SB_RestoreButton);
			this.SB_Panel.Controls.Add(this.SB_BackupButton);
			this.SB_Panel.Controls.Add(this.SB_AddButton);
			this.SB_Panel.Controls.Add(this.SB_ProfileBox);
			this.SB_Panel.Controls.Add(this.SB_PathButton);
			this.SB_Panel.Location = new System.Drawing.Point(5, 5);
			this.SB_Panel.Margin = new System.Windows.Forms.Padding(5);
			this.SB_Panel.Name = "SB_Panel";
			this.SB_Panel.Size = new System.Drawing.Size(742, 96);
			this.SB_Panel.TabIndex = 0;
			// 
			// SB_RestoreButton
			// 
			this.SB_RestoreButton.BackColor = System.Drawing.Color.White;
			this.SB_RestoreButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.SB_RestoreButton.Location = new System.Drawing.Point(195, 62);
			this.SB_RestoreButton.Margin = new System.Windows.Forms.Padding(5, 5, 5, 0);
			this.SB_RestoreButton.Name = "SB_RestoreButton";
			this.SB_RestoreButton.Size = new System.Drawing.Size(180, 23);
			this.SB_RestoreButton.TabIndex = 17;
			this.SB_RestoreButton.Text = "Restore hotkey";
			this.SB_RestoreButton.UseVisualStyleBackColor = false;
			// 
			// SB_BackupButton
			// 
			this.SB_BackupButton.BackColor = System.Drawing.Color.White;
			this.SB_BackupButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.SB_BackupButton.Location = new System.Drawing.Point(5, 62);
			this.SB_BackupButton.Margin = new System.Windows.Forms.Padding(5, 5, 5, 0);
			this.SB_BackupButton.Name = "SB_BackupButton";
			this.SB_BackupButton.Size = new System.Drawing.Size(180, 23);
			this.SB_BackupButton.TabIndex = 16;
			this.SB_BackupButton.Text = "Backup hotkey";
			this.SB_BackupButton.UseVisualStyleBackColor = false;
			// 
			// SB_AddButton
			// 
			this.SB_AddButton.BackColor = System.Drawing.Color.White;
			this.SB_AddButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.SB_AddButton.Location = new System.Drawing.Point(385, 62);
			this.SB_AddButton.Margin = new System.Windows.Forms.Padding(5);
			this.SB_AddButton.Name = "SB_AddButton";
			this.SB_AddButton.Size = new System.Drawing.Size(352, 23);
			this.SB_AddButton.TabIndex = 15;
			this.SB_AddButton.Text = "Add backup slot";
			this.SB_AddButton.UseVisualStyleBackColor = false;
			// 
			// SB_ProfileBox
			// 
			this.SB_ProfileBox.BackColor = System.Drawing.SystemColors.ButtonHighlight;
			this.SB_ProfileBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.SB_ProfileBox.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.SB_ProfileBox.FormattingEnabled = true;
			this.SB_ProfileBox.Location = new System.Drawing.Point(5, 5);
			this.SB_ProfileBox.Margin = new System.Windows.Forms.Padding(5, 5, 5, 0);
			this.SB_ProfileBox.Name = "SB_ProfileBox";
			this.SB_ProfileBox.Size = new System.Drawing.Size(732, 24);
			this.SB_ProfileBox.TabIndex = 14;
			// 
			// SB_PathButton
			// 
			this.SB_PathButton.BackColor = System.Drawing.Color.White;
			this.SB_PathButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.SB_PathButton.Location = new System.Drawing.Point(5, 34);
			this.SB_PathButton.Margin = new System.Windows.Forms.Padding(5, 5, 5, 0);
			this.SB_PathButton.Name = "SB_PathButton";
			this.SB_PathButton.Size = new System.Drawing.Size(732, 23);
			this.SB_PathButton.TabIndex = 6;
			this.SB_PathButton.Text = "Select path to backup";
			this.SB_PathButton.UseVisualStyleBackColor = false;
			// 
			// FlowPanel_SaveBackups
			// 
			this.FlowPanel_SaveBackups.AutoScroll = true;
			this.FlowPanel_SaveBackups.BackColor = System.Drawing.SystemColors.ButtonHighlight;
			this.FlowPanel_SaveBackups.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
			this.FlowPanel_SaveBackups.Location = new System.Drawing.Point(5, 111);
			this.FlowPanel_SaveBackups.Margin = new System.Windows.Forms.Padding(5);
			this.FlowPanel_SaveBackups.Name = "FlowPanel_SaveBackups";
			this.FlowPanel_SaveBackups.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
			this.FlowPanel_SaveBackups.Size = new System.Drawing.Size(742, 421);
			this.FlowPanel_SaveBackups.TabIndex = 22;
			this.FlowPanel_SaveBackups.WrapContents = false;
			// 
			// Panel_SaveBackups
			// 
			this.Panel_SaveBackups.BackColor = System.Drawing.SystemColors.ButtonHighlight;
			this.Panel_SaveBackups.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.Panel_SaveBackups.Controls.Add(this.SB_Panel);
			this.Panel_SaveBackups.Controls.Add(this.FlowPanel_SaveBackups);
			this.Panel_SaveBackups.Location = new System.Drawing.Point(305, 44);
			this.Panel_SaveBackups.Margin = new System.Windows.Forms.Padding(5);
			this.Panel_SaveBackups.Name = "Panel_SaveBackups";
			this.Panel_SaveBackups.Size = new System.Drawing.Size(754, 539);
			this.Panel_SaveBackups.TabIndex = 23;
			// 
			// textPBar1
			// 
			this.textPBar1.BackColor = System.Drawing.SystemColors.ButtonHighlight;
			this.textPBar1.ForeColor = System.Drawing.Color.Black;
			this.textPBar1.Location = new System.Drawing.Point(12, 554);
			this.textPBar1.Name = "textPBar1";
			this.textPBar1.ProgressColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
			this.textPBar1.ProgressColor2 = System.Drawing.Color.Green;
			this.textPBar1.Size = new System.Drawing.Size(290, 29);
			this.textPBar1.TabIndex = 21;
			this.textPBar1.VisualMode = SnowrunnerMT.ProgressBarDisplayMode.NoText;
			// 
			// SB_TabButton
			// 
			this.SB_TabButton.Activated = 0;
			this.SB_TabButton.ActivatedColor = System.Drawing.Color.White;
			this.SB_TabButton.Location = new System.Drawing.Point(12, 116);
			this.SB_TabButton.Name = "SB_TabButton";
			this.SB_TabButton.Size = new System.Drawing.Size(294, 30);
			this.SB_TabButton.TabIndex = 20;
			this.SB_TabButton.Text = "Save backups";
			this.SB_TabButton.UseVisualStyleBackColor = true;
			// 
			// ST_TabButton
			// 
			this.ST_TabButton.Activated = 0;
			this.ST_TabButton.ActivatedColor = System.Drawing.Color.White;
			this.ST_TabButton.Location = new System.Drawing.Point(12, 80);
			this.ST_TabButton.Name = "ST_TabButton";
			this.ST_TabButton.Size = new System.Drawing.Size(294, 30);
			this.ST_TabButton.TabIndex = 19;
			this.ST_TabButton.Text = "Special tweaks";
			this.ST_TabButton.UseVisualStyleBackColor = true;
			// 
			// CT_TabButton
			// 
			this.CT_TabButton.Activated = 3;
			this.CT_TabButton.ActivatedColor = System.Drawing.Color.White;
			this.CT_TabButton.Location = new System.Drawing.Point(12, 44);
			this.CT_TabButton.Name = "CT_TabButton";
			this.CT_TabButton.Size = new System.Drawing.Size(294, 30);
			this.CT_TabButton.TabIndex = 18;
			this.CT_TabButton.Text = "Common tweaks";
			this.CT_TabButton.UseVisualStyleBackColor = true;
			// 
			// Form_SnowrunnerMT
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1068, 595);
			this.Controls.Add(this.textPBar1);
			this.Controls.Add(this.SB_TabButton);
			this.Controls.Add(this.ST_TabButton);
			this.Controls.Add(this.CT_TabButton);
			this.Controls.Add(this.GroupBox_Mods);
			this.Controls.Add(this.GroupBox_Tweaks);
			this.Controls.Add(this.Button_GameDir);
			this.Controls.Add(this.FlowPanel_CommonTweaks);
			this.Controls.Add(this.FlowPanel_SpecialTweaks);
			this.Controls.Add(this.Panel_SaveBackups);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.KeyPreview = true;
			this.MaximumSize = new System.Drawing.Size(1104, 634);
			this.Name = "Form_SnowrunnerMT";
			this.Text = "SnowrunnerMT";
			this.GroupBox_Tweaks.ResumeLayout(false);
			this.GroupBox_Tweaks.PerformLayout();
			this.GroupBox_Mods.ResumeLayout(false);
			this.GroupBox_Mods.PerformLayout();
			this.SB_Panel.ResumeLayout(false);
			this.Panel_SaveBackups.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.Button Button_Apply;
		private System.Windows.Forms.FlowLayoutPanel FlowPanel_CommonTweaks;
		private System.Windows.Forms.RadioButton RadioButton_Original;
		private System.Windows.Forms.RadioButton RadioButton_Current;
		private System.Windows.Forms.Button Button_Restore;
		private System.Windows.Forms.Button Button_GameDir;
		private System.Windows.Forms.Button Button_Backup;
		private System.Windows.Forms.RadioButton RadioButton_Mods;
		private System.Windows.Forms.RadioButton RadioButton_NoMods;
		private System.Windows.Forms.Button Button_ModsAfter;
		private System.Windows.Forms.Button Button_ModsBefore;
		private System.Windows.Forms.GroupBox GroupBox_Tweaks;
		private System.Windows.Forms.GroupBox GroupBox_Mods;
		private System.Windows.Forms.FlowLayoutPanel FlowPanel_SpecialTweaks;
		private TabButton CT_TabButton;
		private TabButton ST_TabButton;
		private TabButton SB_TabButton;
		private TextPBar textPBar1;
		private System.Windows.Forms.Panel SB_Panel;
		private System.Windows.Forms.Button SB_PathButton;
		private System.Windows.Forms.Button SB_AddButton;
		private System.Windows.Forms.ComboBox SB_ProfileBox;
		private System.Windows.Forms.FlowLayoutPanel FlowPanel_SaveBackups;
		private System.Windows.Forms.Button SB_RestoreButton;
		private System.Windows.Forms.Button SB_BackupButton;
		private System.Windows.Forms.Panel Panel_SaveBackups;
	}
}