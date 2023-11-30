using UnityEngine;
using Authorization;
using Metagame;
using Cysharp.Threading.Tasks;

namespace Main
{
	public record MainState
	{
		public record Authorization() : MainState;
		public record Metagame() : MainState;
		public record Game() : MainState;
		public record Close() : MainState;
	}

	public record MainProperty(MainState State);

	public class Main : MonoBehaviour
	{
		private IAuthorizationPresenter _authorizationPresenter;
		private IMetagamePresenter _metagamePresenter;

		[Zenject.Inject]
		public void Zenject(IAuthorizationPresenter authorizationPresenter, IMetagamePresenter metagamePresenter)
		{
			_authorizationPresenter = authorizationPresenter;
			_metagamePresenter = metagamePresenter;
		}

		void Start()
		{
			_ = _Main();
		}

		private async UniTask _Main()
		{
			var prop = new MainProperty(new MainState.Authorization());
			while (prop.State is not MainState.Close)
			{
				var subTabReturn = await _GetCurrentSubTabPresenter(prop.State).Run();

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

		private IMainSubTabPresenter _GetCurrentSubTabPresenter(MainState type)
			=> type switch
			{
				MainState.Authorization => _authorizationPresenter,
				MainState.Metagame => _metagamePresenter,
				_ => null
			};
	}
}