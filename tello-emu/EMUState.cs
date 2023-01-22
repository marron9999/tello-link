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
		public EMUTello tello;
		private UdpClient state;
		private Dictionary<string, string> now = new Dictionary<string, string>();
		private int port;
		public int bat = 0;
		public int tof = 0;
		public bool fly;
		public long fly_time = 0;
		public long fly_prev = 0;
		public DateTime take_time = DateTime.Now;
		public DateTime reset_time = DateTime.Now;
		public void reset()
		{
			reset_time = DateTime.Now;
			tof = 0;
			bat = 100;
			fly = false;
			fly_time = 0;
			fly_prev = 0;
			now["bat"] = "100";
			now["time"] = "0";
		}

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
				DateTime dtn = DateTime.Now;
				if (_stop) break;
				TimeSpan s = dtn - reset_time;
				if(fly) 
				{
					s = dtn - take_time;
					fly_time = fly_prev + (long) s.TotalSeconds;
					now["time"] = "" + fly_time;
				}
				long ts = (long)s.TotalSeconds / 30;
				ts += fly_time / 8;
				bat = 100 - (int)ts;
				if (bat < 0) bat = 0;
				now["bat"] = "" + bat;
				if(bat < 10)
				{
					tello.land();
				}
				now["tof"] = "" + tof;
				now["h"] = "" + tof;
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
				Thread.Sleep(500);
			}
			log("EMUState", "discpnnect 127.0.0.1:" + port);
		}
	}
}
