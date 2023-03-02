using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Common
{
	public class DynamicScrollRect<T> : ScrollRect
	{
		private GameObjectPool _pool;
		private Action<int, T> _onInstantiate;
		private List<GameObject> _gameObjectList = new List<GameObject>();
		
		public void Enter(GameObjectPool pool, Action<int, T> onInstantiate)
		{
			_pool = pool;
			_onInstantiate = onInstantiate;
		}
		
		public void Leave()
		{
			_Clear();
		}
		
		public void FillItems(int count)
		{
			_Clear();
			for(int i=0; i < count; i++)
			{
				_AddItem(i);
			}
		}
		
		public void AppendItem(int count)
		{
			var curCount = _gameObjectList.Count;
			for(int i=curCount; i < curCount + count; i++)
			{
				_AddItem(i);
			}
		}
		
		private void _AddItem(int index)
		{
			var gameObject = _pool.GetGameObject();
			gameObject.transform.SetParent(content.transform);
			_gameObjectList.Add(gameObject);
			_onInstantiate?.Invoke(index, gameObject.GetComponent<T>());
		}
		
		private void _Clear()
		{
			_gameObjectList.ForEach(gameObject => _pool.ReturnGameObject(gameObject));
			_gameObjectList.Clear();
		}
	}
}