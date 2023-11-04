using Cysharp.Threading.Tasks;
using OneOf;
using OneOf.Types;
using Optional;
using System;

namespace Common.UniTaskExtension
{
	public record UniTaskError(string Error);

	public static class UniTaskExtension
	{
		public static async UniTask<OneOf<None, UniTaskError>> ToNone<T>(this UniTask<OneOf<T, UniTaskError>> uniTask)
		{
			var requestOneOf = await uniTask;
			return requestOneOf.MapT0(_ => default(None));
		}

		public static async UniTask<OneOf<T, UniTaskError>> OnSuccess<T>(this UniTask<OneOf<T, UniTaskError>> uniTask, Action<T> onSuccess)
		{
			var requestOneOf = await uniTask;
			requestOneOf.Switch(
				result => onSuccess(result),
				_ => { });
			return requestOneOf;
		}

		public static async UniTask<OneOf<T, UniTaskError>> OnSuccess<T>(this UniTask<OneOf<T, UniTaskError>> uniTask, Func<T, UniTask> onSuccess)
		{
			var requestOneOf = await uniTask;
			if (requestOneOf.TryPickT0(out var result, out var _))
			{
				await onSuccess(result);
			}
			return requestOneOf;
		}

		public static async UniTask<T> Then<T>(this UniTask uniTask, UniTask<T> second)
		{
			await uniTask;
			return await second;
		}

		public static async UniTask<R> Then<T, R>(this UniTask<T> uniTask, Func<T, UniTask<R>> mapping)
		{
			var request = await uniTask;
			return await mapping(request);
		}

		public static async UniTask<Option<TResult>> Map<T, TResult>(this UniTask<Option<T>> uniTask, Func<T, TResult> mapping)
		{
			var requestOpt = await uniTask;
			return requestOpt.Map(mapping);
		}

		public static async UniTask Match<T>(this UniTask<Option<T>> uniTask, Action<T> some, Action none)
		{
			var requestOpt = await uniTask;
			requestOpt.Match(some, none);
		}

		public static async UniTask<bool> IsSuccess<T>(this UniTask<Option<T>> uniTask)
		{
			var requestOpt = await uniTask;
			return requestOpt.HasValue;
		}

		public static async UniTask<bool> IsFail<T>(this UniTask<Option<T>> uniTask)
		{
			var requestOpt = await uniTask;
			return !requestOpt.HasValue;
		}
	}
}
