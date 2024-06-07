using Cathei.BakingSheet;

namespace Data.Sheet.Container.Sheets
{
	public class GOACharactersSheet : SheetExtend<GOACharactersSheet.GOACharacterRow>
    {
        public class GOACharacterRow : SheetRow
        {
            public string ImageKey { get; private set; }
            public string NameKey { get; private set; }
            public string SkillKey { get; private set; }
            public string SkillDescriptionKey { get; private set; }
        }
    }
}
