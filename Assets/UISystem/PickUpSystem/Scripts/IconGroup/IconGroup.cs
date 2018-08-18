using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem.PickUpUISystem{
	public interface IIconGroup: IPickUpSystemUIElement{
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
		void ReceiveTravelTransfer(IItemIcon immmigratingII);
		void ReceiveSpotTransfer(IItemIcon transferringII);
		void SwapIIInAllMutations(IItemIcon sourceII, IItemIcon targetII);
		void RemoveIIAndMutate(IItemIcon itemIconToRemove);
	}
	public abstract class AbsIconGroup: AbsPickUpSystemUIElement, IIconGroup{
		public AbsIconGroup(IIconGroupConstArg arg) :base(arg){
			CheckSizeValidity(arg.minSize, arg.maxSize);
			thisIITAM = arg.iiTAM;
			thisMinSize = arg.minSize;
			thisMaxSize = arg.maxSize;
			thisHoverPadsManager = arg.hoverPadsManager;
			thisItemIcons = arg.iis;
		}
		void CheckSizeValidity(int minSize, int maxSize){
			if(maxSize < 1)
				throw new System.InvalidOperationException("maxSize must be at least 1");
			else
				if(maxSize < minSize)
					throw new System.InvalidOperationException("minSize must not be greater than maxSize");
		}
		protected override void OnUIActivate(){
			DeactivateHoverPads();
		}
		protected IItemIconTransactionManager thisIITAM;
		/* TA evaluation */
			public void EvaluateAllIIsPickability(){
				foreach(IItemIcon ii in this.thisItemIcons){
					ii.EvaluatePickability();
				}
			}
			public void EvaluateAllIIsHoverability(IItemIcon pickedII){
				foreach(IItemIcon ii in this.thisItemIcons){
					if(ii != pickedII){
						ii.EvaluateHoverability(pickedII);
					}
				}
			}
		/* slots and iis */
			readonly int thisMinSize;
			readonly int thisMaxSize;
			public bool HasSlotSpace(){
				if(GetSize() < thisMaxSize)
					return true;
				else
					if(this.HasEmptySlot())
						return true;
				return false;
			}
			protected List<IItemIcon> thisItemIcons;
			protected bool HasEmptySlot(){
				foreach(IItemIcon ii in thisItemIcons){
					if(ii.IsEmpty())
						return true;
				}
				return false;
			}
			public int GetSize(){
				return thisItemIcons.Count;
			}
			public int GetItemQuantity(IUIItem item){
				int result = 0;
				foreach( IItemIcon ii in thisItemIcons){
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
				foreach(IItemIcon ii in thisItemIcons){
					if(ii.IsEmpty())
						result.Add(ii);
				}
				return result;
			}
			public IItemIcon GetItemIconFromItem(IUIItem item){
				foreach(IItemIcon ii in thisItemIcons){
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
				foreach(IItemIcon ii in thisItemIcons){
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
				foreach(IItemIcon ii in thisItemIcons)
					newIIs[i++] = ii;
				newIIs[indexToReplace] = replacingII;
				UpdateIIs(newIIs);
			}
			public void UpdateIIs(List<IItemIcon> newIIs){}
			List<IUIItem> GetItems(){
				List<IUIItem> result = new List<IUIItem>();
				foreach(IItemIcon ii in thisItemIcons){
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
			readonly IHoverPadsManager thisHoverPadsManager;
			public void ActivateHoverPads(){
				this.thisHoverPadsManager.ActivateHoverPads();
			}
			public void DeactivateHoverPads(){
				this.thisHoverPadsManager.DeactivateHoverPads();
			}
		/* Mutation */
			void UpdateMutation(IReformation reformation, ITravelTransferData travelTransferData){

			}
			List<IMutation> thisMutationStack;
			IMutation thisRunningMutation{
				get{
					if(thisMutationStack.Count != 0){
						return thisMutationStack[0];
					}else
						return null;
				}
			}
			public void SwapIIInAllMutations(IItemIcon sourceII, IItemIcon targetII){
				foreach(IMutation mut in thisMutationStack){
					mut.FindInProspectiveIIsAndSwap(sourceII, targetII);
				}
			}
			public void AddEmptyAddTarget(IUIItem item){
				int prospectiveSlotID = this.GetProspectiveSlotID(item);
				this.AddIIAndMutate(null, prospectiveSlotID);
			}
			int GetProspectiveSlotID(IUIItem item){
				List<IUIItem> items = this.GetItems();
				items.Add(item);
				this.GetSorter().SortItems(items);
				return items.IndexOf(item);
			}
			void AddIIAndMutate(IUIItem item, int idAtAdd){
				IItemIcon newItemIcon = thisPickupSystemUIElementFactory.CreateItemIcon(item);
				List<IItemIcon> newIIs = CreateNewItemIconsFrom(thisItemIcons);
				newIIs.Insert(idAtAdd, newItemIcon);
				this.UpdateIIs(newIIs);
				IReformation reformation = new Reformation(iconGroup:this, newProspectiveIIs: thisItemIcons, travelTransferII: newItemIcon, iisToInit: null);
				UpdateMutation(reformation, travelTransferData: null);
			}
			public void RemoveIIAndMutate(IItemIcon itemIconToRemove){
				if(thisItemIcons.Contains(itemIconToRemove)){
					List<IItemIcon> newIIs = CreateNewItemIconsFrom(thisItemIcons);
					newIIs.Remove(itemIconToRemove);
					this.UpdateIIs(newIIs);
					IReformation reformation = new Reformation(iconGroup: this, newProspectiveIIs: newIIs, travelTransferII: null, iisToInit: null);
					UpdateMutation(reformation, travelTransferData: null);
				}else
					throw new System.InvalidOperationException("itemIconToRemove is not a memeber of thisItemIcons");
			}
			List<IItemIcon> CreateNewItemIconsFrom(List<IItemIcon> sourceIIs){
				List<IItemIcon> result = new List<IItemIcon>();
				foreach(IItemIcon ii in sourceIIs){
					result.Add(ii);
				}
				return result;
			}
			public void ReceiveTravelTransfer(IItemIcon immigratingII){
				int destSlotID = GetProspectiveSlotID(immigratingII.GetUIItem());
				this.ReceiveTravelTransferAt(immigratingII, destSlotID);
			}
			void ReceiveTravelTransferAt(IItemIcon immigratingII, int destSlotID){

			}
			public void ReceiveSpotTransfer(IItemIcon transferringII){
				IUIItem addedItem = transferringII.GetUIItem();
				int destSlotID = GetProspectiveSlotID(addedItem);
				AddIIAndMutate(addedItem, destSlotID);
			}
			public void AddItem(IUIItem item, bool doesIncrement){
				int idAtAdd = GetProspectiveSlotID(item);
				if(thisItemIcons.Count == idAtAdd)
					AddIIAndMutate(item, idAtAdd);
				else{
					IItemIcon iiAtID = thisItemIcons[idAtAdd];
					if(iiAtID.IsEmpty()){
						iiAtID.Disemptify(item);
						iiAtID.SetQuantityInstantly(0);
						iiAtID.UpdateQuantity(item.GetQuantity(), doesIncrement);
					}else{
						if(iiAtID.HasSameItem(item))
							iiAtID.IncreaseBy(item.GetQuantity(), doesIncrement);
						else
							this.AddIIAndMutate(item, idAtAdd);
					}
				}
			}
			public void RemoveItem(IUIItem item, bool doesIncrement, bool removesEmpty){
				if(item != null){
					IItemIcon thisII = GetItemIconFromItem(item);
					if(thisII != null)
						thisII.DecreaseBy(thisII.GetItemQuantity(), doesIncrement, removesEmpty);
				}
			}
		/*  */
	}
	public interface IIconGroupConstArg: IPickUpSystemUIEConstArg{
		IItemIconTransactionManager iiTAM{get;}
		int minSize{get;}
		int maxSize{get;}
		IHoverPadsManager hoverPadsManager{get;}
		List<IItemIcon> iis{get;}
	}
	public class IconGroupConstArg: PickUpSystemUIEConstArg, IIconGroupConstArg{
		public IconGroupConstArg(
			IUIManager uim, 
			IPickUpSystemProcessFactory pickUpSystemProcessFactory, 
			IPickUpSystemUIElementFactory pickUpSystemUIElementFactory, 
			IPickUpSystemUIA pickUpSyatemUIA, 
			IUIImage image, 
			IUITool tool, 
			IItemIconTransactionManager iiTAM, 
			int minSize, 
			int maxSize, 
			IHoverPadsManager hoverPadsManager, 
			List<IItemIcon> iis
		): base(
			uim, 
			pickUpSystemProcessFactory, 
			pickUpSystemUIElementFactory, 
			image, 
			pickUpSyatemUIA,
			ActivationMode.None
		){
			thisIITAM = iiTAM;
			thisMinSize = minSize;
			thisMaxSize = maxSize;
			thisHoverPadsManager = hoverPadsManager;
			thisIIs = iis;
		}
		readonly IItemIconTransactionManager thisIITAM;
		public IItemIconTransactionManager iiTAM{get{return thisIITAM;}}
		readonly int thisMinSize;
		public int minSize{get{return thisMinSize;}}
		readonly int thisMaxSize;
		public int maxSize{get{return thisMaxSize;}}
		readonly IHoverPadsManager thisHoverPadsManager;
		public IHoverPadsManager hoverPadsManager{get{return thisHoverPadsManager;}}
		readonly List<IItemIcon> thisIIs;
		public List<IItemIcon> iis{get{return thisIIs;}}
	}
}
