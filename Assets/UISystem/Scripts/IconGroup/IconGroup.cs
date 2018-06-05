using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IIconGroup: IUIElement{
		void EvaluatePickability();
		bool AllowsInsert();
		int GetSize();
		int GetItemQuantity(IItemTemplate itemTemp);
		void ReplaceAndUpdateII(int indexToReplace, IItemIcon replacingII);
		void UpdateIIs(List<IItemIcon> newIIs);
	}
	public abstract class AbsIconGroup: AbsUIElement, IIconGroup{
		public AbsIconGroup(IUIManager uim, IUIAdaptor uia, IUIImage image) :base(uim, uia, image){}
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
		public void ReplaceAndUpdateII(int indexToReplace, IItemIcon replacingII){
			List<IItemIcon> newIIs = new List<IItemIcon>();
			int i = 0;
			foreach(IItemIcon ii in this.itemIcons)
				newIIs[i++] = ii;
			newIIs[indexToReplace] = replacingII;
			UpdateIIs(newIIs);
		}
		public void UpdateIIs(List<IItemIcon> newIIs){}
	}
	public interface IEqpToolIG: IIconGroup{}
	public interface IEqpToolPoolIG: IEqpToolIG{}
	public class EqpToolPoolIG: AbsIconGroup, IEqpToolPoolIG{
		public EqpToolPoolIG(IUIManager uim, IUIAdaptor uia, IUIImage image) :base(uim, uia, image){}
	}
	public interface IEqpToolEqpIG<T> : IEqpToolIG where T: IItemTemplate{}
	public class EqpToolEqpBowIG: AbsIconGroup, IEqpToolEqpIG<IBowTemplate>{
		public EqpToolEqpBowIG(IUIManager uim, IUIAdaptor uia, IUIImage image) :base(uim, uia, image){}
	}
	public class EqpToolEqpWearIG: AbsIconGroup, IEqpToolEqpIG<IWearTemplate>{
		public EqpToolEqpWearIG(IUIManager uim, IUIAdaptor uia, IUIImage image) :base(uim, uia, image){}
	}
	public class EqpToolEqpCarriedGearIG: AbsIconGroup, IEqpToolEqpIG<ICarriedGearTemplate>{
		public EqpToolEqpCarriedGearIG(IUIManager uim, IUIAdaptor uia, IUIImage image) :base(uim, uia, image){}
	}
}
