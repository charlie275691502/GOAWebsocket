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
		DataSheet,
	}

	public interface IAssetSession
	{
		void AsyncLoad<T>(AssetType assetType, string assetName, Action<T> onComplete) where T : Object;
		Option<T> SyncLoad<T>(AssetType assetType, string assetName) where T : Object;
		UniTask<Option<T>> Load<T>(AssetType assetType, string assetName) where T : Object;
	}
}
