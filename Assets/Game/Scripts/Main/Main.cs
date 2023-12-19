using UnityEngine;
using Authorization;
using Metagame;
using Cysharp.Threading.Tasks;
using Data.Sheet;
using Common.Class;
using Gameplay;

namespace Main
{
	public record MainState
	{
		public record Authorization() : MainState;
		public record Metagame() : MainState;
		public record Game(GameType GameType, int GameId) : MainState;
		public record Close() : MainState;
	}

	public record MainProperty(MainState State);

	public class Main : MonoBehaviour
	{
		private IExcelDataSheetLoader _excelDataSheetLoader;
		private IAuthorizationPresenter _authorizationPresenter;
		private IMetagamePresenter _metagamePresenter;
		private IGameplayPresenter _gameplayPresenter;

		[Zenject.Inject]
		public void Zenject(
			IExcelDataSheetLoader excelDataSheetLoader,
			IAuthorizationPresenter authorizationPresenter,
			IMetagamePresenter metagamePresenter,
			IGameplayPresenter gameplayPresenter)
		{
			_excelDataSheetLoader = excelDataSheetLoader;
			_authorizationPresenter = authorizationPresenter;
			_metagamePresenter = metagamePresenter;
			_gameplayPresenter = gameplayPresenter;
		}

		void Start()
		{
			_ = _Main();
		}

		private async UniTask _Main()
		{
			await _LoadExcelData();
			var prop = new MainProperty(new MainState.Authorization());
			while (prop.State is not MainState.Close)
			{
				var subTabReturn = await _GetCurrentSubTabUniTask(prop.State);

				switch (subTabReturn.Type)
				{
					case MainSubTabReturnType.Close:
						prop = prop with { State = new MainState.Close() };
						break;
					case MainSubTabReturnType.Switch info:
						prop = prop with { State = info.State };
						break;
				}
			}
		}
		
		private async UniTask _LoadExcelData()
		{
			await _excelDataSheetLoader.Bake();
		}

		private UniTask<MainSubTabReturn> _GetCurrentSubTabUniTask(MainState type)
			=> type switch
			{
				MainState.Authorization => _authorizationPresenter.Run(),
				MainState.Metagame => _metagamePresenter.Run(),
				MainState.Game info => _gameplayPresenter.Run(new GameplayParameter(info.GameType, info.GameId)),
				_ => throw new System.NotImplementedException(),
			};
	}
}