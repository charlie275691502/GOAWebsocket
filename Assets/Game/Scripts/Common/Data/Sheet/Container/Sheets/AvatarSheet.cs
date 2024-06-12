using Cathei.BakingSheet;

namespace Data.Sheet.Container.Sheets
{
	public class AvatarsSheet : SheetExtend<AvatarsSheet.AvatarRow>
    {
        public class AvatarRow : SheetRow
        {
            public string ImageKey { get; private set; }
        }
    }
}
