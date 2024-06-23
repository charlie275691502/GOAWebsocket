using System.Collections.Generic;
using System.Linq;
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
		Empty,
		Wealth,
		Industry,
		SeaPower,
		Military,
	}

	public class GOACardsSheet : SheetExtend<GOACardsSheet.GOACardRow>
	{
		public class GOACardRow : SheetRow
		{
			public record TypePowerPair(
				PowerType PowerType,
				int Power
			);

			public string ImageKey { get; private set; }
			public string CardNameKey { get; private set; }
			public string CardDescriptionKey { get; private set; }
			public PowerType PowerType { get; private set; }
			public int Power { get; private set; }
			public CardType CardType { get; private set; }
			public PowerType RequirementPowerType1 { get; private set; }
			public int RequirementPower1 { get; private set; }
			public PowerType RequirementPowerType2 { get; private set; }
			public int RequirementPower2 { get; private set; }

			private List<TypePowerPair> _requirements;
			public List<TypePowerPair> Requirements
			{
				get => _requirements ??=
					new List<TypePowerPair>() { new TypePowerPair(RequirementPowerType1, RequirementPower1), new TypePowerPair(RequirementPowerType2, RequirementPower2) }
						.Where(pair => pair.PowerType != PowerType.Empty)
						.OrderBy(card => card.PowerType)
						.ThenBy(card => card.Power)
						.ToList();
			}
		}
	}
}
