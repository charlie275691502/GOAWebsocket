using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Web;

namespace Common
{
	public class BackendPlayerData
	{
		public string AccessKey {get; private set;}
		public string RefreshKey {get; private set;}
		public int Id {get; private set;}
		public string Email {get; private set;}
		public string Username {get; private set;}
		public string NickName {get; private set;}
		public int Coin {get; private set;}
		
		public void Accept(LoginResult result)
		{
			AccessKey = result.AccessKey;
			RefreshKey = result.RefreshKey;
		}
		
		public void Accept(GetPlayerProfileResult result)
		{
			NickName = result.NickName;
			Coin = result.Coin;
		}
		
		public void AcceptNickName(string nickName)
		{
			NickName = nickName;
		}
		
		public void AcceptCoin(int coin)
		{
			Coin = coin;
		}
	}
}