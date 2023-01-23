using System.Configuration;
using System.Windows.Forms;

namespace tello_link
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Logger.Setup();
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(true);
			EMUTello udp = new EMUTello();
			udp.Open();
			Application.Run();
		}

		public static int Setting(string key, int def)
		{
			int val = def;
			try
			{
				string? str = ConfigurationManager.AppSettings[key];
				if(str != null) 
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
