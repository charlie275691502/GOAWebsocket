using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Authorization.Login;
using Authorization.Register;

namespace Authorization
{
	public record AuthorizationSubTabReturnType
	{
		public record Switch(AuthorizationState State) : AuthorizationSubTabReturnType;
		public record Close() : AuthorizationSubTabReturnType;
	}

	public record AuthorizationSubTabReturn(AuthorizationSubTabReturnType Type);

	public interface IAuthorizationSubTabPresenter
	{
		UniTask<AuthorizationSubTabReturn> Run();
	}
}
