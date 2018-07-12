using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem.PickUpUISystem{
	public interface IEquipToolIG: IIconGroup, IEquipToolElementUIE{
		IEquippableItemIcon GetDefaultTATargetEqpII(IEquippableItemIcon pickedEqpII);
	}
	public interface IEquipToolPoolIG: IEquipToolIG{}
	public abstract class AbsEqpToolIG: AbsIconGroup, IEquipToolIG{
		public AbsEqpToolIG(IIconGroupConstArg arg): base(arg){}
		protected List<IEquippableItemIcon> eqpItemIcons{
			get{
				List<IEquippableItemIcon> result = new List<IEquippableItemIcon>();
				foreach(IItemIcon ii in this.thisItemIcons)
					result.Add((IEquippableItemIcon)ii);
				return result;
			}
		}
		protected void CheckPassedIUIItemTypeValidity(IUIItem item){
			if(item is IEquippableUIItem)
				return;
			else
				throw new System.ArgumentException("item needs to be of type IEquippableUIItem");
		}
		public abstract IEquippableItemIcon GetDefaultTATargetEqpII(IEquippableItemIcon pickedEqpII);
		protected IEquippableItemIcon GetSameItemEqpII(IEquippableItemIcon sourceEqpII){
			IItemIcon iiWithItem = GetItemIconFromItem(sourceEqpII.GetUIItem());
			if(iiWithItem != null){
				return (IEquippableItemIcon)iiWithItem;
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
	public class EqpToolPoolIG: AbsEqpToolIG, IEquipToolPoolIG{
		public EqpToolPoolIG(IIconGroupConstArg arg) :base(arg){}
		public override bool HasItemSpace(IUIItem item){
			CheckPassedIUIItemTypeValidity(item);
			return true;
		}
		public override IEquippableItemIcon GetDefaultTATargetEqpII(IEquippableItemIcon pickedEqpII){
			return GetSameItemEqpII(pickedEqpII);
		}
	}
	public interface IEquipToolEquipIG : IEquipToolIG{}
	public interface IEquipToolEquippedBowIG: IEquipToolEquipIG{}
	public class EqpToolEqpBowIG: AbsEqpToolIG, IEquipToolEquippedBowIG{
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
	public interface IEquipToolEquippedWearIG: IEquipToolEquipIG{}
	public class EqpToolEqpWearIG: AbsEqpToolIG, IEquipToolEquippedWearIG{
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
	public interface IEquipToolEquippedCarriedGearsIG: IEquipToolEquipIG{}
	public class EqpToolEqpCarriedGearsIG: AbsEqpToolIG, IEquipToolEquippedCarriedGearsIG{
		public EqpToolEqpCarriedGearsIG(IIconGroupConstArg arg) :base(arg){}
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
