using Common;
using Web;
using Common.Warning;
using Cysharp.Threading.Tasks;
using Main;
using Common.Class;
using Gameplay.TicTacToe;
using Gameplay.GOA;

namespace Gameplay
{
	public record GameplayParameter(IGameData GameData);

	public interface IGameplayPresenter
	{
		UniTask<MainSubTabReturn> Run(GameplayParameter parameter);
	}

	public class GameplayPresenter : IGameplayPresenter
	{
		private ITicTacToeGameplayPresenter _ticTacToeGameplayPresenter;
		private IGOAGameplayPresenter _gOAGameplayPresenter;
		

		public GameplayPresenter(
			ITicTacToeGameplayPresenter ticTacToeGameplayPresenter,
			IGOAGameplayPresenter gOAGameplayPresenter)
		{
			_ticTacToeGameplayPresenter = ticTacToeGameplayPresenter;
			_gOAGameplayPresenter = gOAGameplayPresenter;
		}

		async UniTask<MainSubTabReturn> IGameplayPresenter.Run(GameplayParameter parameter)
		{
			// use UniTask.WhenAll() to combine gameplay and chat threads

			await _GetCurrentSubTabUniTask(parameter);
			return new MainSubTabReturn(new MainSubTabReturnType.Switch(new MainState.Metagame()));
		}

		private UniTask _GetCurrentSubTabUniTask(GameplayParameter parameter)
			=> parameter.GameData switch
			{
				TicTacToeGameData => _ticTacToeGameplayPresenter.Run((TicTacToeGameData)parameter.GameData),
				GOAGameData => _gOAGameplayPresenter.Run((GOAGameData)parameter.GameData),
				_ => throw new System.NotImplementedException(),
			};
	}
}
