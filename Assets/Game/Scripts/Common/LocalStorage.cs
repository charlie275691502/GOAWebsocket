
using UnityEngine;

namespace Common
{
	public interface ILocalStorage
	{
		string Username {get; set; }
		string Password {get; set; }
	}
	
	public class LocalStorage : ILocalStorage
	{
		private const string USERNAME_KEY = "USERNAME";
		private const string PASSWORD_KEY = "PASSWORD";
		
		public string Username
		{
			get => PlayerPrefs.GetString(USERNAME_KEY, string.Empty);
			set => PlayerPrefs.SetString(USERNAME_KEY, value);
		}
			
		public string Password
		{
			get => PlayerPrefs.GetString(PASSWORD_KEY, string.Empty);
			set => PlayerPrefs.SetString(PASSWORD_KEY, value);
		}
	}
}