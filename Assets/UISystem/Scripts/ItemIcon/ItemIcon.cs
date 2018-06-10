using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IItemIcon: IPickableUIE, IPickUpReceiver, IEmptinessStateHandler{
		void EvaluatePickability();
		IUIItem GetItem();
		int CalcTransferableQuantity(int pickedQ);
		void IncreaseBy(int quantity, bool doesIncrement);
		void DecreaseBy(int quantity, bool doesIncrement, bool removesEmpty);
		IIconGroup GetIconGroup();
		void SetSlotID(int id);
		int GetSlotID();
		void SwapTravellingIIOnRunningTravIrperFromSelfTo(IItemIcon other);
		bool LeavesGhost();
	}
	public abstract class AbsItemIcon : AbsUIElement, IItemIcon{
		public AbsItemIcon(IUIManager uim, IUIAdaptor uia, IUIImage image, IUIItem item,IItemIconTransactionManager iiTAM): base(uim, uia, image){
			this.iiTAM = iiTAM;
			this.item = item;
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
			public void EvaluatePickability(){
				this.UpdateTransferableQuantity(0);
				if( !this.IsEmpty()){
					if( this.IsReorderable() || this.IsTransferable()){
						this.BecomePickable();
						return;
					}
				}
				this.BecomeUnpickable();
			}
			void UpdateTransferableQuantity(int pickedQuantity){
				int transQ = this.CalcTransferableQuantity(pickedQuantity);
				SetTransferableQuantity(transQ);
			}
			public int CalcTransferableQuantity(int pickedQuantity){
				return this.GetMaxTransferableQuantity() - pickedQuantity;
			}
			protected abstract int GetMaxTransferableQuantity();
			void SetTransferableQuantity(int transQ){
				this.transferableQuantity = transQ;
			}
			int transferableQuantity;
			bool IsTransferable(){
				return transferableQuantity > 0;
			}
		/* IPickableUIE */
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
			public void CheckForHover(){}
		/* Emptiness State Handling */
			void InitializeEmptinessState(){
				IUIItem item = this.GetItem();
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
			public IUIItem GetItem(){
				return item;
			}
			protected int GetQuantity(){
				return this.item.GetQuantity();
			}
			void SetQuantity(int q){
				this.item.SetQuantity(q);
			}
			IItemTemplate itemTemp{
				get{return this.item.GetItemTemplate();}
			}
		/* pick up input transmission */
			readonly IItemIconPickUpInputTransmitter inputTransmitter;
			public override void OnTouch(int touchCount){
				inputTransmitter.OnTouch(touchCount);
			}
			public override void OnDelayedTouch(){
				inputTransmitter.OnDelayedTouch();
			}
			public override void OnDrag(Vector2 pos, Vector2 deltaP){
				inputTransmitter.OnDrag(pos, deltaP);
				/* and do some smooth follow stuff */
			}
		/* Travelling */
			ITravelInterpolator runningTravelIrper;
			public ITravelInterpolator GetRunningIrper(){
				return runningTravelIrper;
			}
		/*  */
		public abstract bool LeavesGhost();
		public void DeclinePickUp(){}
		public void SwapTravellingIIOnRunningTravIrperFromSelfTo(IItemIcon other){}
		public void IncreaseBy(int quantity, bool doesIncrement){}
		public void DecreaseBy(int quantity, bool doesIncrement, bool removesEmpty){}
		
	}
}
