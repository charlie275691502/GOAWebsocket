using System.Collections;
using System.Collections.Generic;
using Rayark.Mast;
using UnityEngine;
using Authorization;
using Metagame;

public class Main : MonoBehaviour
{
	private IAuthorizationPresenter _authorizationPresenter;
	private IMetagamePresenter _metagamePresenter;
	
	private Executor _executor = new Executor();
	private bool _leave;
	
	[Zenject.Inject]
	public void Zenject(IAuthorizationPresenter authorizationPresenter, IMetagamePresenter metagamePresenter)
	{
		_authorizationPresenter = authorizationPresenter;
		_metagamePresenter = metagamePresenter;
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
		while(!_leave)
		{
			yield return _authorizationPresenter.Run();
			yield return _metagamePresenter.Run();
			yield return null;
		}
	}
}
