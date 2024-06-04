using Common.UniTaskExtension;
using Common.Warning;
using Cysharp.Threading.Tasks;
using OneOf;
using Optional;

namespace Web
{
	public static class WebUtility
	{
		public static bool RequestDebugMode = true;
		public static string Host = "192.168.180.108";
		public static string Port = "9000";
		public static string Domain = string.IsNullOrEmpty(Port) ? string.Format("{0}", Host) : string.Format("{0}:{1}", Host, Port);
		
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