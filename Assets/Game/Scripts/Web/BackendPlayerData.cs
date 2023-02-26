using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Web
{
	public class BackendPlayerModel
	{
		public string AccessKey;
		public string RefreshKey;
		public int Id;
		public string Email;
		public string Username;
		public string NickName;
		public int Coin;
		
		public void Accept(LoginResult result)
		{
			AccessKey = result.AccessKey;
			RefreshKey = result.RefreshKey;
		}
		
		public void AcceptNickName(string nickName)
		{
			NickName = nickName;
		}
	}
}