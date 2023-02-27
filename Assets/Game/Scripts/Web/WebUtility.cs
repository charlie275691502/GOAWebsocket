using System.Collections;
using System.Collections.Generic;
using Common;
using Rayark.Mast;

namespace Web
{
	public static class WebUtility
	{
		public static bool RequestDebugMode = true;
		public static string Host = "127.0.0.1";
		public static string Port = "9000";
		
		public static IEnumerator RunAndHandleInternetError<T>(this IMonad<T> monad, IWarningPresenter warningPresenter)
		{
			yield return monad.Do();
			if(monad.Error != null)
			{
				yield return warningPresenter.Run("Error occurs when send to server", monad.Error.Message.ToString());
				yield break;
			}
		}
	}
}