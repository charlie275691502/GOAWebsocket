using Cathei.BakingSheet;

namespace Data.Sheet.Container.Sheets
{
	public class GOACardsSheet : SheetExtend<GOACardsSheet.GOACardRow>
    {
        public class GOACardRow : SheetRow
        {
            public string ImageKey { get; private set; }
            public int Power { get; private set; }
        }
    }
}
