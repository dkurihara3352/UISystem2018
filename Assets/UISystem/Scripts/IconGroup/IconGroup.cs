using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IIconGroup: IUIElement{
		void EvaluateAllIIsPickability();
		void EvaluateAllIIsHoverability(IItemIcon pickedII);

		bool AllowsInsert();
		int GetSize();
		int GetItemQuantity(IUIItem item);
		void UpdateIIs(List<IItemIcon> newIIs);
		void ReplaceAndUpdateII(int indexToReplace, IItemIcon replacingII);
		bool HasSlotSpace();
		bool HasItemSpace(IUIItem item);
		IItemIcon GetItemIconFromItem(IUIItem item);
		List<IItemIcon> GetAllItemIconWithItemTemplate(IItemTemplate itemTemp);
		
		void ActivateHoverPads();
		void DeactivateHoverPads();

		void AddEmptyAddTarget(IUIItem pickedItem);
		void RemoveEmptyIIs();
		void ReceiveImmigrant(IItemIcon immmigratingII);
		void ReceiveTransfer(IItemIcon transferringII);
		void SwapIIInAllMutations(IItemIcon sourceII, IItemIcon targetII);
	}
	public abstract class AbsIconGroup: AbsUIElement, IIconGroup{
		public AbsIconGroup(IIconGroupConstArg arg) :base(arg){
			CheckSizeValidity(arg.minSize, arg.maxSize);
			this.minSize = arg.minSize;
			this.maxSize = arg.maxSize;
			this.hoverPadsManager = arg.hoverPadsManager;
			this.itemIcons = arg.iis;
		}
		void CheckSizeValidity(int minSize, int maxSize){
			if(maxSize < 1)
				throw new System.InvalidOperationException("maxSize must be at least 1");
			else
				if(maxSize < minSize)
					throw new System.InvalidOperationException("minSize must not be greater than maxSize");
		}
		protected override void ActivateImple(){
			base.ActivateImple();
			DeactivateHoverPads();
		}
		/* TA evaluation */
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
			protected List<IItemIcon> itemIcons;
			protected bool HasEmptySlot(){
				foreach(IItemIcon ii in this.itemIcons){
					if(ii.IsEmpty())
						return true;
				}
				return false;
			}
			public int GetSize(){
				return itemIcons.Count;
			}
			public int GetItemQuantity(IUIItem item){
				int result = 0;
				foreach( IItemIcon ii in this.itemIcons){
					if(ii.IsEmpty())
						continue;
					else{
						IUIItem thisItem = ii.GetUIItem();
						if(thisItem.IsSameAs(item))
							result += thisItem.GetQuantity();
					}
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
						IUIItem thisItem = ii.GetUIItem();
						if(thisItem.IsSameAs(item))
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
						IItemTemplate thisItemTemp = ii.GetItemTemplate();
						if(thisItemTemp.IsSameAs(itemTemp))
							result.Add(ii);
					}
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
			public abstract bool HasItemSpace(IUIItem item);
		/* Sorting */
			IUIItemSorter GetSorter(){
				return null;
			}
			public bool AllowsInsert(){
				return true;
			}
		/* hover pads */
			readonly IHoverPadsManager hoverPadsManager;
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
			void AddItemAndMutate(IUIItem item, int idAtAdd){	
			}
			public void ReceiveImmigrant(IItemIcon immigratingII){
				int destSlotID = GetProspectiveSlotID(immigratingII.GetUIItem());
				this.ReceiveImmigrantAt(immigratingII, destSlotID);
			}
			void ReceiveImmigrantAt(IItemIcon immigratingII, int destSlotID){

			}
			public void ReceiveTransfer(IItemIcon transferringII){
				IUIItem addedItem = transferringII.GetUIItem();
				int destSlotID = GetProspectiveSlotID(addedItem);
				AddItemAndMutate(addedItem, destSlotID);
			}
		/*  */
	}
	public interface IIconGroupConstArg: IUIElementConstArg{
		int minSize{get;}
		int maxSize{get;}
		IHoverPadsManager hoverPadsManager{get;}
		List<IItemIcon> iis{get;}
	}
	public class IconGroupConstArg: UIElementConstArg, IIconGroupConstArg{
		public IconGroupConstArg(IUIManager uim, IUIAdaptor uia, IUIImage image, int minSize, int maxSize, IHoverPadsManager hoverPadsManager, List<IItemIcon> iis): base(uim, uia, image){
			thisMinSize = minSize;
			thisMaxSize = maxSize;
			thisHoverPadsManager = hoverPadsManager;
			thisIIs = iis;
		}

		readonly int thisMinSize;
		readonly int thisMaxSize;
		readonly IHoverPadsManager thisHoverPadsManager;
		public int minSize{get{return thisMinSize;}}
		public int maxSize{get{return thisMaxSize;}}
		public IHoverPadsManager hoverPadsManager{get{return thisHoverPadsManager;}}
		readonly List<IItemIcon> thisIIs;
		public List<IItemIcon> iis{get{return thisIIs;}}
	}
}
