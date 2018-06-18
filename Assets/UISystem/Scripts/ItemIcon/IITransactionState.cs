using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IIITransactionState: ISwitchableState{}
	public interface IEqpIITransactionState: IIITransactionState{}
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
		public interface IIIPickedState: IIITransactionState{}
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
		public class IIPickableState: AbsIITransactionState, IIIPickableState{
			public IIPickableState(IIITransactionStateConstArg arg): base(arg){}
			public override void OnEnter(){}
			public override void OnExit(){}

		}
		public interface IEqpIIPickableState: IIIPickableState, IEqpIITransactionState{}
		public class EqpIIPickableState: IIPickableState, IEqpIIPickableState{
			public EqpIIPickableState(IEqpIITAStateConstArg arg): base(arg){}
			public override void OnEnter(){
				base.OnEnter();
			}
			public override void OnExit(){
				base.OnExit();
			}
		}
	/* unpickable */
		public interface IIIUnpickableState: IIITransactionState{}
		public class IIUnpickableState: AbsIITransactionState, IIIUnpickableState{
			public IIUnpickableState(IIITransactionStateConstArg arg): base(arg){}
			public override void OnEnter(){}
			public override void OnExit(){}
		}
		public interface IEqpIIUnpickableState: IIIUnpickableState, IEqpIITransactionState{}
		public class EqpIIUnpickableState: IIUnpickableState, IEqpIIUnpickableState{
			public EqpIIUnpickableState(IEqpIITAStateConstArg arg): base(arg){}
			public override void OnEnter(){base.OnEnter();}
			public override void OnExit(){base.OnExit();}
		}
	/* hovered */
		public interface IIIHoverableState: IIITransactionState{}
		public class IIHoverableState: AbsIITransactionState, IIIHoverableState{
			public IIHoverableState(IIITransactionStateConstArg arg): base(arg){}
			public override void OnEnter(){}
			public override void OnExit(){}
		}
		public interface IEqpIIHoverableState: IIIHoverableState, IEqpIITransactionState{}
		public class EqpIIHoverableState: IIHoverableState, IEqpIIHoverableState{
			public EqpIIHoverableState(IEqpIITAStateConstArg arg): base(arg){}
			public override void OnEnter(){base.OnEnter();}
			public override void OnExit(){base.OnExit();}
		}
	/* unhoverable */
		public interface IIIUnhoverableState: IIITransactionState{}
		public class IIUnhoverableState: AbsIITransactionState, IIIUnhoverableState{
			public IIUnhoverableState(IIITransactionStateConstArg arg): base(arg){}
			public override void OnEnter(){}
			public override void OnExit(){}
		}
		public interface IEqpIIUnhoverableState: IIIUnhoverableState, IEqpIITransactionState{}
		public class EqpIIUnhoverableState: IIUnhoverableState, IEqpIIUnhoverableState{
			public EqpIIUnhoverableState(IEqpIITAStateConstArg arg): base(arg){}
			public override void OnEnter(){base.OnEnter();}
			public override void OnExit(){base.OnExit();}
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
	public class IIDroppedState: AbsIITransactionState, IIIDroppedState{
		public IIDroppedState(IIITransactionStateConstArg arg): base(arg){}
		public override void OnEnter(){
			itemIcon.StopIIImageSmoothFollowDragPos();
			iiTAM.ExecuteTransaction();
			iiTAM.SetToDefaultState();
		}
		public override void OnExit(){

		}
	}
	public interface IEqpIIDroppedState: IIIDroppedState, IEqpIITransactionState{}
	public class EqpIIDroppedState: IIDroppedState, IEqpIIDroppedState{
		public EqpIIDroppedState(IEqpIITAStateConstArg arg): base(arg){}
		IEquippableIITAManager eqpIITAM{
			get{return (IEquippableIITAManager)iiTAM;}//safe
		}
		public override void OnEnter(){
			base.OnEnter();
			eqpIITAM.UpdateEquippedItems();
		}
		public override void OnExit(){

		}
	}
	/* Const */
		public interface IIITransactionStateConstArg{
			IItemIcon itemIcon{get;}
			IItemIconTransactionManager iiTAM{get;}
		}
		public class IITransactionStateConstArg: IIITransactionStateConstArg{
			public IITransactionStateConstArg(IItemIcon itemIcon, IItemIconTransactionManager iiTAM){
				thisItemIcon = itemIcon;
				thisIITAM = iiTAM;
			}
			protected IItemIcon thisItemIcon;
			public IItemIcon itemIcon{get{return thisItemIcon;}}
			protected IItemIconTransactionManager thisIITAM;
			public IItemIconTransactionManager iiTAM{get{return thisIITAM;}}
		}
		public interface IEqpIITAStateConstArg: IIITransactionStateConstArg{
			IEquippableItemIcon eqpII{get;}
			IEquippableIITAManager eqpIITAM{get;}
			IEquipTool eqpTool{get;}
		}
		public class EqpIITAStateConstArg: IITransactionStateConstArg, IEqpIITAStateConstArg{
			public EqpIITAStateConstArg(IEquippableItemIcon eqpII, IEquippableIITAManager eqpIITAM, IEquipTool eqpTool): base(eqpII, eqpIITAM){
				thisTool = eqpTool;
			}
			public IEquippableItemIcon eqpII{
				get{return (IEquippableItemIcon)thisItemIcon;}
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
