using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IEqpToolIG: IIconGroup, IEquipToolElementUIE{
		IEquippableItemIcon GetDefaultTATargetEqpII(IEquippableItemIcon pickedEqpII);
	}
	public interface IEqpToolPoolIG: IEqpToolIG{}
	public abstract class AbsEqpToolIG: AbsIconGroup, IEqpToolIG{
		public AbsEqpToolIG(IIconGroupConstArg arg): base(arg){}
		protected List<IEquippableItemIcon> eqpItemIcons{
			get{
				List<IEquippableItemIcon> result = new List<IEquippableItemIcon>();
				foreach(IItemIcon ii in this.itemIcons){
					if(ii is IEquippableItemIcon)
						result.Add(ii as IEquippableItemIcon);
					else
						throw new System.InvalidOperationException("this.itemIcons' all member must be of type IEquippableItemIcon");
				}
				return result;
			}
		}
		protected void CheckPassedIUIItemTypeValidity(IUIItem item){
			if(!(item is IEquippableUIItem))
				throw new System.ArgumentException("item must be of type IEquippableUIItem");
		}
		public abstract IEquippableItemIcon GetDefaultTATargetEqpII(IEquippableItemIcon pickedEqpII);
		protected IEquippableItemIcon GetSameItemEqpII(IEquippableItemIcon sourceEqpII){
			IItemIcon iiWithItem = GetItemIconFromItem(sourceEqpII.GetUIItem());
			if(iiWithItem != null){
				if(iiWithItem is IEquippableItemIcon)
					return iiWithItem as IEquippableItemIcon;
				else
					throw new System.InvalidOperationException("GetSameItemEqpII returns non-IEquippableItemIcon, something's wrong");
			}
			return null;
		}
		protected List<IEquippableItemIcon> GetAllEmptyEqpIIs(){
			List<IItemIcon> emtpyIIs = GetAllEmptyItemIcons();
			List<IEquippableItemIcon> result = new List<IEquippableItemIcon>();
			foreach(IItemIcon ii in emtpyIIs){
				if(ii is IEquippableItemIcon)
					result.Add(ii as IEquippableItemIcon);
				else
					throw new System.InvalidOperationException("all member of this.GetAllEmptyIIs must be of type IEquippableItemIcon");
			}
			return result;
		}
	}
	public class EqpToolPoolIG: AbsEqpToolIG, IEqpToolPoolIG{
		public EqpToolPoolIG(IIconGroupConstArg arg) :base(arg){}
		public override bool HasItemSpace(IUIItem item){
			CheckPassedIUIItemTypeValidity(item);
			return true;
		}
		public override IEquippableItemIcon GetDefaultTATargetEqpII(IEquippableItemIcon pickedEqpII){
			return GetSameItemEqpII(pickedEqpII);
		}
	}
	public interface IEqpToolEqpIG<T> : IEqpToolIG where T: IItemTemplate{}
	public class EqpToolEqpBowIG: AbsEqpToolIG, IEqpToolEqpIG<IBowTemplate>{
		public EqpToolEqpBowIG(IIconGroupConstArg arg) :base(arg){}
		public override bool HasItemSpace(IUIItem item){
			CheckPassedIUIItemTypeValidity(item);
			IEquippableUIItem eqpItem = item as IEquippableUIItem;//safe
			IItemTemplate itemTemp = eqpItem.GetItemTemplate();
			if(itemTemp is IBowTemplate && !eqpItem.IsEquipped())
				return true;
			else
				return false;
		}
		public override IEquippableItemIcon GetDefaultTATargetEqpII(IEquippableItemIcon pickedEqpII){
			return GetEquippedBow();
		}
		IEquippableItemIcon GetEquippedBow(){
			return this.eqpItemIcons[0];
		}
	}
	public class EqpToolEqpWearIG: AbsEqpToolIG, IEqpToolEqpIG<IWearTemplate>{
		public EqpToolEqpWearIG(IIconGroupConstArg arg) :base(arg){}
		public override bool HasItemSpace(IUIItem item){
			CheckPassedIUIItemTypeValidity(item);
			IEquippableUIItem eqpItem = item as IEquippableUIItem;//safe
			IItemTemplate itemTemp = eqpItem.GetItemTemplate();
			if(itemTemp is IWearTemplate && !eqpItem.IsEquipped())
				return true;
			else
				return false;
		}
		public override IEquippableItemIcon GetDefaultTATargetEqpII(IEquippableItemIcon pickedEqpII){
			return GetEquippedWear();
		}
		IEquippableItemIcon GetEquippedWear(){
			return this.eqpItemIcons[0];
		}
	}
	public class EqpToolEqpCarriedGearIG: AbsEqpToolIG, IEqpToolEqpIG<ICarriedGearTemplate>{
		public EqpToolEqpCarriedGearIG(IIconGroupConstArg arg) :base(arg){}
		public override bool HasItemSpace(IUIItem item){
			CheckPassedIUIItemTypeValidity(item);
			IEquippableUIItem eqpItem = item as IEquippableUIItem;//safe
			IItemTemplate eqpItemTemp = eqpItem.GetItemTemplate();
			if(eqpItemTemp is ICarriedGearTemplate){
				int maxEquippableQuantity = eqpItem.GetMaxEquippableQuantity();
				int equippedQuantity = this.GetItemQuantity(eqpItem);
				int space = maxEquippableQuantity - equippedQuantity;
				return space > 0;
			}else
				return false;
		}
		public override IEquippableItemIcon GetDefaultTATargetEqpII(IEquippableItemIcon pickedEqpII){
			IEquippableItemIcon sameItemEqpII = GetSameItemEqpII(pickedEqpII);
			if(sameItemEqpII != null)
				return sameItemEqpII;
			else{
				if(this.HasEmptySlot())
					return GetFirstEmptyEqpII();
				else
					return null;
			}
		}
		IEquippableItemIcon GetFirstEmptyEqpII(){
			List<IEquippableItemIcon> emptyItemIcons = GetAllEmptyEqpIIs();
			if(emptyItemIcons.Count > 0){
				return emptyItemIcons[0];
			}else
				throw new System.InvalidOperationException("emptyItemIcons.Count must be at least 1");
		}
	}
}
