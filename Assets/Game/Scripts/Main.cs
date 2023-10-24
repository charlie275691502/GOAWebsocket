using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Authorization;
using Metagame;
using Cysharp.Threading.Tasks;

public class Main : MonoBehaviour
{
	private IAuthorizationPresenter _authorizationPresenter;
	private IMetagamePresenter _metagamePresenter;
	
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
        _ = _Main();
	}
	
	private async UniTask _Main()
	{
		while(!_leave)
		{
			await _authorizationPresenter.Run();
			await _metagamePresenter.Run();
			await UniTask.Yield();
		}
	}
}
