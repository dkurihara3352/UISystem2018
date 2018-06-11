using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IIconGroup: IUIElement{
		void EvaluateAllIIsPickability();
		bool AllowsInsert();
		int GetSize();
		int GetItemQuantity(IItemTemplate itemTemp);
		void ReplaceAndUpdateII(int indexToReplace, IItemIcon replacingII);
		void UpdateIIs(List<IItemIcon> newIIs);
		void ActivateHoverPads();
		void DeactivateHoverPads();
		void SwapIIInAllMutations(IItemIcon sourceII, IItemIcon targetII);
		bool HasSlotSpace();
		void EvaluateAllIIsHoverability(IItemIcon pickedII);
	}
	public abstract class AbsIconGroup: AbsUIElement, IIconGroup{
		public AbsIconGroup(IUIManager uim, IUIAdaptor uia, IUIImage image, int minSize, int maxSize) :base(uim, uia, image){
			CheckSizeValidity(minSize, maxSize);
			this.minSize = minSize;
			this.maxSize = maxSize;
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
		public int GetItemQuantity(IItemTemplate itemTemp){
			int result = 0;
			foreach( IItemIcon ii in this.itemIcons){
				IUIItem item = ii.GetUIItem();
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
	public interface IEqpToolIG: IIconGroup, IEquipToolElementUIE{}
	public interface IEqpToolPoolIG: IEqpToolIG{}
	public class EqpToolPoolIG: AbsIconGroup, IEqpToolPoolIG{
		public EqpToolPoolIG(IUIManager uim, IUIAdaptor uia, IUIImage image, int minSize, int maxSize) :base(uim, uia, image, minSize, maxSize){}
	}
	public interface IEqpToolEqpIG<T> : IEqpToolIG where T: IItemTemplate{}
	public class EqpToolEqpBowIG: AbsIconGroup, IEqpToolEqpIG<IBowTemplate>{
		public EqpToolEqpBowIG(IUIManager uim, IUIAdaptor uia, IUIImage image, int minSize, int maxSize) :base(uim, uia, image, minSize, maxSize){}
	}
	public class EqpToolEqpWearIG: AbsIconGroup, IEqpToolEqpIG<IWearTemplate>{
		public EqpToolEqpWearIG(IUIManager uim, IUIAdaptor uia, IUIImage image, int minSize, int maxSize) :base(uim, uia, image, minSize, maxSize){}
	}
	public class EqpToolEqpCarriedGearIG: AbsIconGroup, IEqpToolEqpIG<ICarriedGearTemplate>{
		public EqpToolEqpCarriedGearIG(IUIManager uim, IUIAdaptor uia, IUIImage image, int minSize, int maxSize) :base(uim, uia, image, minSize, maxSize){}
	}
}
