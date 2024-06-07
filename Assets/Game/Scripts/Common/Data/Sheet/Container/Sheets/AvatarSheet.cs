using Cathei.BakingSheet;

namespace Data.Sheet.Container.Sheets
{
	public class AvatarSheet : SheetExtend<AvatarSheet.AvatarRow>
    {
        public class AvatarRow : SheetRow
        {
            public string ImageKey { get; private set; }
        }
    }
}
