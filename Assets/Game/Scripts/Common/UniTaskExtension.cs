using Cysharp.Threading.Tasks;
using Optional;
using System;

namespace Common.UniTask
{
    public static class UniTaskExtension
    {
        public static async UniTask<Option<None>> ToNone<T>(this UniTask<Option<T>> uniTask)
        {
            var requestOpt = await uniTask;
            return requestOpt.Map(_ => default(None));
        }

        public static async UniTask<Option<T>> OnSuccess<T>(this UniTask<Option<T>> uniTask, Action<T> onSuccess)
        {
            var resultOpt = await uniTask;
            resultOpt.MatchSome(result => onSuccess(result));
            return resultOpt;
        }

        public static async UniTask<Option<T>> OnSuccess<T>(this UniTask<Option<T>> uniTask, Func<T, Cysharp.Threading.Tasks.UniTask> onSuccess)
        {
            var resultOpt = await uniTask;
            resultOpt.MatchSome(async result => await onSuccess(result));
            return resultOpt;
        }
    }
}
