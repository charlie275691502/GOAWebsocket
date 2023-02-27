using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using System.Linq;
using Common;
using Web;

namespace Common
{
	public interface IBackendPlayerPresenter
	{
		void Accept(LoginResult result);
		void Accept(GetPlayerProfileResult result);
		void AcceptNickName(string nickName);
		void AcceptCoin(int coin);
	} 
	
	public class BackendPlayerPresenter : IBackendPlayerPresenter
	{
		private BackendPlayerData _data;
		private List<IBackendPlayerView> _views;
		
		public BackendPlayerPresenter(
			BackendPlayerData data, 
			[Inject(Id="TopMenu")] IBackendPlayerView topMenuView)
		{
			_data = data;
			_views = new List<IBackendPlayerView>()
			{
				topMenuView,
			};
		}
		
		public void Accept(LoginResult result)
		{
			_data.Accept(result);
		}
		
		public void Accept(GetPlayerProfileResult result)
		{
			_data.Accept(result);
			_views.ForEach(view => view.UpdateNickName(result.NickName));
			_views.ForEach(view => view.UpdateCoin(result.Coin));
		}
		
		public void AcceptNickName(string nickName)
		{
			_data.AcceptNickName(nickName);
			_views.ForEach(view => view.UpdateNickName(nickName));
		}
		
		public void AcceptCoin(int coin)
		{
			_data.AcceptCoin(coin);
			_views.ForEach(view => view.UpdateCoin(coin));
		}
	}
}