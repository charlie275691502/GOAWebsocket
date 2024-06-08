using System;
using System.Collections.Generic;
using Common;
using Common.UniTaskExtension;
using Cysharp.Threading.Tasks;
using OneOf;
using OneOf.Types;
using Web;

namespace Gameplay.GOA
{
	public interface IGOAGameplayWebSocketPresenter : IWebSocketPresenter
	{
		UniTask<OneOf<None, UniTaskError>> Start(int roomId);
		UniTask<OneOf<None, UniTaskError>> ChoosePosition(int position);
		void RegisterOnUpdateGame(Action<GOAGameResult> onReceiveMessage);
		void RegisterOnReceiveSummary(Action<GOASummaryResult> onReceiveMessage);
	}
	
	public class GOAGameplayWebSocketPresenter : WebSocketPresenter, IGOAGameplayWebSocketPresenter
	{
		public GOAGameplayWebSocketPresenter(ISetting setting, BackendPlayerData backendPlayerData) : base(setting, backendPlayerData)
		{
		}

		UniTask<OneOf<None, UniTaskError>> IGOAGameplayWebSocketPresenter.Start(int roomId)
		{
			return _StartWebsocket(string.Format("GOAGame/games/{0}/", roomId.ToString()));
		}
		
		UniTask<OneOf<None, UniTaskError>> IGOAGameplayWebSocketPresenter.ChoosePosition(int position)
			=>
				_SendWaitTillReturn<None>(
					new Dictionary<string, object>()
					{
						{"command", "choose_position_action"},
						{"position", position},
					});

		void IGOAGameplayWebSocketPresenter.RegisterOnUpdateGame(Action<GOAGameResult> onReceiveMessage)
		{
			_RegisterOnReceiveMessage("update_game", onReceiveMessage);
		}
		
		void IGOAGameplayWebSocketPresenter.RegisterOnReceiveSummary(Action<GOASummaryResult> onReceiveMessage)
		{
			_RegisterOnReceiveMessage("summary", onReceiveMessage);
		}
	}
}