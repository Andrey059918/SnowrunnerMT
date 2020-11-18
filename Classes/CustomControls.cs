using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SnowrunnerMT {
	public class TabButton : Button {
		private Int32 activated = 0;
		public Int32 Activated {
			get => activated;
			set {
				activated = value;
				Refresh();
			}
		}
		private Color activatedColor = Color.White;
		public Color ActivatedColor {
			get => activatedColor;
			set {
				activatedColor = value;
				Refresh();
			}
		}

		protected override void OnPaint(PaintEventArgs pevent) {
			if (activated == 0) {
				pevent.Graphics.FillRectangle(new SolidBrush(BackColor), pevent.ClipRectangle);
			}
			else {
				pevent.Graphics.FillRectangle(new SolidBrush(activatedColor), pevent.ClipRectangle);
			}
			if (activated != 1) {
				pevent.Graphics.DrawLine(new Pen(ForeColor, 1), 0, 0, 0, pevent.ClipRectangle.Bottom - 1);
			}
			if (activated != 2) {
				pevent.Graphics.DrawLine(new Pen(ForeColor, 1), 0, 0, pevent.ClipRectangle.Right - 1, 0);
			}
			if (activated != 3) {
				pevent.Graphics.DrawLine(new Pen(ForeColor, 1), pevent.ClipRectangle.Right - 1, 0, pevent.ClipRectangle.Right - 1, pevent.ClipRectangle.Bottom - 1);
			}
			if (activated != 4) {
				pevent.Graphics.DrawLine(new Pen(ForeColor, 1), 0, pevent.ClipRectangle.Bottom - 1, pevent.ClipRectangle.Right - 1, pevent.ClipRectangle.Bottom - 1);
			}
			Size ssize = TextRenderer.MeasureText(Text, Font);
			pevent.Graphics.DrawString(Text, Font, new SolidBrush(ForeColor), (Size.Width - ssize.Width) / 2, (Size.Height - ssize.Height) / 2);
		}

	}
	public class TextPBar : ProgressBar {
		private string text = "";
		[BrowsableAttribute(true)]
		public override String Text {
			get => text;
			set {
				text = value;
				Refresh();
			}
		}
		private Font font = DefaultFont;
		[BrowsableAttribute(true)]
		public override Font Font {
			get => font;
			set {
				font = value;
				Refresh();
			}
		}

		private Color _progressColour1 = Color.FromArgb(0, 200, 80);
		[Category("Additional Options"), Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
		public Color ProgressColor1 {
			get {
				return _progressColour1;
			}
			set {
				_progressColour1 = value;
				Invalidate();
			}
		}
		private Color _progressColour2 = Color.Green;
		[Category("Additional Options"), Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
		public Color ProgressColor2 {
			get {
				return _progressColour2;
			}
			set {
				_progressColour2 = value;
				Invalidate();
			}
		}

		private ProgressBarDisplayMode _visualMode = ProgressBarDisplayMode.TextAndPercentage;
		[Category("Additional Options"), Browsable(true)]
		public ProgressBarDisplayMode VisualMode {
			get {
				return _visualMode;
			}
			set {
				_visualMode = value;
				Invalidate();
			}
		}


		public TextPBar() {
			SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
		}
		protected override void OnPaint(PaintEventArgs e) {
			Rectangle rect = ClientRectangle;
			Graphics g = e.Graphics;
			rect.Width -= 1;
			rect.Height -= 1;
			g.DrawRectangle(new Pen(ForeColor), rect);
			Int32 val = (int)Math.Round(((float)Value / Maximum) * (rect.Width - 1));

			rect.Width -= 1;
			rect.Height -= 1;
			rect.Offset(1, 1);
			Rectangle clip = rect;
			clip.Width = val;
			g.FillRectangle(new LinearGradientBrush(new Point(1, 1), new Point(1, Height - 1), _progressColour1, _progressColour2), clip);
			clip.Width = ClientRectangle.Width - val - 2;
			clip.X = val + 1;
			g.FillRectangle(new SolidBrush(BackColor), clip);

			String txt = "";
			if ((int)_visualMode % 2 == 1)
				txt = text;
			if ((int)_visualMode == 3 || (int)_visualMode == 5)
				txt += " ";
			if ((int)_visualMode / 2 == 1)
				txt += Value.ToString();
			else if ((int)_visualMode / 2 == 2)
				txt += (Value * 100f / Maximum).ToString("F2") + " %";


			SizeF len = g.MeasureString(txt, font);
			Point location = new Point(Convert.ToInt32((Width / 2) - len.Width / 2), Convert.ToInt32((Height / 2) - len.Height / 2));

			g.DrawString(txt, font, new SolidBrush(ForeColor), location);
		}
	}
	public enum ProgressBarDisplayMode {
		NoText,
		Text,
		Value,
		TextAndValue,
		Percentage,
		TextAndPercentage
	}

}
