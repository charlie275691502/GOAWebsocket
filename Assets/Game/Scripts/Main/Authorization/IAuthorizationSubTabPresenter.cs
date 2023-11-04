using Cysharp.Threading.Tasks;

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
