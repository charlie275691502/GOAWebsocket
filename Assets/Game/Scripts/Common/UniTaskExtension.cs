using Cysharp.Threading.Tasks;
using OneOf;
using OneOf.Types;
using Optional;
using System;

namespace Common.UniTask
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

        public static async UniTask<OneOf<T, UniTaskError>> OnSuccess<T>(this UniTask<OneOf<T, UniTaskError>> uniTask, Func<T, Cysharp.Threading.Tasks.UniTask> onSuccess)
        {
            var requestOneOf = await uniTask;
            requestOneOf.Switch(
                async result => await onSuccess(result),
                _ => { });
            return requestOneOf;
        }
    }
}
