using System.Configuration;
using System.Drawing.Imaging;
using System.Net;
using System.Text;

#pragma warning disable CS8618 // null 非許容のフィールドには、コンストラクターの終了時に null 以外の値が入っていなければなりません。Null 許容として宣言することをご検討ください。
#pragma warning disable CS8600 // Null リテラルまたは Null の可能性がある値を Null 非許容型に変換しています。
#pragma warning disable CS8604 // Null 参照引数の可能性があります。

namespace tello_link
{
	public class TCPBase : Runner
	{
		protected string name = "TCP";
		protected int Port = 8080;
		protected IPAddress Addr = IPAddress.Any;
		protected HttpListener listener;

		public TCPBase()
		{
		}

		public string fullname()
		{
			return name + " " + Addr + ":" + Port;
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
				log(fullname(), "open");
				listener = new HttpListener();
				listener.Prefixes.Clear();
				listener.Prefixes.Add("http://+:" + Port + "/"); // プレフィックスの登録
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
					log(fullname(), ex.Message);
					Logger.WriteLine(ex.StackTrace);
				}
			}
			log(fullname(), "close");
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
				log(fullname(), path);
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
					node = "c:/java/github/tello-link/tello-html";
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
				log(fullname(), ex.ToString());
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
