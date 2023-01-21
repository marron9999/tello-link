#pragma warning disable CS8601 // Null 参照代入の可能性があります。
#pragma warning disable CS8618 // null 非許容のフィールドには、コンストラクターの終了時に null 以外の値が入っていなければなりません。Null 許容として宣言することをご検討ください。
#pragma warning disable CS1998 // 非同期メソッドは、'await' 演算子がないため、同期的に実行されます

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
