using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IIconGroup{
		void EvaluatePickability();
		bool AllowsInsert();
		int GetSize();
		int GetItemQuantity(IItemTemplate itemTemp);
	}
	public class IconGroup: IIconGroup{
		List<IItemIcon> itemIcons;
		public void EvaluatePickability(){
			foreach(IItemIcon ii in this.itemIcons){
				ii.EvaluatePickability();
			}
		}
		public int GetSize(){
			return itemIcons.Count;
		}
		public bool AllowsInsert(){
			return true;
		}
		public int GetItemQuantity(IItemTemplate itemTemp){
			int result = 0;
			foreach( IItemIcon ii in this.itemIcons){
				IUIItem item = ii.GetItem();
				IItemTemplate itemTempToExamine = item.GetItemTemplate();
				if(itemTempToExamine.IsSameAs(itemTemp))
					result += item.GetQuantity();
			}
			return result;
		}
	}
}
