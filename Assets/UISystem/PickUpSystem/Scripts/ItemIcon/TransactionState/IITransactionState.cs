using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem.PickUpUISystem{
	public interface IIITransactionState: DKUtility.ISwitchableState{
		void SetItemIcon(IItemIcon itemIcon);
	}
	public interface IEqpIITransactionState: IIITransactionState{}
	public abstract class AbsIITransactionState: IIITransactionState{
		public AbsIITransactionState(IIITransactionStateConstArg arg){
			thisIITAM = arg.iiTAM;
		}
		public void SetItemIcon(IItemIcon itemIcon){
			thisItemIcon = itemIcon;
		}
		protected IItemIcon thisItemIcon;
		protected readonly IItemIconTransactionManager thisIITAM;
		public abstract void OnEnter();
		public abstract void OnExit();
	}
	/* picked */
		public interface IIIPickedState: IIITransactionState{}
		public abstract class AbsIIPickedState: AbsIITransactionState, IIIPickedState{
			public AbsIIPickedState(IIITransactionStateConstArg arg): base(arg){}
			public override void OnEnter(){
				thisItemIcon.SetUpAsPickedII();
				thisIITAM.SetToPickedState(thisItemIcon);
				thisItemIcon.BecomeVisuallyPickedUp();
				thisItemIcon.BecomeSelected();
			}
			public override void OnExit(){
				thisItemIcon.BecomeVisuallyUnpicked();
			}
		}
		public interface IEqpIIPickedState: IIIPickedState, IEqpIITransactionState{}
		public class EqpIIPickedState: AbsIIPickedState, IEqpIIPickedState{
			public EqpIIPickedState(IEqpIITAStateConstArg arg): base(arg){
				thisEqpTool = arg.eqpTool;
			}
			IEquippableItemIcon thisEqpII{
				get{return (IEquippableItemIcon)thisItemIcon;}//safe
			}
			IEquippableIITAManager thisEqpIITAM{
				get{ return (IEquippableIITAManager)thisIITAM;}//safe
			}
			readonly IEquipTool thisEqpTool;
			public override void OnEnter(){
				IItemTemplate eqpItemTemp = thisEqpII.GetItemTemplate();
				thisEqpTool.TrySwitchItemMode(eqpItemTemp);
				if(thisEqpII.IsInEqpIG())
					thisEqpTool.TrySwitchItemFilter(eqpItemTemp);
				base.OnEnter();
			}
		}
	/* pickable */
		public interface IIIPickableState: IIITransactionState{}
		public abstract class AbsIIPickableState: AbsIITransactionState, IIIPickableState{
			public AbsIIPickableState(IIITransactionStateConstArg arg): base(arg){}

		}
		public interface IEqpIIPickableState: IIIPickableState, IEqpIITransactionState{}
		public class EqpIIPickableState: AbsIIPickableState, IEqpIIPickableState{
			public EqpIIPickableState(IEqpIITAStateConstArg arg): base(arg){}
			public override void OnEnter(){
			}
			public override void OnExit(){
			}
		}
	/* unpickable */
		public interface IIIUnpickableState: IIITransactionState{}
		public abstract class AbsIIUnpickableState: AbsIITransactionState, IIIUnpickableState{
			public AbsIIUnpickableState(IIITransactionStateConstArg arg): base(arg){}
		}
		public interface IEqpIIUnpickableState: IIIUnpickableState, IEqpIITransactionState{}
		public class EqpIIUnpickableState: AbsIIUnpickableState, IEqpIIUnpickableState{
			public EqpIIUnpickableState(IEqpIITAStateConstArg arg): base(arg){}
			public override void OnEnter(){
			}
			public override void OnExit(){
			}
		}
	/* hovered */
		public interface IIIHoverableState: IIITransactionState{}
		public abstract class AbsIIHoverableState: AbsIITransactionState, IIIHoverableState{
			public AbsIIHoverableState(IIITransactionStateConstArg arg): base(arg){}
		}
		public interface IEqpIIHoverableState: IIIHoverableState, IEqpIITransactionState{}
		public class EqpIIHoverableState: AbsIIHoverableState, IEqpIIHoverableState{
			public EqpIIHoverableState(IEqpIITAStateConstArg arg): base(arg){}
			public override void OnEnter(){
			}
			public override void OnExit(){
			}
		}
	/* unhoverable */
		public interface IIIUnhoverableState: IIITransactionState{}
		public abstract class AbsIIUnhoverableState: AbsIITransactionState, IIIUnhoverableState{
			public AbsIIUnhoverableState(IIITransactionStateConstArg arg): base(arg){}
		}
		public interface IEqpIIUnhoverableState: IIIUnhoverableState, IEqpIITransactionState{}
		public class EqpIIUnhoverableState: AbsIIUnhoverableState, IEqpIIUnhoverableState{
			public EqpIIUnhoverableState(IEqpIITAStateConstArg arg): base(arg){}
			public override void OnEnter(){
			}
			public override void OnExit(){
			}
		}
	/* hovered */
	public interface IIIHoveredState: IIITransactionState{
		void SetPickedItemIcon(IItemIcon pickedII);
	}
	public abstract class AbsIIHoveredState: AbsIITransactionState, IIIHoveredState{
		public AbsIIHoveredState(IIITransactionStateConstArg arg): base(arg){}
		public void SetPickedItemIcon(IItemIcon pickedII){
			this.thisPickedII = pickedII;
		}
		protected IItemIcon thisPickedII;
		public override void OnEnter(){
			this.thisItemIcon.BecomeSelected();
		}
		public override void OnExit(){
			thisItemIcon.BecomeSelectable();
			thisPickedII = null;
		}
	}
	public interface IEqpIIHoveredState: IIIHoveredState, IEqpIITransactionState{}
	public class EqpIIHoveredState: AbsIIHoveredState, IEqpIIHoveredState{
		public EqpIIHoveredState(IEqpIITAStateConstArg arg): base(arg){
			thisEqpTool = arg.eqpTool;
		}
		readonly IEquipTool thisEqpTool;
		IEquippableItemIcon thisEqpII{get{return (IEquippableItemIcon)thisItemIcon;}}//safe
		IEquippableIITAManager thisEqpIITAM{get{return (IEquippableIITAManager)thisIITAM;}}//safe
		IEquippableItemIcon pickedEqpII{
			get{
				if(thisPickedII is IEquippableItemIcon)
					return (IEquippableItemIcon)thisPickedII;
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
			if(thisEqpII.IsInEqpIG()){
				if(thisEqpII.IsEmpty())
					return;
				else{
					if(thisEqpII.HasSameItem(pickedEqpII))
						return;
					else
						thisEqpIITAM.SetEqpIIToUnequip(thisEqpII);
				}
			}else{//in pool
				if(thisEqpII.HasSameItem(pickedEqpII))
					return;
				else
					thisEqpIITAM.SetEqpIIToEquip(thisEqpII);
			}
		}
	}
	/* Dropped */
	public interface IIIDroppedState: IIITransactionState{}
	public  abstract class AbsIIDroppedState: AbsIITransactionState, IIIDroppedState{
		public AbsIIDroppedState(IIITransactionStateConstArg arg): base(arg){}
		public override void OnEnter(){
			thisItemIcon.StopImageSmoothFollowDragPosition();
			thisIITAM.ExecuteTransaction();
			thisIITAM.SetToDefaultState();
		}
	}
	public interface IEqpIIDroppedState: IIIDroppedState, IEqpIITransactionState{}
	public class EqpIIDroppedState: AbsIIDroppedState, IEqpIIDroppedState{
		public EqpIIDroppedState(IEqpIITAStateConstArg arg): base(arg){}
		IEquippableIITAManager thisEqpIITAM{
			get{return (IEquippableIITAManager)thisIITAM;}//safe
		}
		public override void OnEnter(){
			base.OnEnter();
			thisEqpIITAM.UpdateEquippedItems();
		}
		public override void OnExit(){

		}
	}
	/* Const */
		public interface IIITransactionStateConstArg{
			IItemIconTransactionManager iiTAM{get;}
		}
		public class IITransactionStateConstArg: IIITransactionStateConstArg{
			public IITransactionStateConstArg(IItemIconTransactionManager iiTAM){
				thisIITAM = iiTAM;
			}
			protected IItemIconTransactionManager thisIITAM;
			public IItemIconTransactionManager iiTAM{get{return thisIITAM;}}
		}
		public interface IEqpIITAStateConstArg: IIITransactionStateConstArg{
			IEquippableIITAManager eqpIITAM{get;}
			IEquipTool eqpTool{get;}
		}
		public class EqpIITAStateConstArg: IITransactionStateConstArg, IEqpIITAStateConstArg{
			public EqpIITAStateConstArg(IEquippableIITAManager eqpIITAM, IEquipTool eqpTool): base(eqpIITAM){
				thisTool = eqpTool;
			}
			public IEquippableIITAManager eqpIITAM{
				get{return (IEquippableIITAManager)thisIITAM;}
			}
			IEquipTool thisTool;
			public IEquipTool eqpTool{
				get{return thisTool;}
			}
		}
	/*  */
}
