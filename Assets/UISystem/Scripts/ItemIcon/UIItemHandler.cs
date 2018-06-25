using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IUIItemHandler{
		IUIItem GetUIItem();
		IItemTemplate GetItemTemplate();
		int GetItemQuantity();
		bool HasSameItem(IItemIcon other);
		bool LeavesGhost();
		void IncreaseBy(int quantity, bool doesIncrement);
		void DecreaseBy(int quantity, bool doesIncrement);
	}
}
