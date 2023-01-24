using OpenCvSharp;
using OpenCvSharp.Extensions;

#pragma warning disable CS8604 // Null 参照引数の可能性があります。
#pragma warning disable CS8618 // null 非許容のフィールドには、コンストラクターの終了時に null 以外の値が入っていなければなりません。Null 許容として宣言することをご検討ください。

namespace tello_link
{
	public class UDPStream : UDPBase
	{
		private VideoCapture vc;
		private int stream_port = 11111;
		//private int stream_fps = 2;
		private bool _send = true;
		private int frame_port = 11111;

		public UDPStream() : base("UDPStream")
		{
		}

		public override void Open()
		{
			stream_port = Program.Setting("stream_port", 11111);
			//stream_fps = Program.Setting("stream_fps", 2);
			if(Program.emulate > 0) {
				frame_port = Program.Setting("frame_port", 11111);
				base.Open("127.0.0.1", frame_port);
			}
			else
			{
				base.Open(0);
				Port = stream_port;
			}
		}

		public override async void Run()
		{
			if (Program.emulate > 0)
			{
				log(name, "open " + frame_port);
				while (true)
				{
					Received recv = await Receive();
					if (recv.Sender == null) break;
					if (_send)
					{
						if (Program.tcpStream != null
						&& (!Program.tcpStream.IsBusy()))
						{
							Program.tcpStream.send(recv.bytes);
						}
					}
				}
				log(name, "close " + frame_port);
				return;
			}
			string url = "udp://0.0.0.0:" + stream_port;
			//log(name, "open " + url + ", FPS:" + stream_fps);
			log(name, "open " + url);
			try
			{
				vc = new VideoCapture(url, VideoCaptureAPIs.FFMPEG);
				//vc.Fps = stream_fps;
				var mat = new Mat();
				while (vc.IsOpened())
				{
					if (_stop) break;
					vc.Read(mat);
					var width = mat.Size().Width;
					if (width > 0)
					{
						var ms = new MemoryStream();
						width = width / 2;
						var height = mat.Size().Height / 2;
						var half = mat.Resize(new OpenCvSharp.Size(width, height), 0, 0,
								OpenCvSharp.InterpolationFlags.Lanczos4);
						var bitmap = BitmapConverter.ToBitmap(half);
						bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
						var buf = ms.GetBuffer();
						ms.Close();
						if (_send)
						{
							if (Program.tcpStream != null
							&& (!Program.tcpStream.IsBusy()))
							{
								Program.tcpStream.send(buf);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				if (!vc.IsDisposed)
				{
					log(fullname(), "error: " + ex.Message);
					Logger.WriteLine(ex.StackTrace);
				}
			}
			if (!vc.IsDisposed)
			{
				vc.Dispose();
			}
			log(name, "close " + url);
		}
	}
}
