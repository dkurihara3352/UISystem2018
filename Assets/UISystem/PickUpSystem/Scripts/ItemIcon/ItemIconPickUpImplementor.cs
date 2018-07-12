using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem.PickUpUISystem{
	public interface IItemIconPickUpImplementor{
		void SetItemIcon(IItemIcon itemIcon);
		void CheckAndIncrementPickUpQuantity();
		void SetUpAsPickedII();
	}
	public class ItemIconPickUpImplementor: IItemIconPickUpImplementor{
		public ItemIconPickUpImplementor(IItemIconTransactionManager iiTAM, IPickUpSystemUIElementFactory pickUpSystemUIElementFactory){
			thisIITAM = iiTAM;
			thisPickUpSystemUIElementFactory = pickUpSystemUIElementFactory;
		}
		readonly IPickUpSystemUIElementFactory thisPickUpSystemUIElementFactory;
		public void SetItemIcon(IItemIcon itemIcon){
			thisItemIcon = itemIcon;
		}
		IItemIcon thisItemIcon;
		readonly IItemIconTransactionManager thisIITAM;
		IPickableUIImage thisImage{
			get{return thisItemIcon.GetPickableUIImage();}
		}
		IUIItem thisItem{
			get{return thisItemIcon.GetUIItem();}
		}
		IItemTemplate thisItemTemp{
			get{return thisItem.GetItemTemplate();}
		}
		int thisTransferableQuantity{
			get{return thisItemIcon.GetTransferableQuantity();}
		}
		int thisQuantity{
			get{return thisItemIcon.GetItemQuantity();}
		}
		IIconGroup thisIG{
			get{return thisItemIcon.GetIconGroup();}
		}
		public void SetUpAsPickedII(){
			int pickedQuantity = this.CalcPickedQuantity();
			IPickUpContextUIE pickUpContextUIE = thisIITAM.GetPickUpContextUIE();

			if(this.ShouldCreateLeftoverII(pickedQuantity))
				SetUpLeftoverII(pickedQuantity);
			SetUpPickedQuantity(pickedQuantity);
			thisImage.DetachTo(pickUpContextUIE);
			this.MoveToPickUpReservePos();
			this.SlotOut();
			thisItemIcon.StartImageSmoothFollowDragPosition();
		}
		int CalcPickedQuantity(){
			int pickUpStepQuantity = thisItemTemp.GetPickUpStepQuantity();
			return Mathf.Min(thisTransferableQuantity, pickUpStepQuantity);
		}
		bool ShouldCreateLeftoverII(int pickedQuantity){
			int leftoverQ = thisQuantity - pickedQuantity;
			if(leftoverQ > 0)
				return true;
			else
				if(thisItemIcon.LeavesGhost())
					return true;
			return false;
		}
		void SetUpPickedQuantity(int pickedQ){
			thisItem.SetQuantity(0);
			thisItemIcon.IncreaseBy(pickedQ, doesIncrement: true);
		}
		void MoveToPickUpReservePos(){
			IPickUpContextUIE pickUpContextUIE = this.thisIITAM.GetPickUpContextUIE();
			Vector2 reservePos = pickUpContextUIE.GetPickUpReservePosition();
			thisItemIcon.SetParentUIE(pickUpContextUIE, true);
			thisItemIcon.SetLocalPosition(reservePos);
		}
		void SlotOut(){
			thisItemIcon.SetSlotID(-1);
		}
		void SetUpLeftoverII(int pickedQuantity){
			IItemIcon leftoverII = CreateLeftoverII(pickedQuantity);
			leftoverII.UpdateTransferableQuantity(pickedQuantity);
			thisItemIcon.HandOverTravel(leftoverII);
		}
		IItemIcon CreateLeftoverII(int pickedQuantity){
			IItemIcon leftoverII = thisPickUpSystemUIElementFactory.CreateItemIcon(thisItem);
			IUIImage leftoverIIImage = leftoverII.GetUIImage();

			leftoverII.SetParentUIE(thisItemIcon.GetParentUIE(), true);
			leftoverIIImage.CopyPosition(thisImage);
			thisIG.ReplaceAndUpdateII(thisItemIcon.GetSlotID(), leftoverII);
			leftoverII.DisemptifyInstantly(thisItem);
			leftoverII.DecreaseBy(pickedQuantity, doesIncrement: true, removesEmpty: false);

			return leftoverII;
		}
		public void CheckAndIncrementPickUpQuantity(){
			if(thisIITAM.IsInPickedUpState()){
				int incrementQuantity = CalcPickedQuantity();
				if(incrementQuantity > 0)
					this.IncrementPickUpQuantityBy(incrementQuantity);
				else
					this.DeclineIncrementPickUpQuantity();
			}
		}
		void IncrementPickUpQuantityBy(int increQuantity){
			IItemIcon pickedII = thisIITAM.GetPickedII();
			pickedII.IncreaseBy(increQuantity, doesIncrement:true);
			thisItemIcon.DecreaseBy(increQuantity, doesIncrement:true, removesEmpty:false);
			int newPickedUpQuantity = pickedII.GetItemQuantity();
			thisItemIcon.UpdateTransferableQuantity(newPickedUpQuantity);
		}
		void DeclineIncrementPickUpQuantity(){
		}
	}
}
