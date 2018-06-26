using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IUIItemHandler{
		IUIItem GetUIItem();
		IItemTemplate GetItemTemplate();
		int GetItemQuantity();
		bool HasSameItem(IItemIcon other);
		bool HasSameItem(IUIItem item);
		bool LeavesGhost();
		void UpdateQuantity(int souceQuantity, int targetQuantity, bool doesIncrement);
	}
}
