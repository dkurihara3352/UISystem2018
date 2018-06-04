using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IItemIcon: IPickableUIE, IPickUpReceiver, IEmptinessStateHandler{
		void EvaluatePickability();
		IUIItem GetItem();
	}
	public interface IEmptinessStateHandler{
		bool IsEmpty();
	}
	public abstract class AbsItemIcon : AbsUIElement, IItemIcon{
		public AbsItemIcon(IUIManager uim, IUIAdaptor uia, IItemIconTransactionManager iiTAM): base(uim, uia){
			this.iiTAM = iiTAM;
		}
		protected readonly IItemIconTransactionManager iiTAM;
		public override IUIImage CreateUIImage(){
			return null;
		}
		/* IITransaction */
		readonly IIITransactionStateEngine iiTAStateEngine;
		/* Pickability state handling */
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
		int CalcTransferableQuantity(int pickedQuantity){
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
		public void CheckForHover(){}
		/* Emptiness State Handling */
		readonly IEmptinessStateEngine engine;
		public bool IsEmpty(){
			return engine.IsEmpty();
		}
		/* IG */
		IIconGroup iconGroup;
		bool IsReorderable(){
			return this.iconGroup.AllowsInsert() && this.iconGroup.GetSize() > 1;
		}
		/* Item Handling */
		protected IUIItem item;
		public IUIItem GetItem(){
			return item;
		}
		protected int GetQuantity(){
			return this.item.GetQuantity();
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
		/* PickUp Imple */
		public void PickUp(){
			this.SetUpAsPickedII();
			iiTAM.SetToPickedState(this);
		}
		void SetUpAsPickedII(){
			
		}
		public void DeclinePickUp(){}
		public abstract void CheckForImmediatePickUp();
		public abstract void CheckForDelayedPickUp();
		public abstract void CheckForSecondTouchPickUp();
		public abstract void CheckForDragPickUp();
		
	}
	public interface IIITransactionStateEngine: IPickabilityStateHandler, IHoverabilityStateHandler{

	}
	public interface IEmptinessStateEngine: IEmptinessStateHandler{
	}
	public interface IEquipStateHandler{
		void Equip();
		void Unequip();
		bool IsEquipped();
	}
	public interface IEquippableUIE: IUIElement, IEquipStateHandler{
	}
	public interface IEquippableItemIcon: IItemIcon, IEquippableUIE{
	}
	public class EquippableItemIcon: AbsItemIcon, IEquippableItemIcon{
		public EquippableItemIcon(IUIManager uim, IUIAdaptor uia, IEquippableIITAManager eqpIITAM) :base(uim, uia, eqpIITAM){
		}
		IEquippableIITAManager eqpIITAM{
			get{
				return (IEquippableIITAManager)this.iiTAM;
			}
		}
		IEquippableUIItem eqpItem{
			get{
				return this.item as IEquippableUIItem;
			}
		}
		int GetMaxEquippableQuantity(){
			return this.eqpItem.GetMaxEquippableQuantity();
		}
		protected override int GetMaxTransferableQuantity(){
			IItemTemplate thisItemTemp = this.eqpItem.GetItemTemplate();
			int thisQuantity = this.GetQuantity();
			if(thisItemTemp is IBowTemplate || thisItemTemp is IWearTemplate){
				if(thisQuantity != 0)
					return 1;
				else
					return 0;
			}else{/* itemTemp is CGearsTemplate */
				if(thisItemTemp is INonStackableItemTemplate)
					return thisQuantity;
				else{
					IIconGroup relevantEqpCGIG = eqpIITAM.GetRelevantEqpCGearsIG();
					int itemQuantityInEqpCGIG = relevantEqpCGIG.GetItemQuantity(thisItemTemp);
					return this.GetMaxEquippableQuantity() - itemQuantityInEqpCGIG;
				}
			}
		}
		/* pick up imple */
		public override void CheckForImmediatePickUp(){
			return;
		}
		public override void CheckForDelayedPickUp(){
			this.CheckForPickUp();
		}
		public override void CheckForSecondTouchPickUp(){
			this.CheckForPickUp();
		}
		public override void CheckForDragPickUp(){
			return;
		}
		void CheckForPickUp(){
			if(this.IsPicked())
				return;
			else{
				if(this.IsPickable())
					this.PickUp();
				else
					this.DeclinePickUp();
			}
		}
		/* Equip imple */
		public void Equip(){}
		public void Unequip(){}
		public bool IsEquipped(){return false;}
	}
	public interface IItemIconPickUpInputTransmitter{
		void OnTouch(int touchCount);
		void OnDelayedTouch();
		void OnDrag(Vector2 dragPos, Vector2 deltaP);
	}
	public class ItemIconPickUpInputTransmitter: IItemIconPickUpInputTransmitter{
		readonly IItemIcon itemIcon;
		public void OnTouch(int touchCount){
			if(touchCount == 1){
				itemIcon.CheckForImmediatePickUp();
			}else{
				if(touchCount == 2){
					itemIcon.CheckForSecondTouchPickUp();
				}
			}
			return;
		}
		public void OnDelayedTouch(){
			itemIcon.CheckForDelayedPickUp();
		}
		public void OnDrag(Vector2 dragPos, Vector2 deltaP){
			return;
		}
	}
}
