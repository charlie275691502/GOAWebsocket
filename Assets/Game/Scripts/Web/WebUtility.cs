using System.Collections;
using System.Collections.Generic;
using Common;
using Common.UniTaskExtension;
using Cysharp.Threading.Tasks;
using OneOf;
using OneOf.Types;
using Optional;

namespace Web
{
	public static class WebUtility
	{
		public static bool RequestDebugMode = true;
		public static string Host = "127.0.0.1";
		public static string Port = "9000";
		
		public static async UniTask<Option<T>> RunAndHandleInternetError<T>(this UniTask<OneOf<T, UniTaskError>> uniTask, IWarningPresenter warningPresenter)
		{
			var resultOneOf = await uniTask;
			resultOneOf.Switch(
				_ => { },
				async error => await warningPresenter.Run("Error occurs when send to server", error.Error));

			return resultOneOf.Match(result => result.Some(), _ => Option.None<T>());
		}
	}
}