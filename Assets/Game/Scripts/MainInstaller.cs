using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using System;
using Web;
using Common;
using Authorization;
using Metagame;

public class MainInstaller : MonoInstaller
{
	[SerializeField]
	private WarningView _warningView;
	[SerializeField]
	private LoadingView _loadingView;
	
	[SerializeField]
	private LoginView _loginView;
	[SerializeField]
	private RegisterView _registerView;
	[SerializeField]
	private MainPageView _mainPageView;
	[SerializeField]
	private RoomView _roomView;
	[SerializeField]
	private TopMenuView _topMenuView;
	
	public override void InstallBindings()
	{
		#region Web
		
		Container
			.Bind<IHTTPPresenter>()
			.To<HTTPPresenter>()
			.AsSingle();
		
		Container
			.Bind<BackendPlayerData>()
			.AsSingle();
		Container
			.Bind<IBackendPlayerPresenter>()
			.To<BackendPlayerPresenter>()
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
			.Bind<ILoadingView>()
			.To<LoadingView>()
			.FromInstance(_loadingView);
		Container
			.Bind<IBackendPlayerView>()
			.WithId("TopMenu")
			.To<TopMenuView>()
			.FromInstance(_topMenuView);
			
		#endregion
		
		#region Authorization
		
		Container
			.Bind<IAuthorizationPresenter>()
			.To<AuthorizationPresenter>()
			.AsSingle();
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
		
		#region Metagame
		
		Container
			.Bind<IMetagamePresenter>()
			.To<MetagamePresenter>()
			.AsSingle();
		Container
			.Bind<IMainPagePresenter>()
			.To<MainPagePresenter>()
			.AsSingle();
		Container
			.Bind<IMainPageView>()
			.To<MainPageView>()
			.FromInstance(_mainPageView);
		Container
			.Bind<IRoomPresenter>()
			.To<RoomPresenter>()
			.AsSingle();
		Container
			.Bind<IRoomView>()
			.To<RoomView>()
			.FromInstance(_roomView);
		Container
			.Bind<ITopMenuView>()
			.To<TopMenuView>()
			.FromInstance(_topMenuView);
		#endregion
	}
}
