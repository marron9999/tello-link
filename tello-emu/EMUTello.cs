using System.Text;

namespace tello_link
{
	public class EMUTello : EMUBase
	{
		private EMUState state;
		private EMUCamera camera;
		private int link_port;

		public EMUTello() : base("EMUTello")
		{
			state = new EMUState();
			camera = new EMUCamera();
		}

		public override void Open()
		{
			int port = Program.Setting("tello_port", 8888);
			link_port = Program.Setting("link_port", 8889);
			base.Open(port);
			state.Open();
			camera.Open();
		}

		public override void Close()
		{
			camera.Close();
			state.Close();
			base.Close();
		}

		public void land() {
			state.fly = false;
		}

		public override async void Run()
		{
			log(name + " " + Addr + ":" + Port, "open");
			while (true)
			{
				if (_stop) break;
				Received recv = await Receive();
				if (_stop) break;
				if (recv.Sender == null) break;
				string rc = "ok";
				Logger.WriteLine(name + ": " + recv.Sender + ": " + recv.Message);
				if (recv.Message.Equals("streamon"))
				{
					camera.send(true);
				}
				else if (recv.Message.Equals("streamoff"))
				{
					camera.send(false);
				}
				else if (recv.Message.Equals("takeoff"))
				{
					Task task = new Task(() =>
					{
						for (state.tof = 50; state.tof < 100; state.tof += 5)
						{
							Thread.Sleep(500);
						}
					});
					task.Start();
					state.take_time = DateTime.Now;
					state.fly_prev = state.fly_time;
					state.fly = true;
					task.Wait();
				}
				else if (recv.Message.Equals("land"))
				{
					Task task = new Task(() =>
					{
						while(true)
						{
							int t = state.tof - 20;
							if(t <= 0)
							{
								state.tof = 0;
								break;
							}
							state.tof = t;
							Thread.Sleep(500);
						}
					});
					task.Start();
					task.Wait();
					land();
				}
				else if (recv.Message.StartsWith("up"))
				{
					if(state.fly)
					{
						int p = recv.Message.IndexOf(" ");
						state.tof += Int32.Parse(recv.Message.Substring(p + 1));
					}
				}
				else if (recv.Message.StartsWith("down"))
				{
					if (state.fly)
					{
						int p = recv.Message.IndexOf(" ");
						state.tof -= Int32.Parse(recv.Message.Substring(p + 1));
						if (state.tof <= 0)
						{
							state.tof = 0;
							land();
						}
					}
				}
				else if (recv.Message.Equals("emergency"))
				{
					state.fly = false;
				}
				else if (recv.Message.Equals("sdk?"))
				{
					rc = "01";
				}
				else if (recv.Message.Equals("sn?"))
				{
					rc = "12345678";
				}
				else if (recv.Message.Equals("wifi?"))
				{
					rc = "90";
				}
				else if (recv.Message.Equals("@reset"))
				{
					state.reset();
				}
				byte[] buf = Encoding.ASCII.GetBytes(rc);
				client.Send(buf, buf.Length, "127.0.0.1", link_port);
			}
			log(name + " " + Addr + ":" + Port, "close");
		}
	}
}
