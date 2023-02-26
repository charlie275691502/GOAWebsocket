using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Common
{
	public interface IBackendPlayerView
	{
		void UpdateNickName(string nickName);
		void UpdateCoin(int coin);
	}
}