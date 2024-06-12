using System.Collections;
using System.Collections.Generic;
using Optional;
using UnityEngine;

namespace Cathei.BakingSheet
{
	public class SheetExtend<T> : Sheet<T> where T : SheetRow<string>, new()
	{
		public Option<T> GetRow(string key)
			=> string.IsNullOrEmpty(key) || !Contains(key) 
				? Option.None<T>()
				: this[key].Some();
		
		public Option<T> GetRow(int id)
			=> GetRow(id.ToString());
	}
}
