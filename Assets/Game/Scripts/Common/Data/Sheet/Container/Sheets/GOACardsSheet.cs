using Cathei.BakingSheet;

namespace Data.Sheet.Container.Sheets
{
	public enum CardType
	{
		Power,
		ActionMask,
		ActionReform,
		ActionExpand,
		Strategy,
	}
	
	public enum PowerType
	{
		Wealth,
		Industry,
		SeaPower,
		Military,
	}
	
	public class GOACardsSheet : SheetExtend<GOACardsSheet.GOACardRow>
	{
		public class GOACardRow : SheetRow
		{
			public string ImageKey { get; private set; }
			public PowerType PowerType { get; private set; }
			public int Power { get; private set; }
			public CardType CardType { get; private set; }
		}
	}
}
