using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace Web
{
	public class LoginResult
	{
		[JsonProperty("access")]
		public string AccessKey;
		[JsonProperty("refresh")]
		public string RefreshKey;
	}
	
	public class GetPlayerProfileResult
	{
		[JsonProperty("nick_name")]
		public string NickName;
		[JsonProperty("coin")]
		public int Coin;
	}
}