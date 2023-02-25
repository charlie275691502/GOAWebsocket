using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using System;
using Web;
using Common;
using Authorization;

public class MainInstaller : MonoInstaller
{
	[SerializeField]
	private WarningView _warningView;
	[SerializeField]
	private LoadingView _loadingView;
	
	[SerializeField]
	private AuthorizationView _authorizationView;
	[SerializeField]
	private LoginView _loginView;
	[SerializeField]
	private RegisterView _registerView;
	
	public override void InstallBindings()
	{
		#region Web
		
		Container
			.Bind<IHTTPPresenter>()
			.To<HTTPPresenter>()
			.AsSingle();
			
		#endregion
		
		#region Common
		
		Container
			.Bind<IWarningPresenter>()
			.To<WarningPresenter>()
			.AsSingle();
		Container
			.Bind<IWarningView>()
			.To<WarningView>()
			.FromInstance(_warningView);
		Container
			.Bind<ILoadingPresenter>()
			.To<LoadingPresenter>()
			.AsSingle();
		Container
			.Bind<ILoadingView>()
			.To<LoadingView>()
			.FromInstance(_loadingView);
			
		#endregion
		
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
