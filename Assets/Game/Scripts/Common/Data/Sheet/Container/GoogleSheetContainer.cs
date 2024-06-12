using Cathei.BakingSheet;

namespace Data.Sheet.Container
{
	using Sheets;
	public class GoogleSheetContainer : SheetContainerBase
	{
		public GoogleSheetContainer(Microsoft.Extensions.Logging.ILogger logger) : base(logger) {}
		public AvatarsSheet Avatars { get; private set; }
		public GOACharactersSheet GOACharacters {get; private set;}
		public GOACardsSheet GOACards {get; private set;}
	}
}
