using Cathei.BakingSheet;

namespace Data.Sheet.Container
{
	using Sheets;
	public class ExcelDataSheetContainer : SheetContainerBase
	{
		public ExcelDataSheetContainer(Microsoft.Extensions.Logging.ILogger logger) : base(logger) {}
		public AvatarSheet Avatars { get; private set; }
	}
}
