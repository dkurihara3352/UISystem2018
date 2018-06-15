using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IItemIcon: IPickableUIE, IPickUpReceiver, IEmptinessStateHandler{
		void EvaluatePickability();
		void EvaluateHoverability(IItemIcon pickedII);
		IUIItem GetUIItem();
		IItemTemplate GetItemTemplate();
		int GetItemQuantity();
		bool ItemTempFamilyIsSameAs(IItemTemplate itemTemp);
		bool HasSameItem(IItemIcon other);
		int CalcTransferableQuantity(int pickedQ);
		int CalcPickedQuantity();
		bool IsTransferable();
		void IncreaseBy(int quantity, bool doesIncrement);
		void DecreaseBy(int quantity, bool doesIncrement);
		void UpdateTransferableQuantity(int pickedQuantity);
		IIconGroup GetIconGroup();
		void SetSlotID(int id);
		int GetSlotID();
		bool LeavesGhost();
		void HandOverTravel(IItemIcon other);
		void SetRunningTravelInterpolator(ITravelInterpolator irper);
	}
	public abstract class AbsItemIcon : AbsUIElement, IItemIcon{
		public AbsItemIcon(IItemIconConstArg arg): base(arg){
			this.iiTAM = arg.iiTAM;
			this.item = arg.item;
		}
		protected override void ActivateImple(){
			base.ActivateImple();
			InitializeTransactionState();
			InitializeEmptinessState();
		}
		protected readonly IItemIconTransactionManager iiTAM;
		/* IITransaction */
			readonly IIITransactionStateEngine iiTAStateEngine;
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
			public void BecomePickable(){
				iiTAStateEngine.BecomePickable();
			}
			public void BecomeUnpickable(){
				iiTAStateEngine.BecomeUnpickable();
			}
			public void BecomePicked(){
				iiTAStateEngine.BecomePicked();
			}
			public bool IsPickable(){
				return iiTAStateEngine.IsPickable();
			}
			public bool IsPicked(){
				return iiTAStateEngine.IsPicked();
			}
			public void UpdateTransferableQuantity(int pickedQuantity){
				int transQ = this.CalcTransferableQuantity(pickedQuantity);
				this.transferableQuantity = transQ;
			}
			public int CalcTransferableQuantity(int pickedQuantity){
				return this.GetMaxTransferableQuantity() - pickedQuantity;
			}
			protected abstract int GetMaxTransferableQuantity();
			int transferableQuantity;
			public bool IsTransferable(){
				return transferableQuantity > 0;
			}
		/* IPickableUIE imple */
			void CheckAndCallPickUp(int touchCount){
				if(touchCount == 1){
					CheckForImmediatePickUp();
				}else{
					if(touchCount == 2){
						CheckForSecondTouchPickUp();
					}
				}
				return;
			}
			public void PickUp(){
				this.BecomePicked();
			}
			public abstract void CheckForImmediatePickUp();
			public abstract void CheckForDelayedPickUp();
			public abstract void CheckForSecondTouchPickUp();
			public abstract void CheckForDragPickUp();
			public void BecomeVisuallyPickedUp(){}
			public void BecomeVisuallyUnpicked(){}
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
				IUIItem item = this.GetUIItem();
				this.Disemptify(item);
			}
			readonly IEmptinessStateEngine engine;
			public bool IsEmpty(){
				return engine.IsEmpty();
			}
			public void DisemptifyInstantly(IUIItem item){
			}
			public void EmptifyInstantly(){}
			public void Disemptify(IUIItem item){}
			public void Emptify(){}
		/* IG */
			protected IIconGroup iconGroup;
			public IIconGroup GetIconGroup(){
				return iconGroup;
			}
			bool IsReorderable(){
				return this.iconGroup.AllowsInsert() && this.iconGroup.GetSize() > 1;
			}
			int slotID;
			public void SetSlotID(int id){
				this.slotID = id;
			}
			public int GetSlotID(){
				return slotID;
			}
		/* Item Handling */
			protected IUIItem item;
			public IUIItem GetUIItem(){
				return item;
			}
			public int GetItemQuantity(){
				return this.item.GetQuantity();
			}
			void SetQuantity(int q){
				this.item.SetQuantity(q);
			}
			protected IItemTemplate itemTemp{
				get{return this.item.GetItemTemplate();}
			}
			public IItemTemplate GetItemTemplate(){
				return this.itemTemp;
			}
			public abstract bool ItemTempFamilyIsSameAs(IItemTemplate itemTemp);
			public abstract bool HasSameItem(IItemIcon other);
		/* input handling */
			public override void OnTouch(int touchCount){
				CheckAndCallPickUp(touchCount);
				CheckAndIncrementPickUpQuantity();
			}			
			public override void OnDelayedTouch(){
				CheckForDelayedPickUp();
			}
			public override void OnDrag(Vector2 pos, Vector2 deltaP){
				CheckForDragPickUp();
				/* and do some smooth follow stuff */
			}
		/* Travelling */
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
		/*  */
		public abstract bool LeavesGhost();
		public void DeclinePickUp(){}
		/* incrementing */
			void CheckAndIncrementPickUpQuantity(){
				if(this.iiTAM.IsInPickedUpState()){
					int incrementQuantity = CalcPickedQuantity();
					if(incrementQuantity > 0)
						this.IncrementPickUpQuantityBy(incrementQuantity);
					else
						this.DeclineIncrementPickUpQuantity();
				}
			}
			public int CalcPickedQuantity(){
				int transferableQuantity = this.transferableQuantity;
				int pickUpStepQuantity = GetItemTemplate().GetPickUpStepQuantity();
				return Mathf.Min(transferableQuantity, pickUpStepQuantity);
			}
			void IncrementPickUpQuantityBy(int increQuantity){
				IItemIcon pickedII = iiTAM.GetPickedII();
				pickedII.IncreaseBy(increQuantity, doesIncrement:true);
				this.DecreaseBy(increQuantity, doesIncrement:true);
				int newPickedUpQuantity = pickedII.GetItemQuantity();
				this.UpdateTransferableQuantity(newPickedUpQuantity);
			}
			void DeclineIncrementPickUpQuantity(){

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
