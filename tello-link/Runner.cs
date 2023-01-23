#pragma warning disable CS8618 // null 非許容のフィールドには、コンストラクターの終了時に null 以外の値が入っていなければなりません。Null 許容として宣言することをご検討ください。
#pragma warning disable CS1998 // 非同期メソッドは、'await' 演算子がないため、同期的に実行されます

namespace tello_link
{
	public class Runner
	{
		protected Task _task;
		protected bool _stop;

		public virtual async void Run()
		{
		}

		public virtual void Close()
		{
			Stop();
			_task.Wait();
		}

		public virtual void Open()
		{
			_task = Task.Factory.StartNew(Run);
		}

		public virtual void Stop()
		{
			_stop = true;
		}

		protected void log(string prefix, string text)
		{
			if (text == null)
			{
				Logger.WriteLine(prefix);
				return;
			}
			Logger.WriteLine(prefix + ": " + text);
		}
	}
}
