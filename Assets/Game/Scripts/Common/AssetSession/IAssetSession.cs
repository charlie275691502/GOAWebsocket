using Cysharp.Threading.Tasks;
using Optional;
using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Common.AssetSession
{
	public enum AssetType
	{
		Avatar,
		GOACharacterMid,
		GOACharacterSmall,
		GOAPublicCardNormal,
		GOAPublicCardChosen,
		GOAPublicCardDetail,
		GOAStrategyCardNormal,
		GOAStrategyCardDetail,
	}

	public interface IAssetSession
	{
		string GetPath(AssetType assetType, string assetName);
		void AsyncLoad<T>(AssetType assetType, string assetName, Action<Option<T>> onComplete) where T : Object;
		UniTask<Option<T>> SyncLoad<T>(AssetType assetType, string assetName) where T : Object;
		Option<T> SyncBlockLoad<T>(AssetType assetType, string assetName) where T : Object;
	}
}
