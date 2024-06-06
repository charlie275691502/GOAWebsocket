using System;
using System.Collections;
using System.Collections.Generic;
using Common.AssetSession;
using Cysharp.Threading.Tasks;
using Optional;
using UnityEngine;
using UnityEngine.UI;

namespace Common
{
	public class SyncImage : MonoBehaviour
	{
		[SerializeField]
		private Image _image;
		
		private Option<UniTask<Option<Sprite>>> _cacheLoader = Option.None<UniTask<Option<Sprite>>>();
		private IAssetSession _assetSession;
		
		public void Initialize(IAssetSession assetSession)
		{
			_assetSession = assetSession;
		}
		
		public async UniTask LoadSprite(AssetType assetType, string assetName)
		{
			await LoadSprite(_assetSession.SyncLoad<Sprite>(assetType, assetName));
		}
		
		public async UniTask LoadSprite(UniTask<Option<Sprite>> loader)
		{
			_cacheLoader = loader.Some();
			var spriteOpt = await loader;
			_image.enabled = _cacheLoader.Equals(loader.Some()) && spriteOpt.HasValue;
			spriteOpt.MatchSome(sprite => _image.sprite = sprite);
		}
		
		public void Clear()
		{
			_cacheLoader = Option.None<UniTask<Option<Sprite>>>();
			_image.enabled = false;
		}
	}
}
