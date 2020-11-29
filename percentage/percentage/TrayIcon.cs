using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace percentage
{
    class TrayIcon
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern bool DestroyIcon(IntPtr handle);

		private const string iconFont = "Verdana";
		private int iconFontSize;
		Color textColor;
		private int batteryPercentage;
		private bool charging;
        private NotifyIcon notifyIcon;
		

        public TrayIcon()
        {
            ContextMenu contextMenu = new ContextMenu();
            MenuItem menuItem = new MenuItem();

            notifyIcon = new NotifyIcon();

            // initialize contextMenu
            contextMenu.MenuItems.AddRange(new MenuItem[] { menuItem });

            // initialize menuItem
            menuItem.Index = 0;
            menuItem.Text = "E&xit";
            menuItem.Click += new System.EventHandler(MenuItem_Click);

            notifyIcon.ContextMenu = contextMenu;

            batteryPercentage = 0;
			charging = false;

			iconFontSize = 22;
			textColor = Color.White;

            notifyIcon.Visible = true;

			iconUpdate();

            Timer timer = new Timer();
            timer.Tick += new EventHandler(Timer_Tick);
            timer.Interval = 5000; // in miliseconds
			// timer.Enabled = true;
			timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
			iconUpdate();
		}

		private void iconUpdate()
		{
			batteryPercentage = (int)(SystemInformation.PowerStatus.BatteryLifePercent * 100);
			charging = SystemInformation.PowerStatus.BatteryChargeStatus.HasFlag(BatteryChargeStatus.Charging);
	
			if (batteryPercentage == 100)
				iconFontSize = 18;
			else
				iconFontSize = 22;


			if (batteryPercentage <= 15 && !charging)
				textColor = Color.Red;
			else
				textColor = Color.White;


			using (Bitmap bitmap = new Bitmap(DrawText(batteryPercentage.ToString(), new Font(iconFont, iconFontSize, FontStyle.Regular, GraphicsUnit.Pixel), textColor, Color.Transparent)))
			{
				System.IntPtr intPtr = bitmap.GetHicon();
				try
				{
					using (Icon icon = Icon.FromHandle(intPtr))
					{
						notifyIcon.Icon = icon;
					}
				}
				finally
				{
					DestroyIcon(intPtr);
				}
			}

		}

		private void MenuItem_Click(object sender, EventArgs e)
        {
            notifyIcon.Visible = false;
            notifyIcon.Dispose();
            Application.Exit();
        }


		private Image DrawText(String text, Font font, Color textColor, Color backColor)
		{


			Image bitmapImage = new Bitmap(32, 32);
			Rectangle rect = new Rectangle(-6, 2, 42, 32);
			using (Graphics g = Graphics.FromImage(bitmapImage))
            {
				g.Clear(backColor);
				using (SolidBrush brush = new SolidBrush(backColor))
				{
					g.FillRectangle(brush, 0, 0, 32, 32);
				}
				using (Brush textBrush = new SolidBrush(textColor))
				{
					g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
					using (StringFormat sf = new StringFormat())
                    {
						sf.Alignment = StringAlignment.Center;
						sf.LineAlignment = StringAlignment.Center;

						g.DrawString(text, font, textBrush, rect, sf);
						g.Save();
					}
				}

			}

			return bitmapImage;
		}

	}
}
