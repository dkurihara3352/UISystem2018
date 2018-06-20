using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IItemIcon: IPickableUIE, IPickUpReceiver, IEmptinessStateHandler, IPickabilityStateHandler{
		void EvaluatePickability();
		void EvaluateHoverability(IItemIcon pickedII);
		void SetUpAsPickedII();

		IUIItem GetUIItem();
		IItemTemplate GetItemTemplate();
		int GetItemQuantity();
		bool ItemTempFamilyIsSameAs(IItemTemplate itemTemp);
		bool HasSameItem(IItemIcon other);
		bool LeavesGhost();

		bool IsTransferable();
		int GetTransferableQuantity();
		void UpdateTransferableQuantity(int pickedQuantity);

		void IncreaseBy(int quantity, bool doesIncrement);
		void DecreaseBy(int quantity, bool doesIncrement);

		IIconGroup GetIconGroup();
		void SetSlotID(int id);
		int GetSlotID();
		
		void HandOverTravel(IItemIcon other);
		void SetRunningTravelInterpolator(ITravelInterpolator irper);

		void BecomeVisuallyPickedUp();
		void BecomeVisuallyUnpicked();

		void StopIIImageSmoothFollowDragPos();

		void Immigrate(IIconGroup destIG);
		void Transfer(IIconGroup destIG);
	}
	public abstract class AbsItemIcon : AbsPickableUIE, IItemIcon{
		public AbsItemIcon(IItemIconConstArg arg): base(arg){
			this.iiTAM = arg.iiTAM;
			thisItem = arg.item;
			thisPickUpImplementor = arg.iiPickUpImplementor;
			thisPickUpImplementor.SetItemIcon(this);
			thisIITAStateEngine = arg.iiTAStateEngine;
			thisIITAStateEngine.SetItemIcon(this);
			thisEmptinessStateEngine = arg.emptinessStateEngine;
			thisEmptinessStateEngine.SetItemIcon(this);
		}
		protected override void ActivateImple(){
			base.ActivateImple();
			InitializeTransactionState();
			InitializeEmptinessState();
		}
		protected readonly IItemIconTransactionManager iiTAM;
		/* IITransaction */
			protected IItemIconTransactionStateEngine thisIITAStateEngine;
			void InitializeTransactionState(){
				WaitForPickUp();/* returns immediately in turn */
			}
		/* Pickability state handling */
			public void EvaluatePickability(){
				this.UpdateTransferableQuantity(pickedQuantity: 0);
				if( !this.IsEmpty()){
					if( this.IsReorderable() || this.IsTransferable()){
						this.BecomePickable();
						return;
					}
				}
				this.BecomeUnpickable();
			}
			public void PickUp(){
				thisIITAStateEngine.PickUp();
			}
			public void Drop(){
				thisIITAStateEngine.Drop();
			}
			public void BecomePickable(){
				thisIITAStateEngine.BecomePickable();
			}
			public void BecomeUnpickable(){
				thisIITAStateEngine.BecomeUnpickable();
			}
			public bool IsPickable(){
				return thisIITAStateEngine.IsPickable();
			}
			public bool IsPicked(){
				return thisIITAStateEngine.IsPicked();
			}
			public void UpdateTransferableQuantity(int pickedQuantity){
				int transQ = this.CalcTransferableQuantity(pickedQuantity);
				thisTransferableQuantity = transQ;
			}
			int CalcTransferableQuantity(int pickedQuantity){
				int diff = GetMaxTransferableQuantity() - pickedQuantity;
				if(diff >= 0)
					return diff;
				else
					throw new System.InvalidOperationException("pickedQuantity must not exceed max transferable quantity");
			}
			protected abstract int GetMaxTransferableQuantity();
			int thisTransferableQuantity;
			public int GetTransferableQuantity(){
				return thisTransferableQuantity;
			}
			public bool IsTransferable(){
				return thisTransferableQuantity > 0;
			}
		/*  */
			public override void DeclinePickUp(){}
			public void BecomeVisuallyPickedUp(){}
			public void BecomeVisuallyUnpicked(){}
			public void StopIIImageSmoothFollowDragPos(){}
		/* Picked Up Behaviour */
			readonly IItemIconPickUpImplementor thisPickUpImplementor;
			public void SetUpAsPickedII(){
				thisPickUpImplementor.SetUpAsPickedII();
			}
		/* Hoverability state handling */
			public void EvaluateHoverability(IItemIcon pickedII){
				if(this.IsEligibleForHover(pickedII))
					BecomeHoverable();
				else
					BecomeUnhoverable();
			}
			protected abstract bool IsEligibleForHover(IItemIcon pickedII);
			public void WaitForPickUp(){
				thisIITAStateEngine.WaitForPickUp();
			}
			public void BecomeHoverable(){
				thisIITAStateEngine.BecomeHoverable();
			}
			public void BecomeUnhoverable(){
				thisIITAStateEngine.BecomeHoverable();
			}
			public void BecomeHovered(){
				thisIITAStateEngine.BecomeHovered();
			}
			public bool IsHoverable(){
				return thisIITAStateEngine.IsHoverable();
			}
			public bool IsHovered(){
				return thisIITAStateEngine.IsHovered();
			}
			public abstract void CheckForHover();
		/* Emptiness State Handling */
			void InitializeEmptinessState(){
				this.Disemptify(thisItem);
			}
			readonly IItemIconEmptinessStateEngine thisEmptinessStateEngine;
			public bool IsEmpty(){
				return thisEmptinessStateEngine.IsEmpty();
			}
			public void DisemptifyInstantly(IUIItem item){
			}
			public void EmptifyInstantly(){}
			public void Disemptify(IUIItem item){}
			public void Emptify(){}
		/* IG */
			protected IIconGroup thisIG;
			public IIconGroup GetIconGroup(){
				return thisIG;
			}
			bool IsReorderable(){
				return thisIG.AllowsInsert() && thisIG.GetSize() > 1;
			}
			int slotID;
			public void SetSlotID(int id){
				this.slotID = id;
			}
			public int GetSlotID(){
				return slotID;
			}
		/* Item Handling */
			protected IUIItem thisItem;
			public IUIItem GetUIItem(){
				return thisItem;
			}
			public int GetItemQuantity(){
				return thisQuantity;
			}
			protected int thisQuantity{
				get{return thisItem.GetQuantity();}
			}
			void SetQuantity(int q){
				thisItem.SetQuantity(q);
			}
			protected IItemTemplate thisItemTemp{
				get{return thisItem.GetItemTemplate();}
			}
			public IItemTemplate GetItemTemplate(){
				return thisItemTemp;
			}
			public abstract bool ItemTempFamilyIsSameAs(IItemTemplate itemTemp);
			public abstract bool HasSameItem(IItemIcon other);
			public abstract bool LeavesGhost();
		/* input handling */
			public override void OnTouch(int touchCount){
				base.OnTouch(touchCount);
				thisPickUpImplementor.CheckAndIncrementPickUpQuantity();
			}
		/* Travelling */
			public void Immigrate(IIconGroup destIG){
				destIG.ReceiveImmigrant(this);
			}
			public void Transfer(IIconGroup destIG){
				destIG.ReceiveTransfer(this);
			}
			ITravelInterpolator runningTravelInterpolator;
			public void SetRunningTravelInterpolator(ITravelInterpolator irper){
				this.runningTravelInterpolator = irper;
			}
			public ITravelInterpolator GetRunningTravelInterpolator(){
				return runningTravelInterpolator;
			}
			public void HandOverTravel(IItemIcon other){
				/*  Update running travel Irper
					update mutation
					pass image trans info to other's ?
				*/
				UpdateTravelIrper(other);
				FindAndSwapIIInAllMutations(other);
			}
			void UpdateTravelIrper(IItemIcon targetII){
				if(runningTravelInterpolator != null){
					runningTravelInterpolator.UpdateTravellingII(targetII);
					SetRunningTravelInterpolator(null);
				}
			}
			void FindAndSwapIIInAllMutations(IItemIcon targetII){
				GetIconGroup().SwapIIInAllMutations(this, targetII);
			}
		public void IncreaseBy(int quantity, bool doesIncrement){}
		public void DecreaseBy(int quantity, bool doesIncrement){
			/*  does not remove resultant empty
				must be explicitly removed outside this
			*/
		}	
	}
	public interface IItemIconConstArg: IUIElementConstArg{
		IItemIconTransactionManager iiTAM{get;}
		IUIItem item{get;}
		IItemIconTransactionStateEngine iiTAStateEngine{get;}
		IItemIconPickUpImplementor iiPickUpImplementor{get;}
		IItemIconEmptinessStateEngine emptinessStateEngine{get;}
	}
	public class ItemIconConstArg: UIElementConstArg, IItemIconConstArg{
		public ItemIconConstArg(IUIManager uim, IItemIconUIAdaptor iiUIA, IUIImage image, IItemIconTransactionManager iiTAM, IUIItem item, IItemIconTransactionStateEngine iiTAStateEngine, IItemIconPickUpImplementor pickUpImplementor, IItemIconEmptinessStateEngine emptinessStateEngine): base(uim, iiUIA, image){
			thisIITAM = iiTAM;
			thisItem = item;
			thisIITAStateEngine = iiTAStateEngine;
			thisIIPickUpImplementor = iiPickUpImplementor;
			thisEmptinessStateEngine = emptinessStateEngine;
		}
		readonly IItemIconTransactionManager thisIITAM;
		public IItemIconTransactionManager iiTAM{get{return thisIITAM;}}
		readonly IUIItem thisItem;
		public IUIItem item{get{return thisItem;}}
		readonly IItemIconTransactionStateEngine thisIITAStateEngine;
		public IItemIconTransactionStateEngine iiTAStateEngine{get{return thisIITAStateEngine;}}
		readonly IItemIconPickUpImplementor thisIIPickUpImplementor;
		public IItemIconPickUpImplementor iiPickUpImplementor{get{return thisIIPickUpImplementor;}}
		readonly IItemIconEmptinessStateEngine thisEmptinessStateEngine;
		public IItemIconEmptinessStateEngine emptinessStateEngine{get{return thisEmptinessStateEngine;}}
	}
}
