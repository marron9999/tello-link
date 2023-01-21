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
	public class UDPStream : UDPBase
	{
		private VideoCapture vc;
		private int stream_port = 11111;
		private int stream_fps = 2;
		private bool _send = true;
		private int frame_port = 11112;
		public UDPStream() : base("UDPStream")
		{
			vc = new VideoCapture();
		}
		public override void Open()
		{
			stream_port = Program.Setting("stream_port", 11111);
			stream_fps = Program.Setting("stream_fps", 2);
			if(Program.emulate > 0) {
				frame_port = Program.Setting("frame_port", 11112);
				base.Open(frame_port);
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
				log(name + ":" + Port, "open");
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
				log(name + ":" + Port, "close");
				return;
			}
			client = new UdpClient();
			try
			{
				log(name + ":" + Port, "Open, FPS:" + stream_fps);
				vc.Open("udp:///0.0.0.0:" + stream_port);
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
				log(name + Port, "close");
			}
			catch (Exception ex)
			{
				if (!vc.IsDisposed)
				{
					log(name, "error: " + ex.Message);
					Logger.WriteLine(ex.StackTrace);
				}
			}
			client.Close();
			if (!vc.IsDisposed)
			{
				vc.Dispose();
			}
		}
	}
}
