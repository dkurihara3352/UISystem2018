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
	/* picked */
	public class IIPickedState: AbsIITransactionState, IIIPickedState{
		public IIPickedState(IIITransactionStateConstArg arg): base(arg){}
		public override void OnEnter(){
			itemIcon.SetUpAsPickedII();
			iiTAM.SetToPickedState(itemIcon);
			itemIcon.BecomeVisuallyPickedUp();
			itemIcon.BecomeSelected();
		}
		public override void OnExit(){
			itemIcon.BecomeVisuallyUnpicked();
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
	/* pickable */
	public interface IIIPickableState: IIITransactionState{}
	public class EqpIIPickableState: IIIPickableState{
		public void OnEnter(){}
		public void OnExit(){}
	}
	/* unpickable */
	public interface IIIUnpickableState: IIITransactionState{}
	public class EqpIIUnpickableState: IIIUnpickableState{
		public void OnEnter(){}
		public void OnExit(){}
	}
	/* hovered */
	public interface IIIHoverableState: IIITransactionState{}
	public class EqpIIHoverableState: IIIHoverableState{
		public void OnEnter(){}
		public void OnExit(){}
	}
	/* unhoverable */
	public interface IIIUnhoverableState: IIITransactionState{}
	public class EqpIIUnhoverableState: IIIUnhoverableState{
		public void OnEnter(){}
		public void OnExit(){}
	}
	/* hovered */
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
	/* Dropped */
	public interface IIIDroppedState: IIITransactionState{}
	public class EqpIIDroppedState: AbsIITransactionState, IIIDroppedState{
		public EqpIIDroppedState(IEqpIITAStateConstArg arg): base(arg){}
		public override void OnEnter(){
			itemIcon.StopIIImageSmoothFollowDragPos();
			iiTAM.ExecuteTransaction();
			iiTAM.SetToDefaultState();
		}
		public override void OnExit(){

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
