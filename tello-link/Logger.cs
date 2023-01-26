using System.Runtime.InteropServices;
using System.Text;

namespace tello_link
{
	public class Logger
	{
		[DllImport("kernel32.dll")]
		private static extern bool AllocConsole();
		[DllImport("kernel32.dll")]
		private static extern bool FreeConsole();
		[DllImport("kernel32.dll")]
		private static extern IntPtr GetConsoleWindow();
		[DllImport("user32.dll")]
		static extern int ShowWindow(IntPtr handle, int command);
		[DllImport("user32.dll")]
		static extern bool IsWindowVisible(IntPtr handle);

		private static IntPtr hConsole = 0;

		public static void Setup()
		{
			AllocConsole();
			hConsole = GetConsoleWindow();
			Console.SetError(Console.Out);
			if (Program.log.Length > 0)
			{
				File.WriteAllText(Program.log, "", Encoding.UTF8);
			}
			if (Program.emulate > 0)
			{
				ShowWindow(hConsole, 1/*SW_SHOWNORMAL*/);
			}
			else
			{
				ShowWindow(hConsole, 0/*SW_HIDE*/);
			}
		}

		public static bool IsVisible()
		{
			return IsWindowVisible(hConsole);
		}

		public static void Visible(bool show)
		{
			if (show)
			{
				if ( ! IsVisible())
				{
					ShowWindow(hConsole, 1/*SW_SHOWNORMAL*/);
				}
			}
			else
			{
				if (IsVisible())
				{
					ShowWindow(hConsole, 0/*SW_HIDE*/);
				}
			}
		}

		public static void WriteLine(float value)
		{
			Console.WriteLine("" + value);
			if (Program.log.Length > 0)
			{
				lock (Program.log)
				{
					File.AppendAllText(Program.log, value + "\n", Encoding.UTF8);
				}
			}
		}
		public static void WriteLine(string str)
		{
			if (str == null || str.Length <= 0)
				return;
			Console.WriteLine(str);
			if (Program.log.Length > 0)
			{
				lock(Program.log)
				{
					File.AppendAllText(Program.log, str + "\n", Encoding.UTF8);
				}
			}
		}
		public static void Write(string str)
		{
			if (str == null || str.Length <= 0)
				return;
			Console.Write(str);
			if (Program.log.Length > 0)
			{
				lock (Program.log)
				{
					File.AppendAllText(Program.log, str, Encoding.UTF8);
				}
			}
		}
	}
}
