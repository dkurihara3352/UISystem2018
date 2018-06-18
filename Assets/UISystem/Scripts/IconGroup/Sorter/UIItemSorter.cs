using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IUIItemSorter{
		void SortItems(List<IUIItem> items);
	}
}
