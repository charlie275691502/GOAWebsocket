using Cysharp.Threading.Tasks;
using Optional;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Common.AssetSession
{
    public class ResourceLoader : IAssetSession
    {
        public static Dictionary<AssetType, string> _assetPath = new Dictionary<AssetType, string>()
        {
            { AssetType.Avatar, "Avatars"},
            { AssetType.DataSheet, "DataSheets"},
        };

        public string GetPath(AssetType assetType, string assetName)
            => _assetPath.TryGetValue(assetType, out var folderPath)
                ? Path.Combine(folderPath, assetName)
                : string.Empty;

        void IAssetSession.AsyncLoad<T>(AssetType assetType, string assetName, Action<T> onComplete)
        {
            var path = GetPath(assetType, assetName);
            UniTask.Void(async () => 
            {
                var resultOpt = await _LoadAsync<T>(path);
                resultOpt.MatchSome(result => onComplete?.Invoke(result));
            });
        }

        Option<T> IAssetSession.SyncLoad<T>(AssetType assetType, string assetName)
        {
            var path = GetPath(assetType, assetName);
            return Resources.Load<T>(path)?.Some() ?? Option.None<T>();
        }

        UniTask<Option<T>> IAssetSession.Load<T>(AssetType assetType, string assetName)
        {
            var path = GetPath(assetType, assetName);
            return _LoadAsync<T>(path);
        }

        private async UniTask<Option<T>> _LoadAsync<T>(string path) where T : Object
        {
            var request = Resources.LoadAsync<T>(path);
            await request;
            return ((T)request.asset)?.Some() ?? Option.None<T>();
        }
    }
}
