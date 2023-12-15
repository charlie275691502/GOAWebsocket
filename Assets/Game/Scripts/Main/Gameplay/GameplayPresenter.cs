using Common;
using Web;
using Common.Warning;
using Cysharp.Threading.Tasks;
using Main;
using Common.Class;
using Gameplay.TicTacToe;

namespace Gameplay
{
	public record GameplayState
	{
		public record TicTacToe(): GameplayState;
		public record GenerationOfAuthority(): GameplayState;
		public record Close() : GameplayState;
	}

	public record GameplayProperty(GameplayState State);

	public interface IGameplayPresenter
	{
		UniTask<MainSubTabReturn> Run(GameType gameType);
	}

	public class GameplayPresenter : IGameplayPresenter
	{
		private ITicTacToeGameplayPresenter _ticTacToeGameplayPresenter;

		public GameplayPresenter(
			ITicTacToeGameplayPresenter ticTacToeGameplayPresenter)
		{
			_ticTacToeGameplayPresenter = ticTacToeGameplayPresenter;
		}

		async UniTask<MainSubTabReturn> IGameplayPresenter.Run(GameType gameType)
		{
			// use UniTask.WhenAll() to combine gameplay and chat threads

			await _GetCurrentSubTabUniTask(gameType);
			return new MainSubTabReturn(new MainSubTabReturnType.Switch(new MainState.Metagame()));
		}

		private UniTask _GetCurrentSubTabUniTask(GameType gameType)
			=> gameType switch
			{
				GameType.TicTacToe => _ticTacToeGameplayPresenter.Run(),
				_ => throw new System.NotImplementedException(),
			};
	}
}
