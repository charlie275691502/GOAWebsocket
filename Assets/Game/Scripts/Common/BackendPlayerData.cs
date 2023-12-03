using Web;

namespace Common
{
	public class BackendPlayerData
	{
		public string AccessKey {get; private set;}
		public string RefreshKey {get; private set;}
		public string Email {get; private set;}
		public string Username {get; private set;}
		public PlayerData PlayerData {get; private set;} = new PlayerData();
		
		public void Accept(LoginResult result)
		{
			AccessKey = result.AccessKey;
			RefreshKey = result.RefreshKey;
		}
		
		public void Accept(PlayerDataResult result)
		{
			PlayerData.Accept(result);
		}
		
		public void AcceptNickName(string nickName)
		{
			PlayerData.AcceptNickName(nickName);
		}
		
		public void AcceptCoin(int coin)
		{
			PlayerData.AcceptCoin(coin);
		}
	}
	
	public class PlayerData
	{
		public int Id {get; private set;}
		public string NickName {get; private set;}
		public int Coin {get; private set;}
		public string AvatarId {get; private set;}
		
		public PlayerData()
		{
			
		}
		
		public PlayerData(PlayerDataResult result)
		{
			Id = result.Id;
			NickName = result.NickName;
			Coin = result.Coin;
			AvatarId = result.AvatarId;
		}
		
		public void Accept(PlayerDataResult result)
		{
			Id = result.Id;
			NickName = result.NickName;
			Coin = result.Coin;
			AvatarId = result.AvatarId;
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