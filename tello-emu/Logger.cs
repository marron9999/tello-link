using System.Runtime.InteropServices;

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
			ShowWindow(hConsole, 1);
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
		}
		public static void WriteLine(string str)
		{
			if (str == null || str.Length <= 0)
				return;
			Console.WriteLine(str);
		}
		public static void Write(string str)
		{
			if (str == null || str.Length <= 0)
				return;
			Console.Write(str);
		}
	}
}
