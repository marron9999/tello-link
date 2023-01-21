using System.IO;
using System.Net;
using System.Net.WebSockets;
using System.Resources;
using System.Text;
using System.Drawing.Imaging;
using System.Configuration;
using System.Drawing.Drawing2D;

#pragma warning disable CS8601 // Null 参照代入の可能性があります。
#pragma warning disable CS8618 // null 非許容のフィールドには、コンストラクターの終了時に null 以外の値が入っていなければなりません。Null 許容として宣言することをご検討ください。
#pragma warning disable CS8600 // Null リテラルまたは Null の可能性がある値を Null 非許容型に変換しています。
#pragma warning disable CS8604 // Null 参照引数の可能性があります。

namespace tello_link
{

	public class TCPStream : TCPServer
	{
		public TCPStream()
		{
			port = 8081;
		}
	}
}
