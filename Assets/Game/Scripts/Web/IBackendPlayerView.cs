using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Web
{
	public interface IBackendPlayerView
	{
		void UpdateNickName(string nickName);
		void UpdateCoin(int coin);
	}
}