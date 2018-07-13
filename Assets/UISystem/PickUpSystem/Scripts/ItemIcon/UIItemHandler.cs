using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem.PickUpUISystem{
	public interface IUIItemHandler{
		IUIItem GetUIItem();
		void SetUIItem(IUIItem item);
		IItemTemplate GetItemTemplate();
		int GetItemQuantity();
		bool HasSameItem(IItemIcon other);
		bool HasSameItem(IUIItem item);
		bool LeavesGhost();
		void UpdateQuantity(int targetQuantity, bool doesIncrement);
		void SetQuantityInstantly(int targetQuantity);
	}
}
