using Cathei.BakingSheet;

namespace Data.Sheet.Container
{
	using Sheets;
	public class GoogleSheetContainer : SheetContainerBase
	{
		public GoogleSheetContainer(Microsoft.Extensions.Logging.ILogger logger) : base(logger) {}
		public AvatarSheet Avatars { get; private set; }
	}
}
