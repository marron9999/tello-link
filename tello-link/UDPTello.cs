using System.DirectoryServices;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Linq;

#pragma warning disable CS8601 // Null 参照代入の可能性があります。
#pragma warning disable CS8618 // null 非許容のフィールドには、コンストラクターの終了時に null 以外の値が入っていなければなりません。Null 許容として宣言することをご検討ください。
#pragma warning disable CS8604 // Null 参照引数の可能性があります。
#pragma warning disable CS8625 // null リテラルを null 非許容参照型に変換できません。
#pragma warning disable CS8600 // Null リテラルまたは Null の可能性がある値を Null 非許容型に変換しています。

namespace tello_link
{
	public class UDPTello : UDPBase
	{
		private UDPState state;
		private UDPStream stream;
		private UDPTelloSDK tello_sdk;
		private int tello_port;
		private string tello_addr;

		public UDPTello() : base("UDPTello")
		{
			state = new UDPState();
			stream = new UDPStream();
			tello_sdk = new UDPTelloSDK();
			tello_sdk.tello = this;
		}

		public override void Open()
		{
			int port = Program.Setting("link_port", 8889);
			string addr = "0.0.0.0";
			if (Program.emulate > 0)
			{
				tello_port = Program.Setting("emu_port", 8888);
				tello_addr = "127.0.0.1";
			}
			else {
				tello_port = Program.Setting("tello_port", 8889);
				tello_addr = Program.Setting("tello_addr", "192.168.10.1");
				IPAddress[] hips = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
				for (int i = 0; i < hips.Length; i++)
				{
					string taddr = hips[i].ToString();
					if (taddr.StartsWith("192.168.10."))
					{
						addr = taddr;
						break;
					}
				}
			}
			base.Open(addr, port);
			state.Open();
			stream.Open();
			tello_sdk.Open();
		}
		public override void Close()
		{
			tello_sdk.Close();
			stream.Close();
			state.Close();
			base.Close();
		}

		public override async void Run()
		{
			log(fullname(), "open");
			while (true)
			{
				Received recv = await Receive();
				if (recv.Sender == null) break;
				if(_logging)
				{
					log(name + ": " + recv.Sender, recv.Message);
				}
				_result = recv.Message;
			}
			log(fullname(), "close");
		}

		public bool mode_sdk = false;
		public bool mode_fly = false;
		public bool mode_eye = false;
		private Object _lock = new Object();
		private string _result = null;
		private bool _logging = true;
		public string result(string cmd, bool log)
		{
			return result(cmd, 5, log);
		}

		public string result(string cmd, int timeout = 5, bool logging = true)
		{
			lock (_lock)
			{
				DateTime _timeout = DateTime.Now;
				_timeout = _timeout.AddSeconds(timeout);
				_logging = logging;
				_result = null;
				try
				{
					Task<string> task = new Task<string>(() => {
						string __result = null;
						if(_logging)
						{
							Logger.WriteLine(name + ": " + cmd);
						}
						byte[] buf = Encoding.ASCII.GetBytes(cmd);
						client.SendAsync(buf, buf.Length, tello_addr, tello_port);
						while (__result == null)
						{
							Thread.Sleep(100);
							__result = _result;
						}
						if (_timeout < DateTime.Now)
						{
							return "error (timeout)";
						}
						return __result;
					});
					task.Start();
					task.Wait();
					if (_logging)
					{
						Logger.WriteLine(name + ": " + cmd + " " + task.Result);
					}
					return task.Result;
				}
				catch (Exception ex)
				{
					return "error (" + ex.ToString() + ")";
				}
			}
		}

		private bool validate(string val, int low, int high)
		{
			try
			{
				int ival = Int32.Parse(val);
				return validate(ival, low, high);
			}
			catch (Exception /*ex*/)
			{
				//
			}
			return false;
		}
		private bool validate(int val, int low, int high)
		{
			if (val < low) return false;
			if (val > high) return false;
			return true;
		}
		private bool validate(string val, int low, int high, int elow, int ehigh)
		{
			try
			{
				int ival = Int32.Parse(val);
				return validate(ival, low, high, elow, ehigh);
			}
			catch (Exception /*ex*/)
			{
				//
			}
			return false;
		}
		private bool validate(int val, int low, int high, int elow, int ehigh)
		{
			if (val < low) return false;
			if (val > high) return false;
			if (val >= elow && val <= ehigh) return false;
			return true;
		}

		public string parse(string cmd)
		{
			string rc = "error (unknown)";
			try
			{
				if (cmd[0] == '!')
				{
					rc = result(cmd.Substring(1).Trim());
					return cmd + " " + rc;
				}
				string[] c = cmd.Split(" ");
				c[0] = c[0].Replace("?", "_");
				Type t = this.GetType();
				MethodInfo mi = t.GetMethod(c[0]);
				if(mi != null) {
					if (c.Length <= 1)
					{
						rc = "" + mi.Invoke(this, null);
					}
					else
					{
						List<string> args = new List<string>();
						for (int i = 1; i < c.Length; i++) args.Add(c[i]);
						rc = "" + mi.Invoke(this, args.ToArray());
					}
					if (rc.Length == 0)
					{
						rc = result(cmd);
					}
					return cmd + " " + rc;
				}
			}
			catch (Exception ex)
			{
				rc = "error (" + ex.ToString() + ")";
			}
			return cmd + " " + rc;
		}

		public string connect()
		{
			if (!mode_sdk) return "connect error (nosdk)";
			if(Program.emulate > 0)
			{
				result("@reset", false);
			}
			return "ok";
		}
		public string disconnect()
		{
			if (!mode_sdk) return "disconnect error (nosdk)";
			if(mode_eye)
			{
				result("streamoff");
			}
			return "ok";
		}

		// ------------------
		// Control Commands
		// ------------------
		public string Command()
		{
			// Enter SDK mode.
			// ok / error

			string rc = result("command", false);
			if (rc.IndexOf("ok") >= 0)
				mode_sdk = true;
			return rc;
		}
		public string takeoff()
		{
			// Auto takeoff.
			if (!mode_sdk) return "error (nosdk)";
			if (mode_fly) return "ok";
			string rc = result("takeoff", 10);
			if(rc.IndexOf("ok") >= 0)
				mode_fly = true;
			return rc;
		}
		public string land()
		{
			//Auto landing.
			if (!mode_sdk) return "error (nosdk)";
			//if (!mode_fly) return "ok";
			mode_fly = false;
			string rc = result("land", 10);
			return rc;
		}
		public string streamon()
		{
			// Enable video stream.
			if (!mode_sdk) return "error (nosdk)";
			string rc = result("streamon");
			if (rc.IndexOf("ok") >= 0)
			{
				mode_eye = true;
			}
			return rc;
		}
		public string streamoff()
		{
			// Disable video stream.
			if (!mode_sdk) return "error (nosdk)";
			string rc = result("streamoff");
			if (rc.IndexOf("ok") >= 0)
			{
				mode_eye = false;
			}
			return rc;
		}
		public string emergency()
		{
			// Stop motors immediately.
			if (!mode_sdk) return "error (nosdk)";
			mode_sdk = false;
			mode_fly = false;
			return "";
		}
		public string up(string x)
		{
			// Ascend to “x” cm.
			// x = 20 - 500
			if (!validate(x, 20, 500)) return "error (x)";
			if (!mode_sdk) return "error (nosdk)";
			if (!mode_fly) return "error (nofly)";
			return "";
		}
		public string down(string x)
		{
			// down “x” Descend to “x” cm.
			// x = 20 - 500
			if (!validate(x, 20, 500)) return "error (arg)";
			if (!mode_sdk) return "error (nosdk)";
			if (!mode_fly) return "error (nofly)";
			return "";
		}
		public string left(string x)
		{
			// Fly left for “x” cm.
			// “x” = 20-500
			if (!validate(x, 20, 500)) return "error (arg)";
			if (!mode_sdk) return "error (nosdk)";
			if (!mode_fly) return "error (nofly)";
			return "";
		}
		public string right(string x)
		{
			// Fly right for “x” cm.
			// “x” = 20-500
			if (!validate(x, 20, 500)) return "error (arg)";
			if (!mode_sdk) return "error (nosdk)";
			if (!mode_fly) return "error (nofly)";
			return "";
		}
		public string forward(string x)
		{
			// Fly forward for “x” cm.
			// “x” = 20-500
			if (!validate(x, 20, 500)) return "error (arg)";
			if (!mode_sdk) return "error (nosdk)";
			if (!mode_fly) return "error (nofly)";
			return "";
		}
		public string back(string x)
		{
			// Fly backward for “x” cm.
			// “x” = 20-500
			if (!validate(x, 20, 500)) return "error (arg)";
			if (!mode_sdk) return "error (nosdk)";
			if (!mode_fly) return "error (nofly)";
			return "";
		}
		public string cw(string x)
		{
			// Rotate “x” degrees clockwise.
			// “x” = 1-360
			if (!validate(x, 1, 360)) return "error (arg)";
			if (!mode_sdk) return "error (nosdk)";
			if (!mode_fly) return "error (nofly)";
			return "";
		}
		public string ccw(string x)
		{
			// Rotate “x” degrees counterclockwise.
			// “x” = 1-360
			if (!validate(x, 1, 360)) return "error (arg)";
			if (!mode_sdk) return "error (nosdk)";
			if (!mode_fly) return "error (nofly)";
			return "";
		}
		public string flip(string x)
		{
			// Flip in “x” direction.
			// “l” = left
			// “r” = right
			// “f” = forward
			// “b” = back
			if (!mode_sdk) return "error (nosdk)";
			if (!mode_fly) return "error (nofly)";
			return "";
		}
		public string go(string x, string y, string z, string speed)
		{
			// Fly to “x” “y” “z” at “speed” (cm/s).
			// “x” = -500-500
			// “y” = -500-500
			// “z” = -500-500
			// “speed” = 10-100
			// Note: “x”, “y”, and “z” values can’t be set between
			// -20 – 20 simultaneously.
			if (!validate(x, -500, 500, -20, 20)) return "error (x)";
			if (!validate(y, -500, 500, -20, 20)) return "error (y)";
			if (!validate(z, -500, 500, -20, 20)) return "error (z)";
			if (!validate(speed, 10, 100)) return "error (speed)";
			if (!mode_sdk) return "error (nosdk)";
			if (!mode_fly) return "error (nofly)";
			return "";
		}
		public string stop()
		{
			// Hovers in the air.
			// Note: works at any time.
			if (!mode_sdk) return "error (nosdk)";
			if (!mode_fly) return "error (nofly)";
			return "";
		}
		public string curve(string x1, string y1, string z1, string x2, string y2, string z2, string speed)
		{
			// Fly at a curve according to the two given coordinates
			// at “speed” (cm/s).
			// If the arc radius is not within a range of 0.5-10 meters,
			// it will respond with an error.
			// “x1”, “x2” = -500-500
			// “y1”, “y2” = -500-500
			// “z1”, “z2” = -500-500
			// “speed” = 10-60
			// Note: “x”, “y”, and “z” values can’t be set between
			// -20 – 20 simultaneously.
			if (!validate(x1, -500, 500, -20, 20)) return "error (x1)";
			if (!validate(y1, -500, 500, -20, 20)) return "error (y1)";
			if (!validate(z1, -500, 500, -20, 20)) return "error (z1)";
			if (!validate(x2, -500, 500, -20, 20)) return "error (x2)";
			if (!validate(y2, -500, 500, -20, 20)) return "error (y2)";
			if (!validate(z2, -500, 500, -20, 20)) return "error (z2)";
			if (!validate(speed, 10, 60)) return "error (speed)";
			if (!mode_sdk) return "error (nosdk)";
			if (!mode_fly) return "error (nofly)";
			return "";
		}
		public string go(string x, string y, string z, string speed, string mid)
		{
			// Fly to the “x”, “y”, and “z” coordinates of the Mission
			// Pad.
			// “mid” = m1-m8
			// “x” = -500-500
			// “y” = -500-500
			// “z” = -500-500
			// “speed” = 10-100 (cm/s)
			// Note: “x”, “y”, and “z” values can’t be set between
			// -20 – 20 simultaneously.
			if (!validate(x, -500, 500, -20, 20)) return "error (x)";
			if (!validate(y, -500, 500, -20, 20)) return "error (y)";
			if (!validate(z, -500, 500, -20, 20)) return "error (z)";
			if (!validate(speed, 10, 100)) return "error (z)";
			if (!mode_sdk) return "error (nosdk)";
			if (!mode_fly) return "error (nofly)";
			return "";
		}
		public string curve(string x1, string y1, string z1, string x2, string y2, string z2, string speed, string mid)
		{
			// Fly at a curve according to the two given coordinates
			// of the Mission Pad ID at “speed” (cm/s).
			// If the arc radius is not within a range of 0.5-10 meters,
			// it will respond with an error.
			// “x1”, “x2” = -500-500
			// “y1”, “y2” = -500-500
			// “z1”, “z2” = -500-500
			// “speed” = 10-60
			// Note: “x”, “y”, and “z” values can’t be set between
			// -20 – 20 simultaneously.
			if (!validate(x1, -500, 500, -20, 20)) return "error (x1)";
			if (!validate(y1, -500, 500, -20, 20)) return "error (y1)";
			if (!validate(z1, -500, 500, -20, 20)) return "error (z1)";
			if (!validate(x2, -500, 500, -20, 20)) return "error (x2)";
			if (!validate(y2, -500, 500, -20, 20)) return "error (y2)";
			if (!validate(z2, -500, 500, -20, 20)) return "error (z2)";
			if (!validate(speed, 10, 60)) return "error (speed)";
			if (!mode_sdk) return "error (nosdk)";
			if (!mode_fly) return "error (nofly)";
			return "";
		}
		public string jump(string x, string y, string z, string speed, string yaw, string mid1, string mid2)
		{
			// Fly to coordinates “x”, “y”, and “z” of Mission Pad 1,
			// and recognize coordinates 0, 0, “z” of Mission Pad 2
			// and rotate to the yaw value.
			// “mid” = m1-m8
			// “x” = -500-500
			// “y” = -500-500
			// “z” = -500-500
			// “speed” = 10-100 (cm/s)
			// Note: “x”, “y”, and “z” values can’t be set between
			// -20 – 20 simultaneously.
			if (!validate(x, -500, 500, -20, 20)) return "error (x)";
			if (!validate(y, -500, 500, -20, 20)) return "error (y)";
			if (!validate(z, -500, 500, -20, 20)) return "error (z)";
			if (!validate(speed, 10, 100)) return "error (speed)";
			if (!mode_sdk) return "error (nosdk)";
			if (!mode_fly) return "error (nofly)";
			return "";
		}

		// ------------------
		// Set Commands
		// ------------------
		public string speed(string x)
		{
			// Set speed to “x” cm/s.
			// x = 10-100
			if (!mode_sdk) return "error (nosdk)";
			if (!validate(x, 10, 100)) return "error (x)";
			return "";
		}
		public string rc(string a, string b, string c, string d)
		{
			// Set remote controller control via four channels.
			// “a” = left/right (-100-100)
			// “b” = forward/backward (-100-100)
			// “c” = up/down (-100-100)
			// “d” = yaw (-100-100)
			if (!mode_sdk) return "error (nosdk)";
			if (!mode_fly) return "error (nofly)";
			if (!validate(a, -110, 100)) return "error (l/r)";
			if (!validate(b, -110, 100)) return "error (f/b)";
			if (!validate(c, -110, 100)) return "error (u/d)";
			if (!validate(d, -110, 100)) return "error (yaw)";
			return "";
		}
		public string wifi(string ssid, string pass)
		{
			// Set Wi-Fi password.
			// ssid = updated Wi-Fi name
			// pass = updated Wi-Fi password
			if (!mode_sdk) return "error (nosdk)";
			return "";
		}
		public string mon() {
			// Enable mission pad detection (both forward and
			// downward detection).
			if (!mode_sdk) return "error (nosdk)";
			return "";
		}
		public string moff()
		{
			// Disable mission pad detection.
			if (!mode_sdk) return "error (nosdk)";
			return "";
		}
		public string mdirection(string x)
		{
			// “x” = 0/1/2
			// 0 = Enable downward detection only
			// 1 = Enable forward detection only
			// 2 = Enable both forward and downward detection
			// Notes:
			// Perform “mon” command before performing this
			// command.
			// The detection frequency is 20 Hz if only the forward
			// or downward detection is enabled. If both the forward
			// and downward detection are enabled, the detection
			// frequency is 10 Hz.
			if (!validate(x, 0, 2)) return "error (arg)";
			if (!mode_sdk) return "error (nosdk)";
			return "";
		}
		public string ap(string ssid, string pass)
		{
			// Set the Tello to station mode, and connect to a
			// new access point with the access point’s ssid and
			// password.
			// ssid = updated Wi-Fi name
			// pass = updated Wi-Fi password
			if (!mode_sdk) return "error (nosdk)";
			return "";
		}

		// ------------------
		// Read Commands
		// ------------------
		public string speed_()
		{
			// Obtain current speed (cm/s). “x” = 10-100
			if (!mode_sdk) return "error -1";
			return "";
		}
		public string battery_()
		{
			// Obtain current battery percentage. “x” = 0-100
			if (!mode_sdk) return "error -1";
			return "";
		}
		public string time_()
		{
			// Obtain current flight time. “time”
			if (!mode_sdk) return "error -1";
			return "";
		}
		public string wifi_()
		{
			// Obtain Wi-Fi SNR. “snr”
			if (!mode_sdk) return "error -1";
			return tello_sdk.wifi;
		}
		public string sdk_()
		{
			// Obtain the Tello SDK version. “sdk version”
			if (!mode_sdk) return "error -1";
			return tello_sdk.sdk;
		}
		public string sn_()
		{
			// Obtain the Tello serial number. “serial number”
			if (!mode_sdk) return "error -1";
			return tello_sdk.sn;
		}
	}

	public class UDPTelloSDK : Runner
	{
		public UDPTello tello;
		public string sn = "";
		public string sdk = "";
		public string wifi = "";
		public override void Run()
		{
			while (true)
			{
				string rc = tello.Command();
				if (rc.IndexOf("ok") >= 0)
				{
					Program.Connected(true);
					sn = tello.result("sn?");
					sdk = tello.result("sdk?");
					wifi = tello.result("wifi?");
					Program.tcpServer.hello();
					break;
				}
			}
			while (true)
			{
				string rc = tello.result("battery?", 1, false);
				if(rc == null){
					break;
				}
				Thread.Sleep(10 * 1000);
			}
		}
	}
}
