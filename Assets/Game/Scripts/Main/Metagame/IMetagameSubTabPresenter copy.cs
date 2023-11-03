using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;
using Web;
using Common.Warning;
using Cysharp.Threading.Tasks;
using Common.UniTaskExtension;
using Metagame.MainPage;
using Metagame.Room;

namespace Metagame
{
	public record MetagameSubTabReturnType
	{
		public record Switch(MetagameState State) : MetagameSubTabReturnType;
		public record Close() : MetagameSubTabReturnType;
	}

	public record MetagameSubTabReturn(MetagameSubTabReturnType Type);
}
