using System.IO;
using System.Net;
using System.Net.WebSockets;
using System.Resources;
using System.Text;
using System.Drawing.Imaging;
using System.Security.Policy;
using System.Configuration;



#pragma warning disable CS8601 // Null 参照代入の可能性があります。
#pragma warning disable CS8618 // null 非許容のフィールドには、コンストラクターの終了時に null 以外の値が入っていなければなりません。Null 許容として宣言することをご検討ください。
#pragma warning disable CS8600 // Null リテラルまたは Null の可能性がある値を Null 非許容型に変換しています。
#pragma warning disable CS8604 // Null 参照引数の可能性があります。

namespace tello_link
{
	public class TCPBase : Runner
	{
		public int port = 8080;
		protected HttpListener listener;

		public TCPBase()
		{
		}
		public override void Stop()
		{
			_stop = true;
			listener.Abort();
			base.Stop();
		}
		public override void Run()
		{
			try
			{
				log("TCP:" + port, "open");
				listener = new HttpListener();
				listener.Prefixes.Clear();
				listener.Prefixes.Add("http://+:" + port + "/"); // プレフィックスの登録
				//listener.TimeoutManager.DrainEntityBody = new TimeSpan(23, 59, 59);
				//listener.TimeoutManager.IdleConnection = new TimeSpan(23, 59, 59);
				listener.Start();
				while (listener.IsListening)
				{
					if (_stop) break;
					HttpListenerContext context = listener.GetContext();
					if (_stop) break;
					Request(context);
				}
				listener.Close();
			}
			catch (Exception ex)
			{
				if(listener.IsListening)
				{
					log("TCPBase error", ex.Message);
					Logger.WriteLine(ex.StackTrace);
				}
			}
			log("TCP:" + port, "close");
		}
		protected virtual void Request(HttpListenerContext context)
		{
			RequestHTTP(context);
		}
		protected virtual void RequestHTTP(HttpListenerContext context)
		{
			HttpListenerRequest req = context.Request;
			HttpListenerResponse res = context.Response;
			try
			{
				string path = req.RawUrl;
				if (path == null) path = "";
				path = path.Replace("\\", "/");
				if (path.Length == 0)
				{
					path = "/";
				}
				if (path.EndsWith("/"))
				{
					path += "index.html";
				}
				log("TCP:" + port, path);
				string node = path.Substring(1).Replace(".", "_");
				Object o = Resources.ResourceManager.GetObject(node);
				if (o != null)
				{
					if (o.GetType() == typeof(string))
					{
						byte[] buf = Encoding.UTF8.GetBytes((string)o);
						res.OutputStream.Write(buf, 0, buf.Length);
						res.StatusCode = (int)HttpStatusCode.OK;
						res.Close();
						return;
					}
					if (o.GetType() == typeof(Bitmap))
					{
						MemoryStream ms = new MemoryStream();
						((Bitmap)o).Save(ms, path.EndsWith("_jpg") ? ImageFormat.Jpeg : ImageFormat.Png);
						byte[] buf = ms.GetBuffer();
						ms.Close();
						res.OutputStream.Write(buf, 0, buf.Length);
						res.StatusCode = (int)HttpStatusCode.OK;
						res.Close();
						return;
					}
				}
#if DEBUG
				if(path.StartsWith("/tello/"))
				{
					path = path.Substring(6);
					node = "c:/java/github/tello-html";
				}
				else node = ".";
#else
				node = ConfigurationManager.AppSettings["root"];
#endif
				node = (node + path).Replace("/", "\\");
				if (File.Exists(node))
				{
					FileStream fs = new FileStream(node, FileMode.Open);
					int len;
					byte[] buf = new byte[4096];
					while ((len = fs.Read(buf, 0, buf.Length)) > 0)
					{
						res.OutputStream.Write(buf, 0, len);
					}
					fs.Close();
					res.StatusCode = (int)HttpStatusCode.OK;
					res.Close();
					return;
				}
			}
			catch (Exception ex)
			{
				log("TCP:" + port, ex.ToString());
				Logger.WriteLine(ex.StackTrace);
			}
			res.StatusCode = (int)HttpStatusCode.NotFound;
			res.Close();
		}

		public virtual bool IsBusy()
		{
			return false;
		}
	}
}
