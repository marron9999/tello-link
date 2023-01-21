using System.Configuration;
using System.Net.Sockets;
using System.Text;

#pragma warning disable CS8601 // Null 参照代入の可能性があります。
#pragma warning disable CS8618 // null 非許容のフィールドには、コンストラクターの終了時に null 以外の値が入っていなければなりません。Null 許容として宣言することをご検討ください。
#pragma warning disable CS8604 // Null 参照引数の可能性があります。
#pragma warning disable CS8625 // null リテラルを null 非許容参照型に変換できません。
#pragma warning disable CS8600 // Null リテラルまたは Null の可能性がある値を Null 非許容型に変換しています。

namespace tello_link
{
	public class EMUTello : EMUBase
	{
		public NotifyIcon notifyIcon;

		private UdpClient link;
		private EMUState state;
		private EMUCamera camera;

		public EMUTello() : base("EMUTello")
		{
			link = new UdpClient();
			state = new EMUState();
			camera = new EMUCamera();
		}

		public override void Open()
		{
			int port = Program.Setting("tello_port", 8888);
			base.Open(port);
			state.Open();
			camera.Open();
			port = Program.Setting("link_port", 8889);
			log(name, "connect 127.0.0.1:" + port);
			link.Connect("127.0.0.1", port);
		}
		public override void Close()
		{
			camera.Close();
			state.Close();
			base.Close();
			link.Close();
		}

		public override async void Run()
		{
			log(name + ":" + Port, "open");
			while (true)
			{
				if (_stop) break;
				Received recv = await Receive();
				if (_stop) break;
				if (recv.Sender == null) break;
				Logger.WriteLine(name + " recv: " + recv.Message);
				if (recv.Message.Equals("streamon", StringComparison.OrdinalIgnoreCase))
				{
					camera.send(true);
				}
				else if (recv.Message.Equals("streamoff", StringComparison.OrdinalIgnoreCase))
				{
					camera.send(false);
				}
				else if (recv.Message.Equals("takeoff", StringComparison.OrdinalIgnoreCase))
				{
					Task task = new Task(() =>
					{
						Thread.Sleep(5000);
					});
					task.Start();
					task.Wait();
				}
				else if (recv.Message.Equals("land", StringComparison.OrdinalIgnoreCase))
				{
					Task task = new Task(() =>
					{
						Thread.Sleep(3000);
					});
					task.Start();
					task.Wait();
				}
				link.Send(Encoding.ASCII.GetBytes("ok"));
			}
			log(name + ":" + Port, "close");
		}
	}
}
