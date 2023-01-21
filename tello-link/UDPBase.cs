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

	// Simple UDP Server
	public struct Received
	{
		public IPEndPoint Sender;
		public string Message;
		public byte[] bytes;
	}

	public class UDPBase : Runner
	{
		protected string name = "UDPServer";
		protected int Port = 0;
		protected UdpClient client;
		public UDPBase(string name)
		{
			this.name = name;
		}
		public virtual void Open(int port)
		{
			Port = port;
			if(port > 0)
			{
				IPEndPoint ep = new IPEndPoint(IPAddress.Any, Port);
				client = new UdpClient(ep);
			}
			base.Open();
		}
		public override void Stop()
		{
			_stop = true;
			client.Close();
			base.Stop();
		}
		public async Task<Received> Receive()
		{
			try
			{
				var result = await client.ReceiveAsync();
				return new Received()
				{
					bytes = result.Buffer.ToArray(),
					Message = Encoding.ASCII.GetString(result.Buffer, 0, result.Buffer.Length),
					Sender = result.RemoteEndPoint
				};
			}
			catch (Exception /*ex*/)
			{
				//
			}
			return new Received();
		}
	}
}
