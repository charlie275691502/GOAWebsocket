using UnityEngine;

namespace Common
{
	public interface ILoadingView
	{
		void Enter();
		void Leave();
	}
	
	public class LoadingView : MonoBehaviour, ILoadingView
	{
		[SerializeField]
		private GameObject _panel;
		
		public void Enter()
		{
			_Enter();
			_Register();
			_panel.SetActive(true);
		}
		
		public void Leave()
		{
			_Unregister();
			_panel.SetActive(false);
			_Leave();
		}
		
		private void _Enter()
		{
		}
		
		private void _Leave()
		{
		}
		
		private void _Register()
		{
		}
		
		private void _Unregister()
		{
		}
	}
}