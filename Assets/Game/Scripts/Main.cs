using System.Collections;
using System.Collections.Generic;
using Rayark.Mast;
using UnityEngine;
using Authorization;

public class Main : MonoBehaviour
{
	private IAuthorizationPresenter _authorizationPresenter;
	
	private Executor _executor = new Executor();
	private bool _leave;
	
	[Zenject.Inject]
	public void Zenject(IAuthorizationPresenter authorizationPresenter)
	{
		_authorizationPresenter = authorizationPresenter;
	}

	void Start()
	{
		_leave = false;
		_executor.Add(_Main());
	}

	void Update()
	{
		_executor.Resume(Time.deltaTime);
	}
	
	private IEnumerator _Main()
	{
		yield return _authorizationPresenter.Run();
		while(!_leave)
		{
			yield return null;
		}
	}
}
