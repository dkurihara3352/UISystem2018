﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IIconGroup: IUIElement{
		void EvaluateAllIIsPickability();
		bool AllowsInsert();
		int GetSize();
		int GetItemQuantity(IUIItem item);
		void ReplaceAndUpdateII(int indexToReplace, IItemIcon replacingII);
		void UpdateIIs(List<IItemIcon> newIIs);
		void ActivateHoverPads();
		void DeactivateHoverPads();
		void SwapIIInAllMutations(IItemIcon sourceII, IItemIcon targetII);
		bool HasSlotSpace();
		bool HasItemSpace(IUIItem item);
		void EvaluateAllIIsHoverability(IItemIcon pickedII);
		IItemIcon GetItemIconFromItem(IUIItem item);
		List<IItemIcon> GetAllItemIconWithItemTemplate(IItemTemplate itemTemp);
		void AddEmptyAddTarget(IUIItem pickedItem);
		void RemoveEmptyIIs();
	}
	public abstract class AbsIconGroup: AbsUIElement, IIconGroup{
		public AbsIconGroup(IIconGroupConstArg arg) :base(arg){
			CheckSizeValidity(arg.minSize, arg.maxSize);
			this.minSize = arg.minSize;
			this.maxSize = arg.maxSize;
		}
		void CheckSizeValidity(int minSize, int maxSize){
			if(maxSize < 1)
				throw new System.InvalidOperationException("maxSize must be at least 1");
			else
				if(maxSize < minSize)
					throw new System.InvalidOperationException("minSize must not be greater than maxSize");
		}
		/* slots and iis */
			readonly int minSize;
			readonly int maxSize;
			public bool HasSlotSpace(){
				if(GetSize() < this.maxSize)
					return true;
				else
					if(this.HasEmptySlot())
						return true;
				return false;
			}
			protected bool HasEmptySlot(){
				foreach(IItemIcon ii in this.itemIcons){
					if(ii.IsEmpty())
						return true;
				}
				return false;
			}
			public abstract bool HasItemSpace(IUIItem item);
			public int GetSize(){
				return itemIcons.Count;
			}
			protected List<IItemIcon> itemIcons;
			public int GetItemQuantity(IUIItem item){
				int result = 0;
				foreach( IItemIcon ii in this.itemIcons){
					IUIItem thisItem = ii.GetUIItem();
					if(thisItem.IsSameAs(item))
						result += thisItem.GetQuantity();
				}
				return result;
			}
			protected List<IItemIcon> GetAllEmptyItemIcons(){
				List<IItemIcon> result = new List<IItemIcon>();
				foreach(IItemIcon ii in this.itemIcons){
					if(ii.IsEmpty())
						result.Add(ii);
				}
				return result;
			}
			public IItemIcon GetItemIconFromItem(IUIItem item){
				foreach(IItemIcon ii in this.itemIcons){
					if(ii.IsEmpty())
						continue;
					else{
						if(item.IsSameAs(ii.GetUIItem()))
							return ii;
					}
				}
				return null;
			}
			public List<IItemIcon> GetAllItemIconWithItemTemplate(IItemTemplate itemTemp){
				List<IItemIcon> result = new List<IItemIcon>();
				foreach(IItemIcon ii in this.itemIcons){
					if(ii.IsEmpty())
						continue;
					else{
						if(itemTemp.IsSameAs(ii.GetItemTemplate()))
							result.Add(ii);
					}
				}
				return result;
			}
		/*  */
		protected override void ActivateImple(){
			base.ActivateImple();
			DeactivateHoverPads();
		}
		public void EvaluateAllIIsPickability(){
			foreach(IItemIcon ii in this.itemIcons){
				ii.EvaluatePickability();
			}
		}
		public void EvaluateAllIIsHoverability(IItemIcon pickedII){
			foreach(IItemIcon ii in this.itemIcons){
				if(ii != pickedII){
					ii.EvaluateHoverability(pickedII);
				}
			}
		}
		public bool AllowsInsert(){
			return true;
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
		IHoverPadsManager hoverPadsManager;
		public void ActivateHoverPads(){
			this.hoverPadsManager.ActivateHoverPads();
		}
		public void DeactivateHoverPads(){
			this.hoverPadsManager.DeactivateHoverPads();
		}
		/* Mutation */
			List<IMutation> mutationStack;
			IMutation runningMutation{
				get{
					if(mutationStack.Count != 0){
						return mutationStack[0];
					}else
						return null;
				}
			}
			public void SwapIIInAllMutations(IItemIcon sourceII, IItemIcon targetII){
				foreach(IMutation mut in mutationStack){
					mut.FindInProspectiveIIsAndSwap(sourceII, targetII);
				}
			}
			public void AddEmptyAddTarget(IUIItem item){
				int prospectiveSlotID = this.GetProspectiveSlotID(item);
				this.AddItemAndMutate(null, prospectiveSlotID);
			}
			int GetProspectiveSlotID(IUIItem item){
				List<IUIItem> items = this.GetItems();
				items.Add(item);
				this.GetSorter().SortItems(items);
				return items.IndexOf(item);
			}

			List<IUIItem> GetItems(){
				List<IUIItem> result = new List<IUIItem>();
				foreach(IItemIcon ii in this.itemIcons){
					IUIItem item = ii.GetUIItem();
					result.Add(item);
				}
				return result;
			}
			public void RemoveEmptyIIs(){

			}
			IUIItemSorter GetSorter(){

			}
			void AddItemAndMutate(IUIItem item, int idAtAdd){
				
			}
		/*  */
	}
	public interface IIconGroupConstArg: IUIElementConstArg{
		int minSize{get;}
		int maxSize{get;}
	}
	public class IconGroupConstArg: IIconGroupConstArg{
		readonly IUIManager _uim;
		readonly IUIAdaptor _uia;
		readonly IUIImage _image;
		readonly int _minSize;
		readonly int _maxSize;
		public IconGroupConstArg(IUIManager uim, IUIAdaptor uia, IUIImage image, int minSize, int maxSize){
			this._uim = uim;
			this._uia = uia;
			this._image = image;
			this._minSize = minSize;
			this._maxSize = maxSize;
		}
		public IUIManager uim{get{return _uim;}}
		public IUIAdaptor uia{get{return _uia;}}
		public IUIImage image{get{return _image;}}
		public int minSize{get{return _minSize;}}
		public int maxSize{get{return _maxSize;}}
	}
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
			if(item is ICarriedGearTemplate){
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
