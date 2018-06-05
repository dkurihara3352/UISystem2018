using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IIITransactionStateEngine: IPickabilityStateHandler, IHoverabilityStateHandler{}
	public class IITransactionStateEngine: AbsSwitchableStateEngine<IIITransactionState>, IIITransactionStateEngine{
		readonly IIIPickedState pickedState;
		readonly IIIPickableState pickableState;
		readonly IIIUnpickableState unpickableState;
		readonly IIIHoverableState hoverableState;
		readonly IIIUnhoverableState unhoverableState;
		readonly IIIHoveredState hoveredState;
		public bool IsPickable(){
			return curState is IIIPickedState;
		}
		public bool IsPicked(){
			return curState is IIIPickedState;
		}
		public void BecomePicked(){
			if(this.IsPickable())
				TrySwitchState(pickedState);
			else
				throw new System.InvalidOperationException("should not be picked while not pickable");
		}
		public void BecomePickable(){
			TrySwitchState(pickableState);
		}
		public void BecomeUnpickable(){
			TrySwitchState(unpickableState);
		}
		public void WaitForPickUp(){
			return;
		}
		public void BecomeHoverable(){
			TrySwitchState(hoverableState);
		}
		public void BecomeUnhoverable(){
			TrySwitchState(unhoverableState);
		}
		public void BecomeHovered(){
			TrySwitchState(hoveredState);
		}
	}
	public interface IIITransactionState: ISwitchableState{}
	public interface IIIPickedState: IIITransactionState{}
	public class IIPickedState: IIIPickedState{
		protected readonly IItemIcon itemIcon;
		readonly IItemIconTransactionManager iiTAM;
		public virtual void OnEnter(){
			itemIcon.SetUpAsPickedII();
			iiTAM.SetToPickedState(itemIcon);
		}
		void SetUpIIAsPickedII(){
			IUIItem item = itemIcon.GetItem();
			int pickedQuantity = this.CalcPickedQuantity();
			if(pickedQuantity != item.GetQuantity()){
				SetUpLeftoverII(pickedQuantity);
			}
			SetUpPickedQuantity(pickedQuantity);
			DetachAndMoveToPickUpReserve();
			StartIIImageSmoothFollowDragPos();
		}
		int CalcPickedQuantity(){
			IUIItem item = itemIcon.GetItem();
			IItemTemplate itemTemp = item.GetItemTemplate();
			int transferableQ = itemIcon.CalcTransferableQuantity(0);
			int pickUpStepQ = itemTemp.GetPickUpStepQuantity();
			if( pickUpStepQ < transferableQ)
				return pickUpStepQ;
			else
				return transferableQ;
		}
		void SetUpPickedQuantity(int pickedQ){
			IUIItem item = itemIcon.GetItem();
			item.SetQuantity(0);
			itemIcon.IncreaseBy(pickedQ, doesIncrement: true);
		}
		void DetachAndMoveToPickUpReserve(){
			IPickUpContextUIE pickUpContextUIE = this.iiTAM.GetPickUpContextUIE();
			IUIImage image = itemIcon.GetUIImage();
			Vector2 curImagePosInContextSpace = image.GetCurPosInUIESpace(pickUpContextUIE);
			image.SetParentUIE(pickUpContextUIE);
			image.SetLocalPosition(curImagePosInContextSpace);
			Vector2 reservePosInWorldSpace = pickUpContextUIE.GetPickUpReservePosInWorldSpace();
			IIconGroup ig = itemIcon.GetIconGroup();
			Vector2 reservePosInThisIGSpace = ig.GetLocalPosition(reservePosInWorldSpace);
			itemIcon.SetLocalPosition(reservePosInThisIGSpace);
			itemIcon.SetSlotID(-1);
		}
		void SetUpLeftoverII(int pickedQuantity){
			IItemIcon leftoverII = iiTAM.CreateItemIcon(itemIcon.GetItem());
			IUIImage leftoverIIImage = leftoverII.GetUIImage();
			IIconGroup thisIG = itemIcon.GetIconGroup();
			leftoverII.SetParentUIE(itemIcon.GetParentUIE());
			IUIImage thisIIImage = itemIcon.GetUIImage();
			leftoverIIImage.SetLocalPosition(thisIIImage.GetCurPosInUIESpace(itemIcon));
			thisIG.ReplaceAndUpdateII(itemIcon.GetSlotID(), leftoverII);
			leftoverII.DisemptifyInstantly();
			leftoverII.DecreaseBy(pickedQuantity, doesIncrement: true, removesEmpty: ThisItemIconIsInEqp());
			/* leaves ghost or not? */
			ITravelInterpolator runningTravelIrper = itemIcon.GetRunningTravelIrper();
			runningTravelIrper.UpdateTravellingII(leftoverII);
		}
		public void OnExit(){}
	}
	public class EqpIIPickedState: IIPickedState{
		IEquippableItemIcon eqpII{
			get{
				if(this.itemIcon is IEquippableItemIcon)
					return this.itemIcon as IEquippableItemIcon;
				else
					throw new System.InvalidCastException("wrong type is assigned in the constsructor");
			}
		}
		readonly IEquipTool eqpTool;
		public override void OnEnter(){
			IEquippableUIItem eqpItem = this.eqpII.GetItem() as IEquippableUIItem;
			IItemTemplate eqpItemTemp = eqpItem.GetItemTemplate();
			eqpTool.TrySwitchItemMode(eqpItemTemp);
			if(this.eqpII.IsInEqpIG())
				eqpTool.TrySwitchItemFilter(eqpItemTemp);
			base.OnEnter();
		}
	}
	public interface IIIPickableState: IIITransactionState{}
	public interface IIIUnpickableState: IIITransactionState{}
	public interface IIIHoverableState: IIITransactionState{}
	public interface IIIUnhoverableState: IIITransactionState{}
	public interface IIIHoveredState: IIITransactionState{}
}
