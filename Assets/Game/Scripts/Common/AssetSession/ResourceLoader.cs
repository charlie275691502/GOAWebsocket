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
			{ AssetType.GOACharacterMid, Path.Join("Characters", "Mid")},
			{ AssetType.GOACharacterSmall, Path.Join("Characters", "Small")},
			{ AssetType.GOAPublicCardNormal, Path.Join("Cards", "PublicCards", "Normal")},
			{ AssetType.GOAPublicCardChosen, Path.Join("Cards", "PublicCards", "Chosen")},
			{ AssetType.GOAPublicCardDetail, Path.Join("Cards", "PublicCards", "Detail")},
			{ AssetType.GOAStrategyCardNormal, Path.Join("Cards", "StrategyCards", "Normal")},
			{ AssetType.GOAStrategyCardDetail, Path.Join("Cards", "StrategyCards", "Detail")},
		};

		public string GetPath(AssetType assetType, string assetName)
			=> string.IsNullOrEmpty(assetName) || !_assetPath.TryGetValue(assetType, out var folderPath)
				? string.Empty
				: Path.Combine(folderPath, assetName);

		void IAssetSession.AsyncLoad<T>(AssetType assetType, string assetName, Action<Option<T>> onComplete)
		{
			var path = GetPath(assetType, assetName);
			UniTask.Void(async () => 
			{
				var resultOpt = await _LoadAsync<T>(path);
				onComplete?.Invoke(resultOpt);
			});
		}

		UniTask<Option<T>> IAssetSession.SyncLoad<T>(AssetType assetType, string assetName)
		{
			var path = GetPath(assetType, assetName);
			return _LoadAsync<T>(path);
		}

		Option<T> IAssetSession.SyncBlockLoad<T>(AssetType assetType, string assetName)
		{
			var path = GetPath(assetType, assetName);
			return Resources.Load<T>(path)?.Some() ?? Option.None<T>();
		}

		private async UniTask<Option<T>> _LoadAsync<T>(string path) where T : Object
		{
			var request = Resources.LoadAsync<T>(path);
			await request;
			return ((T)request.asset)?.Some() ?? Option.None<T>();
		}
	}
}
