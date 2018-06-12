using System.Collections;
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
		bool HasEmptySlot(){
			foreach(IItemIcon ii in this.itemIcons){
				if(ii.IsEmpty())
					return true;
			}
			return false;
		}
		public abstract bool HasItemSpace(IUIItem item);
		protected override void ActivateImple(){
			base.ActivateImple();
			DeactivateHoverPads();
		}
		List<IItemIcon> itemIcons;
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
		public int GetSize(){
			return itemIcons.Count;
		}
		public bool AllowsInsert(){
			return true;
		}
		public int GetItemQuantity(IUIItem item){
			int result = 0;
			foreach( IItemIcon ii in this.itemIcons){
				IUIItem thisItem = ii.GetUIItem();
				if(thisItem.IsSameAs(item))
					result += thisItem.GetQuantity();
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
	public interface IEqpToolIG: IIconGroup, IEquipToolElementUIE{}
	public interface IEqpToolPoolIG: IEqpToolIG{}
	public abstract class AbsEqpToolIG: AbsIconGroup, IEqpToolIG{
		public AbsEqpToolIG(IIconGroupConstArg arg): base(arg){}
		protected void CheckPassedIUIItemTypeValidity(IUIItem item){
			if(!(item is IEquippableUIItem))
				throw new System.ArgumentException("item must be of type IEquippableUIItem");
		}
	}
	public class EqpToolPoolIG: AbsEqpToolIG, IEqpToolPoolIG{
		public EqpToolPoolIG(IIconGroupConstArg arg) :base(arg){}
		public override bool HasItemSpace(IUIItem item){
			CheckPassedIUIItemTypeValidity(item);
			return true;
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
	}
}
