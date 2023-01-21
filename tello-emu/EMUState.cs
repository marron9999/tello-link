using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using static System.Windows.Forms.LinkLabel;



#pragma warning disable CS8601 // Null 参照代入の可能性があります。
#pragma warning disable CS8618 // null 非許容のフィールドには、コンストラクターの終了時に null 以外の値が入っていなければなりません。Null 許容として宣言することをご検討ください。
#pragma warning disable CS8604 // Null 参照引数の可能性があります。
#pragma warning disable CS8625 // null リテラルを null 非許容参照型に変換できません。
#pragma warning disable CS8600 // Null リテラルまたは Null の可能性がある値を Null 非許容型に変換しています。

namespace tello_link
{
	public class EMUState : EMURunner
	{
		private UdpClient state;
		private Dictionary<string, string> now = new Dictionary<string, string>();
		private int port;

		public EMUState()
		{
			state = new UdpClient();
			string[] keys = { "pitch", "roll", "yaw", "vgx", "vgy", "vgz",
				"templ", "temph", "tof", "h", "bat", "baro", "time",
				"agx", "agy", "agz" };
			foreach (string key in keys)
			{
				now[key] = "0";
			}
		}

		public override void Open()
		{
			base.Open();
			port = Program.Setting("state_port", 8890);
			now["baro"] = "" + 0f;
			now["agx"] = "" + 0f;
			now["agy"] = "" + 0f;
			now["agz"] = "" + 0f;
		}
		public override void Close()
		{
			state.Close();
			base.Close();
		}



		public override void Run()
		{
			log("EMUState", "connect 127.0.0.1:" + port);
			state.Connect("127.0.0.1", port);
			while (true)
			{
				if (_stop) break;
				string stat = "";
				foreach (string key in now.Keys)
				{
					stat += ";" + key + ":" + now[key];
				}
				// pitch:%d;roll:%d;yaw:%d;vgx:%d;vgy%d;vgz:%d;templ:%d;temph:%d;tof:%d;h:%d;bat:%d;baro:%.2f;time:%d;agx:%.2f;agy:%.2f;agz:%.2f;\r\n
				stat = stat.Substring(1);
				try
				{
					state.Send(Encoding.ASCII.GetBytes(stat));
				}
				catch (Exception /*ex*/)
				{
					// NONE;
				}
				Thread.Sleep(1000);
			}
			log("EMUState", "discpnnect 127.0.0.1:" + port);
		}
	}
}
