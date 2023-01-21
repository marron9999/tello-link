using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;

#pragma warning disable CS8601 // Null 参照代入の可能性があります。
#pragma warning disable CS8618 // null 非許容のフィールドには、コンストラクターの終了時に null 以外の値が入っていなければなりません。Null 許容として宣言することをご検討ください。
#pragma warning disable CS8604 // Null 参照引数の可能性があります。

namespace tello_link
{

	//Client
	public class UDPState : UDPBase
	{
		public UDPState() : base("UDPState")
		{
		}
		public override void Open() {
			int port = Program.Setting("state_port", 8890);
			base.Open(port);
		}
		public override async void Run()
		{
			log(name + ":" + Port, "open");
			while (true)
			{
				Received recv = await Receive();
				if (recv.Sender == null) break;
				string rc = state(recv.Message, true);
				if(rc.Length > 0
				&& Program.tcpServer != null)
				{
					Program.tcpServer.send(rc);
				}
			}
			log(name + ":" + Port, "close");
		}

		private Dictionary<string, string> status = new Dictionary<string, string>();
		private string state(string value, bool all = false)
		{
			// Data string received when the mission pad detection feature is enabled:
			// ata string received when the mission pad detection feature is enabled:requency is 10 Hz.l
			// theof:%d;h:%d;bat:%d;baro:%f;\r\n

			// Data string received when the mission pad detection feature is disabled:
			// pitch:%d;roll:%d;yaw:%d;vgx:%d;vgy%d;vgz:%d;templ:%d;temph:%d;tof:%d;h:%d;bat:%d;baro:%.2f; time:%d;agx:%.2f;agy:%.2f;agz:%.2f;\r\n

			// “mid” = the ID of the Mission Pad detected. If no Mission Pad is detected, a “-1” message will be
			// received instead.
			// “x” = the “x” coordinate detected on the Mission Pad. If there is no Mission Pad, a “0” message will
			// be received instead.
			// “y” = the “y” coordinate detected on the Mission Pad. If there is no Mission Pad, a “0” message will
			// be received instead.
			// “z” = the “z” coordinate detected on the Mission Pad. If there is no Mission Pad, a “0” message will
			// be received instead.
			// pitch = the degree of the attitude pitch.
			// roll = the degree of the attitude roll.
			// yaw = the degree of the attitude yaw.
			// vgx = the speed of “x” axis.
			// vgy = the speed of the “y” axis.
			// vgz = the speed of the “z” axis.
			// templ = the lowest temperature in degree Celsius.
			// temph = the highest temperature in degree Celsius
			// tof = the time of flight distance in cm.
			// h = the height in cm.
			// bat = the percentage of the current battery level.
			// baro = the barometer measurement in cm.
			// time = the amount of time the motor has been used.
			// agx = the acceleration of the “x” axis.
			// agy = the acceleration of the “y” axis.
			// agz = the acceleration of the “z” axis.
			value = value.Replace("\r", "").Replace("\n", "");
			string[] vs = value.Split(";");
			string u = "";
			foreach (string v in vs)
			{
				string[] t = v.Trim().Split(":");
				if (t.Length != 2) continue;
				if (all || ( ! status.ContainsKey(t[0])))
				{
					u += ";" + t[0] + ":" + t[1];
				}
				else
				{
					if (!status[t[0]].Equals(t[1]))
					{
						u += ";" + t[0] + ":" + t[1];
					}
				}
				status[t[0]] = t[1];
			}
			if (u.Length > 0)
			{
				return u.Substring(1);
			}
			return u;
		}
	}
}
