using Cysharp.Threading.Tasks;

public record MainSubTabReturnType
{
	public record Switch(MainState State) : MainSubTabReturnType;
	public record Close() : MainSubTabReturnType;
}

public record MainSubTabReturn(MainSubTabReturnType Type);

public interface IMainSubTabPresenter
{
	UniTask<MainSubTabReturn> Run();
}