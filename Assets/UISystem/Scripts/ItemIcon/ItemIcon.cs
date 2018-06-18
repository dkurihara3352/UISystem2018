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
			thisPickUpImplementor = new ItemIconPickUpImplementor(this, this.iiTAM);
		}
		protected override void ActivateImple(){
			base.ActivateImple();
			InitializeTransactionState();
			InitializeEmptinessState();
		}
		protected readonly IItemIconTransactionManager iiTAM;
		/* IITransaction */
			protected IIITransactionStateEngine iiTAStateEngine;
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
				iiTAStateEngine.PickUp();
			}
			public void Drop(){
				iiTAStateEngine.Drop();
			}
			public void BecomePickable(){
				iiTAStateEngine.BecomePickable();
			}
			public void BecomeUnpickable(){
				iiTAStateEngine.BecomeUnpickable();
			}
			public bool IsPickable(){
				return iiTAStateEngine.IsPickable();
			}
			public bool IsPicked(){
				return iiTAStateEngine.IsPicked();
			}
			public void UpdateTransferableQuantity(int pickedQuantity){
				int transQ = this.CalcTransferableQuantity(pickedQuantity);
				this.thisTransferableQuantity = transQ;
			}
			int CalcTransferableQuantity(int pickedQuantity){
				return this.GetMaxTransferableQuantity() - pickedQuantity;
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
				iiTAStateEngine.WaitForPickUp();
			}
			public void BecomeHoverable(){
				iiTAStateEngine.BecomeHoverable();
			}
			public void BecomeUnhoverable(){
				iiTAStateEngine.BecomeHoverable();
			}
			public void BecomeHovered(){
				iiTAStateEngine.BecomeHovered();
			}
			public bool IsHoverable(){
				return iiTAStateEngine.IsHoverable();
			}
			public bool IsHovered(){
				return iiTAStateEngine.IsHovered();
			}
			public abstract void CheckForHover();
		/* Emptiness State Handling */
			void InitializeEmptinessState(){
				this.Disemptify(thisItem);
			}
			readonly IEmptinessStateEngine emptinessEngine;
			public bool IsEmpty(){
				return emptinessEngine.IsEmpty();
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
	}
}
