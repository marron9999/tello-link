using System.IO;
using System.Net;
using System.Net.WebSockets;
using System.Resources;
using System.Text;
using System.Drawing.Imaging;
using System.Net.Sockets;
using System.Security.Cryptography;
using static System.Runtime.InteropServices.JavaScript.JSType;


#pragma warning disable CS8601 // Null 参照代入の可能性があります。
#pragma warning disable CS8618 // null 非許容のフィールドには、コンストラクターの終了時に null 以外の値が入っていなければなりません。Null 許容として宣言することをご検討ください。
#pragma warning disable CS8600 // Null リテラルまたは Null の可能性がある値を Null 非許容型に変換しています。
#pragma warning disable CS8604 // Null 参照引数の可能性があります。

namespace tello_link
{

	public class TCPServer : TCPBase
	{
		public HttpListenerContext context;
		protected List<WSRunner> client = new List<WSRunner>();
		protected bool busy = false;
		public long id = 0;
		public void Remove(WSRunner runner)
		{
			client.Remove(runner);
			if(client.Count <= 0)
			{
				Program.udpTello.streamoff();
			}
		}
		public override void Stop()
		{
			_stop = true;
			foreach (WSRunner runner in client)
			{
				runner.Stop();
			}
			base.Stop();
		}
		protected override void Request(HttpListenerContext context)
		{
			if (!context.Request.IsWebSocketRequest)
			{
				RequestHTTP(context);
				return;
			}
			this.context = context;
			WSRunner runner = new WSRunner();
			runner.service = this;
			client.Add(runner);
			runner.Open();
		}
		public void send(string data)
		{
			busy = true;
			Parallel.ForEach(client, p =>
			{
				p.send(data);
			});
			busy = false;
		}
		public void send(byte[] data)
		{
			busy = true;
			Parallel.ForEach(client, p =>
			{
				p.send(data);
			});
			busy = false;
		}
		public override bool IsBusy()
		{
			return busy;
		}
		public void hello()
		{
			busy = true;
			Parallel.ForEach(client, p =>
			{
				p.hello();
			});
			busy = false;
		}
	}

	public class WSRunner : Runner
	{
		public TCPServer service;
		protected WebSocket socket;
		protected long id;
		public override void Stop()
		{
			base.Stop();
			socket.Abort();
		}
		public void hello()
		{
			if (Program.udpTello.mode_sdk)
			{
				send("Hello");
				send(Program.udpTello.parse("sdk?"));
				send(Program.udpTello.parse("sn?"));
				send(Program.udpTello.parse("wifi?"));
			}
		}
		public override async void Run()
		{
			id = (++service.id);
			string url = service.context.Request.RemoteEndPoint.Address.ToString();
			Logger.WriteLine("WS:" + service.fullname()+ "[" + id + "] connect: " + url);
			socket = (await service.context.AcceptWebSocketAsync(null)).WebSocket;
			hello();
			while (socket.State == WebSocketState.Open)
			{
				if (_stop) break;
				try
				{
					var seg = new ArraySegment<byte>(new byte[4096]);
					var ret = await socket.ReceiveAsync(seg, CancellationToken.None);
					if (_stop) break;
					byte[] buf = seg.Take(ret.Count).ToArray();
					if (ret.MessageType == WebSocketMessageType.Text)
					{
						string text = System.Text.Encoding.UTF8.GetString(buf);
						if(text.Length > 0)
						{
							onMessage(socket, text);
						} else
						{
							send("");
						}
						continue;
					}
					if (ret.MessageType == WebSocketMessageType.Binary)
					{
						onBinary(socket, buf);
						continue;
					}
					if (ret.MessageType == WebSocketMessageType.Close)
					{
						break;
					}
				}
				catch (Exception ex)
				{
					if(socket.State == WebSocketState.Open)
					{
						Logger.WriteLine("WS:" + service.fullname() + "[" + id + "] error: " + ex.ToString());
						Logger.WriteLine(ex.StackTrace);
					}
				}
			}
			_stop = true;
			Logger.WriteLine("WS:" + service.fullname() + "[" + id + "] state :" + socket.State);
			Logger.WriteLine("WS:" + service.fullname() + "[" + id + "] disconnect :" + url);
			service.Remove(this);
		}
		protected void onMessage(WebSocket socket, string data)
		{
			Logger.WriteLine("WS:" + service.fullname() + "[" + id + "]: " + data);
			//data += " ok";
			data = Program.udpTello.parse(data);
			send(data);
		}
		protected void onBinary(WebSocket socket, byte[] data)
		{
			// NONE
		}
		public void send(string data)
		{
			if (_stop) return;
			byte[] buf = System.Text.Encoding.UTF8.GetBytes(data);
			lock(socket)
			{
				Task task = socket.SendAsync(buf, WebSocketMessageType.Text, true, CancellationToken.None);
				task.Wait();
			}
		}
		public void send(byte[] data)
		{
			if (_stop) return;
			var buf = new ArraySegment<byte>(data);
			lock (socket)
			{
				Task task = socket.SendAsync(buf, WebSocketMessageType.Binary, true, CancellationToken.None);
				task.Wait();
			}
		}
	}
}
