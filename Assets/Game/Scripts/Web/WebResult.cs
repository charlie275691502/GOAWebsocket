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
	
	public class PlayerDataResult
	{
		[JsonProperty("id")]
		public int Id;
		[JsonProperty("nick_name")]
		public string NickName;
		[JsonProperty("coin")]
		public int Coin;
	}
	
	public class RoomResult
	{
		[JsonProperty("id")]
		public int Id;
		[JsonProperty("room_name")]
		public string RoomName;
		[JsonProperty("players")]
		public List<PlayerDataResult> Players;
	}
	
	public class RoomListResult : List<RoomResult>
	{
		
	}
}