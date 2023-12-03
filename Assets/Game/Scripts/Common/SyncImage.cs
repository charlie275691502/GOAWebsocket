using System;
using System.Collections;
using System.Collections.Generic;
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
