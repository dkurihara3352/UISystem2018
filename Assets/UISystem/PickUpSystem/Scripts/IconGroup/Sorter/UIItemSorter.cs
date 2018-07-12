using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem.PickUpUISystem{
	public interface IUIItemSorter{
		void SortItems(List<IUIItem> items);
	}
}
