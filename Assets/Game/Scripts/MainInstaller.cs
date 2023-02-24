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
	[SerializeField]
	private LoginView _loginView;
	[SerializeField]
	private RegisterView _registerView;
	
	public override void InstallBindings()
	{
		#region Authorization
		
		Container
			.Bind<IAuthorizationPresenter>()
			.To<AuthorizationPresenter>()
			.AsSingle();
		Container
			.Bind<IAuthorizationView>()
			.To<AuthorizationView>()
			.FromInstance(_authorizationView);
		Container
			.Bind<ILoginPresneter>()
			.To<LoginPresenter>()
			.AsSingle();
		Container
			.Bind<ILoginView>()
			.To<LoginView>()
			.FromInstance(_loginView);
		Container
			.Bind<IRegisterPresenter>()
			.To<RegisterPresenter>()
			.AsSingle();
		Container
			.Bind<IRegisterView>()
			.To<RegisterView>()
			.FromInstance(_registerView);
			
		#endregion
	}
}
