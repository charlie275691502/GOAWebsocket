using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using System;
using Authorization;

public class MainInstaller : MonoInstaller
{
	[SerializeField]
	private AuthorizationView _authorizationView;
	
	public override void InstallBindings()
	{
		Container
			.Bind<IAuthorizationPresenter>()
			.To<AuthorizationPresenter>()
			.AsSingle();
		Container
			.Bind<IAuthorizationView>()
			.To<AuthorizationView>()
			.FromInstance(_authorizationView);
	}
}
