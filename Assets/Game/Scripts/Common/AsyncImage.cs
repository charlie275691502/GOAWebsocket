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
	public class AsyncImage : MonoBehaviour
	{
		[SerializeField]
		private Image _image;
		
		private Option<AssetType> _assetType = Option.None<AssetType>();
		private Option<string> _assetName = Option.None<string>();
		private IAssetSession _assetSession;
		
		public void Initialize(IAssetSession assetSession)
		{
			_assetSession = assetSession;
		}
		
		public void LoadSprite(AssetType assetType, string assetName)
		{
			_assetType = assetType.Some();
			_assetName = assetName.Some();
			_assetSession.AsyncLoad<Sprite>(
				assetType,
				assetName,
				(spriteOpt) => 
				{
					if(_assetType.Contains(assetType) && _assetName.Contains(assetName))
					{
						_image.enabled = spriteOpt.HasValue;
						spriteOpt.MatchSome(sprite => _image.sprite = sprite);
					}
				});
		}
		
		public void Clear()
		{
			_assetType = Option.None<AssetType>();
			_assetName = Option.None<string>();
			_image.enabled = false;
		}
	}
}
