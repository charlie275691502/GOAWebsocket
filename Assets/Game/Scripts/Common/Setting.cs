using UnityEngine;

namespace Common
{
	public interface ISetting
	{
		bool RequestDebugMode { get; }
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
		private bool _requestDebugMode = true;
		public bool RequestDebugMode { get{ return _requestDebugMode;} }
		
		[SerializeField]
		private Environment _environment;
		
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