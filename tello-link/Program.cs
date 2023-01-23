using System.Configuration;

#pragma warning disable CS8618 // null 非許容のフィールドには、コンストラクターの終了時に null 以外の値が入っていなければなりません。Null 許容として宣言することをご検討ください。

namespace tello_link
{
	public class Program
	{
		public static int emulate = 0;
		public static UDPTello udpTello;
		public static TCPServer tcpServer;
		public static TCPStream tcpStream;
		public static Form form;
		public static NotifyIcon notifyIcon;

		public static void Main(string[] args)
		{
#if DEBUG
			emulate = 1;
#else
			emulate = Program.Setting("emulate", 0);
#endif
			Logger.Setup();
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			ApplicationIcon();
			udpTello = new UDPTello();
			tcpServer = new TCPServer();
			tcpStream = new TCPStream();
			tcpServer.Open();
			tcpStream.Open();
			udpTello.Open();
			Application.Run();
		}

		public static void Close()
		{
			udpTello.Close();
			tcpStream.Close();
			tcpServer.Close();
			notifyIcon.Visible = false;
			notifyIcon.Dispose();
			Application.Exit();
		}

		private static void ApplicationIcon() {
			form = new Form();
			form.ShowInTaskbar = false;
			notifyIcon = new NotifyIcon();
			notifyIcon.Icon = Resources.drone_icon_216206;
			notifyIcon.Visible = true;
			notifyIcon.Text = form.Text = Resources.TITLE;
			ContextMenuStrip menu = new ContextMenuStrip();
			ToolStripMenuItem item1 = new ToolStripMenuItem();
			menu.Items.Add(item1);
			item1.Text = Resources.CONSOLE;
			item1.Checked = Logger.IsVisible();
			item1.Click += (Object /*s*/, EventArgs /*e*/) =>
			{
				item1.Checked = !Logger.IsVisible();
				Logger.Visible(item1.Checked);
			};
			ToolStripMenuItem item2 = new ToolStripMenuItem();
			menu.Items.Add(item2);
			item2.Text = Resources.EXIT;
			item2.Click += (Object /*s*/, EventArgs /*e*/) => {
				Program.Close();
			};
			notifyIcon.ContextMenuStrip = menu;
		}

		public static void Connected(bool sw)
		{
			notifyIcon.Icon = (sw) ?
				Resources.drone_icon_216206G :
				Resources.drone_icon_216206;
		}

		public static int Setting(string key, int def)
		{
			int val = def;
			try
			{
				string? str = ConfigurationManager.AppSettings[key];
				if (str != null)
					val = Int32.Parse(str);
			}
			catch (Exception /*ex*/)
			{
				// NONE
			}
			return val;
		}
		public static string Setting(string key, string def)
		{
			string val = def;
			try
			{
				string? str = ConfigurationManager.AppSettings[key];
				if (str != null)
					val = str;
			}
			catch (Exception /*ex*/)
			{
				// NONE
			}
			return val;
		}
	}
}
