using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Authorization
{
	public interface IAuthorizationView
	{
		void Display();
	}
	public class AuthorizationView : MonoBehaviour, IAuthorizationView
	{
		public void Display()
		{
			Debug.Log("Display");
		}
	}
}