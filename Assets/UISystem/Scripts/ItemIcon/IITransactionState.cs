using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IIITransactionState: ISwitchableState{}
	public interface IIIPickedState: IIITransactionState{}
	public abstract class AbsIITransactionState: IIITransactionState{
		public AbsIITransactionState(IIITransactionStateConstArg arg){
			this.itemIcon = arg.itemIcon;
			this.iiTAM = arg.iiTAM;
		}
		protected readonly IItemIcon itemIcon;
		protected readonly IItemIconTransactionManager iiTAM;
		public abstract void OnEnter();
		public abstract void OnExit();
	}
	public class IIPickedState: AbsIITransactionState, IIIPickedState{
		public IIPickedState(IIITransactionStateConstArg arg): base(arg){}
		public override void OnEnter(){
			SetUpIIAsPickedII();
			iiTAM.SetToPickedState(itemIcon);
			itemIcon.BecomeVisuallyPickedUp();
			itemIcon.BecomeSelected();
		}
		void SetUpIIAsPickedII(){
			IUIItem item = itemIcon.GetUIItem();
			int pickedQuantity = itemIcon.CalcPickedQuantity();
			if(this.ShouldCreateLeftoverII(item.GetQuantity(), pickedQuantity))
				SetUpLeftoverII(pickedQuantity);
			SetUpPickedQuantity(pickedQuantity);
			LeaveUIImageBehind();
			MoveToPickUpReservePos();
			StartIIImageSmoothFollowDragPos();
		}
		bool ShouldCreateLeftoverII(int thisQuantity, int pickedQuantity){
			int leftoverQ = thisQuantity - pickedQuantity;
			if(leftoverQ > 0)
				return true;
			else
				if(this.itemIcon.LeavesGhost())
					return true;
			return false;
		}
		void SetUpPickedQuantity(int pickedQ){
			IUIItem item = itemIcon.GetUIItem();
			item.SetQuantity(0);
			itemIcon.IncreaseBy(pickedQ, doesIncrement: true);
		}
		void LeaveUIImageBehind(){
			IPickUpContextUIE pickUpContextUIE = this.iiTAM.GetPickUpContextUIE();
			IUIImage image = itemIcon.GetUIImage();
			image.DetachTo(pickUpContextUIE);
		}
		void MoveToPickUpReservePos(){
			IPickUpContextUIE pickUpContextUIE = this.iiTAM.GetPickUpContextUIE();
			Vector2 reservePos = pickUpContextUIE.GetPickUpReservePosition();
			itemIcon.SetParentUIE(pickUpContextUIE, true);
			itemIcon.SetLocalPosition(reservePos);
			itemIcon.SetSlotID(-1);
		}
		void SetUpLeftoverII(int pickedQuantity){
			IItemIcon leftoverII = CreateLeftoverII(pickedQuantity);
			leftoverII.UpdateTransferableQuantity(pickedQuantity);
			this.itemIcon.HandOverTravel(leftoverII);
		}
		IItemIcon CreateLeftoverII(int pickedQuantity){
			IUIItem item = itemIcon.GetUIItem();
			IItemIcon leftoverII = iiTAM.CreateItemIcon(item);
			IUIImage leftoverIIImage = leftoverII.GetUIImage();
			IIconGroup thisIG = itemIcon.GetIconGroup();
			leftoverII.SetParentUIE(itemIcon.GetParentUIE(), true);
			IUIImage thisIIImage = itemIcon.GetUIImage();
			leftoverIIImage.CopyPosition(thisIIImage);
			thisIG.ReplaceAndUpdateII(itemIcon.GetSlotID(), leftoverII);
			leftoverII.DisemptifyInstantly(item);
			leftoverII.DecreaseBy(pickedQuantity, doesIncrement: true);
			return leftoverII;
		}
		public override void OnExit(){
			itemIcon.BecomeVisuallyUnpicked();
		}
		void StartIIImageSmoothFollowDragPos(){

		}
	}
	public interface IEqpIITransactionState: IIITransactionState{}
	public interface IEqpIIPickedState: IIIPickedState, IEqpIITransactionState{}
	public class EqpIIPickedState: IIPickedState, IEqpIIPickedState{
		public EqpIIPickedState(IEqpIITAStateConstArg arg): base(arg){
			this.eqpTool = arg.eqpTool;
		}
		IEquippableItemIcon eqpII{
			get{return this.itemIcon as IEquippableItemIcon;}//safe
		}
		IEquippableIITAManager eqpIITAM{
			get{ return this.iiTAM as IEquippableIITAManager;}//safe
		}
		readonly IEquipTool eqpTool;
		public override void OnEnter(){
			IItemTemplate eqpItemTemp = eqpII.GetItemTemplate();
			eqpTool.TrySwitchItemMode(eqpItemTemp);
			if(this.eqpII.IsInEqpIG())
				eqpTool.TrySwitchItemFilter(eqpItemTemp);
			base.OnEnter();
		}
	}
	public interface IIIPickableState: IIITransactionState{}
	public class EqpIIPickableState: IIIPickableState{
		public void OnEnter(){}
		public void OnExit(){}
	}
	public interface IIIUnpickableState: IIITransactionState{}
	public class EqpIIUnpickableState: IIIUnpickableState{
		public void OnEnter(){}
		public void OnExit(){}
	}
	public interface IIIHoverableState: IIITransactionState{}
	public class EqpIIHoverableState: IIIHoverableState{
		public void OnEnter(){}
		public void OnExit(){}
	}
	public interface IIIUnhoverableState: IIITransactionState{}
	public class EqpIIUnhoverableState: IIIUnhoverableState{
		public void OnEnter(){}
		public void OnExit(){}
	}
	public interface IIIHoveredState: IIITransactionState{
		void SetPickedItemIcon(IItemIcon pickedII);
	}
	public class IIHoveredState: AbsIITransactionState, IIIHoveredState{
		public IIHoveredState(IIITransactionStateConstArg arg): base(arg){}
		public void SetPickedItemIcon(IItemIcon pickedII){
			this.pickedII = pickedII;
		}
		protected IItemIcon pickedII;
		public override void OnEnter(){
			this.itemIcon.BecomeSelected();
		}
		public override void OnExit(){
			itemIcon.BecomeSelectable();
			pickedII = null;
		}
	}
	public interface IEqpIIHoveredState: IIIHoveredState, IEqpIITransactionState{}
	public class EqpIIHoveredState: IIHoveredState, IEqpIIHoveredState{
		public EqpIIHoveredState(IEqpIITAStateConstArg arg): base(arg){
			this.eqpTool = arg.eqpTool;
		}
		readonly IEquipTool eqpTool;
		IEquippableItemIcon eqpII{get{return this.itemIcon as IEquippableItemIcon;}}//safe
		IEquippableIITAManager eqpIITAM{get{return this.iiTAM as IEquippableIITAManager;}}//safe
		IEquippableItemIcon pickedEqpII{
			get{
				if(this.pickedII is IEquippableItemIcon)
					return this.pickedII as IEquippableItemIcon;
				else
					throw new System.InvalidOperationException("this.pickedII must be of type IEquippableItemIcon");
			}
		}
		public override void OnEnter(){
			CheckAndSetAsTransactionTargetEqpII();
			base.OnEnter();
		}
		public override void OnExit(){
			base.OnExit();
		}
		void CheckAndSetAsTransactionTargetEqpII(){
			if(eqpII.IsInEqpIG()){
				if(eqpII.IsEmpty())
					return;
				else{
					if(eqpII.HasSameItem(pickedEqpII))
						return;
					else
						eqpIITAM.SetEqpIIToUnequip(eqpII);
				}
			}else{//in pool
				if(eqpII.HasSameItem(pickedEqpII))
					return;
				else
					eqpIITAM.SetEqpIIToEquip(eqpII);
			}
		}
	}
	/* Const */
		public interface IIITransactionStateConstArg{
			IItemIcon itemIcon{get;}
			IItemIconTransactionManager iiTAM{get;}
		}
		public interface IEqpIITAStateConstArg: IIITransactionStateConstArg{
			IEquippableItemIcon eqpII{get;}
			IEquippableIITAManager eqpIITAM{get;}
			IEquipTool eqpTool{get;}
		}
	/*  */
}
