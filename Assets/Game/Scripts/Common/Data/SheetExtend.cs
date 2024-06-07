using System.Collections;
using System.Collections.Generic;
using Optional;
using UnityEngine;

namespace Cathei.BakingSheet
{
	public class SheetExtend<T> : Sheet<T> where T : SheetRow<string>, new()
	{
		public Option<T> GetRow(string id)
		{
			return
				string.IsNullOrEmpty(id) || !Contains(id) 
					? Option.None<T>()
					: this[id].Some();
		}
	}
}
