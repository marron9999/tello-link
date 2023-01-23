using System.Net;
using System.Net.Sockets;
using System.Text;

#pragma warning disable CS8618 // null 非許容のフィールドには、コンストラクターの終了時に null 以外の値が入っていなければなりません。Null 許容として宣言することをご検討ください。

namespace tello_link
{
	// Simple UDP Server
	public struct Received
	{
		public IPEndPoint Sender;
		public string Message;
		public byte[] bytes;
	}

	public class EMUBase : Runner
	{
		protected string name = "EMUServer";
		protected int Port = 0;
		protected IPAddress Addr;
		protected UdpClient client;

		public EMUBase(string name)
		{
			this.name = name;
		}

		public virtual void Open(int port)
		{
			this.Port = port;
			this.Addr = IPAddress.Any;
			IPEndPoint ep = new IPEndPoint(Addr, Port);
			client = new UdpClient(ep);
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
