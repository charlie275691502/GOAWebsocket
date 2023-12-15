
namespace Metagame
{
	public record MetagameSubTabReturnType
	{
		public record Switch(MetagameState State) : MetagameSubTabReturnType;
		public record Close() : MetagameSubTabReturnType;
	}

	public record MetagameSubTabReturn(MetagameSubTabReturnType Type);
}
