using UnityEngine;

namespace Common
{
	public interface ISetting
	{
		bool RequestDebugMode { get; }
		bool SavePassword { get; }
		string Domain { get; }
	}
	
	public class Setting: MonoBehaviour, ISetting
	{
		public enum Environment
		{
			Dev,
			Prod,
		}
		
		[SerializeField]
		private Environment _environment;
		
		[SerializeField]
		private bool _requestDebugMode = true;
		public bool RequestDebugMode { get => _requestDebugMode; }
		
		// public bool SavePassword { get => _environment != Environment.Prod; }
		public bool SavePassword { get => true; }
		
		private static string _devDomain = "192.168.180.108:9000";
		private static string _prodDomain = "52.62.163.1:8000";
		public string Domain
		{
			get
			{
				return _environment switch
				{
					Environment.Dev => _devDomain,
					Environment.Prod => _prodDomain,
					_ => throw new System.NotImplementedException(),
				};
			}
		}
	}
}