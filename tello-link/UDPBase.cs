using System.Net;
using System.Net.Sockets;
using System.Text;

#pragma warning disable CS8618 // null 非許容のフィールドには、コンストラクターの終了時に null 以外の値が入っていなければなりません。Null 許容として宣言することをご検討ください。

namespace tello_link
{
	public struct Received
	{
		public IPEndPoint Sender;
		public string Message;
		public byte[] bytes;
	}

	public class UDPBase : Runner
	{
		protected string name = "UDPServer";
		protected UdpClient client;
		protected int Port;
		protected IPAddress Addr;

		public UDPBase(string name)
		{
			this.name = name;
		}

		protected string fullname()
		{
			return name + " " + Addr + ":" + Port;
		}

		public virtual void Open(int port)
		{
			if(port > 0)
			{
				Port = port;
				Addr = IPAddress.Any;
				IPEndPoint ep = new IPEndPoint(Addr, Port);
				client = new UdpClient(ep);
			}
			base.Open();
		}

		public virtual void Open(string addr, int port)
		{
			Port = port;
			Addr = IPAddress.Parse(addr);
			IPEndPoint ep = new IPEndPoint(Addr, Port);
			client = new UdpClient(ep);
			base.Open();
		}

		public override void Stop()
		{
			_stop = true;
			try
			{
				client.Close();
				client.Dispose();
			}
			catch (Exception)
			{
				// NONE
			}
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
			catch (Exception ex)
			{
				if( !_stop)
				{
					log(fullname(), ex.ToString());
				}
			}
			return new Received();
		}
	}
}
