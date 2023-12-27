using Common;
using Web;
using Common.Warning;
using Cysharp.Threading.Tasks;
using Main;
using Common.Class;
using Gameplay.TicTacToe;

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

		public GameplayPresenter(
			ITicTacToeGameplayPresenter ticTacToeGameplayPresenter)
		{
			_ticTacToeGameplayPresenter = ticTacToeGameplayPresenter;
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
				_ => throw new System.NotImplementedException(),
			};
	}
}
