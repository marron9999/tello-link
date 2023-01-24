using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.Net.Sockets;

#pragma warning disable CS8618 // null 非許容のフィールドには、コンストラクターの終了時に null 以外の値が入っていなければなりません。Null 許容として宣言することをご検討ください。
#pragma warning disable CS8604 // Null 参照引数の可能性があります。

namespace tello_link
{
	public class EMUCamera : Runner
	{
		private UdpClient client;
		private VideoCapture vc;
		private int stream_port = 11111;
		private int stream_fps = 2;
		private int camera = 0;
		private bool _send = false;

		public void send(bool sw)
		{
			if(sw)
			{
				if( ! _send)
				{
					Logger.WriteLine("EMUPCamera: connect 127.0.0.1:" + stream_port);
					client = new UdpClient();
					client.Connect("127.0.0.1", stream_port);
					_send = true;
				}
			}
			else
			{
				if (_send)
				{
					_send = false;
					client.Close();
					Logger.WriteLine("EMUCamera: disconnect 127.0.0.1:" + stream_port);
				}
			}
		}

		public EMUCamera()
		{
			stream_port = Program.Setting("udp_port", 11111);
			stream_fps = Program.Setting("stream_fps", 2);
			camera = Program.Setting("camera", 0);
		}

		public override void Run()
		{
			vc = new VideoCapture();
			client = new UdpClient();
			try
			{
				Logger.WriteLine("EMUCamera: Open camera:" + camera + ", FPS:" + stream_fps);
				vc.Open(camera);
				vc.Fps = stream_fps;
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
						bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
						var buf = ms.GetBuffer();
						ms.Close();
						if(_send)
						{
							client.Send(buf);
						}
					}
				}
				Logger.WriteLine("EMUCamera close: " + 0);
			}
			catch (Exception ex)
			{
				if ( ! vc.IsDisposed)
				{
					Logger.WriteLine("EMUCamera error: " + ex.Message);
					Logger.WriteLine(ex.StackTrace);
				}
			}
			client.Close();
			if ( ! vc.IsDisposed)
			{
				vc.Dispose();
			}
		}
	}
}
