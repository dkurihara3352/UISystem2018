using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem.PickUpUISystem{
	/* super */
		public interface IPanelTransactionState: DKUtility.ISwitchableState{}
		public abstract class AbsPanelTransactionState: IPanelTransactionState{
			public AbsPanelTransactionState(IPanelTransactionStateConstArg arg){
				thisPanel = arg.panel;
			}
			protected readonly IIconPanel thisPanel;
			public abstract void OnEnter();
			public abstract void OnExit();
		}
		public interface IEquipToolPanelTransactionState: IPanelTransactionState{}
	/* sub */
		/* WFPickUpState */
		public interface IPanelWaitingForPickUpState: IPanelTransactionState{}
		public abstract class AbsPanelWaitingForPickUpState: AbsPanelTransactionState, IPanelWaitingForPickUpState{
			public AbsPanelWaitingForPickUpState(IPanelTransactionStateConstArg arg): base(arg){}
		}
		public interface IEquipToolPanelWaitingForPickUpState: IPanelWaitingForPickUpState, IEquipToolPanelTransactionState{}
		public class EquipToolPanelWaitingForPickUpState: AbsPanelWaitingForPickUpState, IEquipToolPanelWaitingForPickUpState{
			public EquipToolPanelWaitingForPickUpState(IEquipToolPanelTransactionStateConstArg arg): base(arg){}
			IEquipToolPanel thisEqpToolPanel{
				get{return (IEquipToolPanel)thisPanel;}
			}
			public override void OnEnter(){
				thisEqpToolPanel.BecomeSelectable();
			}
			public override void OnExit(){

			}
		}
		/* hoverable */
		public interface IPanelHoverableState: IPanelTransactionState{}
		public abstract class AbsPanelHoverableState: AbsPanelTransactionState, IPanelHoverableState{
			public AbsPanelHoverableState(IPanelTransactionStateConstArg arg): base(arg){}
		}
		public interface IEquipToolPanelHoverableState: IPanelHoverableState, IEquipToolPanelTransactionState{}
		public class EquipToolPanelHoverableState: AbsPanelHoverableState, IEquipToolPanelHoverableState{
			public EquipToolPanelHoverableState(IEquipToolPanelTransactionStateConstArg arg): base(arg){}
			IEquipToolPanel thisEqpToolPanel{
				get{return (IEquipToolPanel)thisPanel;}
			}
			public override void OnEnter(){
				thisEqpToolPanel.BecomeSelectable();
			}
			public override void OnExit(){

			}
		}
		/* unhoverable */
		public interface IPanelUnhoverableState: IPanelTransactionState{}
		public abstract class AbsPanelUnhoverableState: AbsPanelTransactionState, IPanelUnhoverableState{
			public AbsPanelUnhoverableState(IPanelTransactionStateConstArg arg): base(arg){}
		}
		public interface IEquipToolPanelUnhoverableState: IPanelUnhoverableState, IEquipToolPanelTransactionState{}
		public class EquipToolPanelUnhoverableState: AbsPanelUnhoverableState, IEquipToolPanelUnhoverableState{
			public EquipToolPanelUnhoverableState(IEquipToolPanelTransactionStateConstArg arg): base(arg){}
			IEquipToolPanel thisEqpToolPanel{get{return (IEquipToolPanel)thisPanel;}}
			public override void OnEnter(){
				thisEqpToolPanel.BecomeUnselectable();
			}
			public override void OnExit(){

			}
		}
		/* hovered */
		public interface IPanelHoveredState: IPanelTransactionState{}
		public abstract class AbsPanelHoveredState: AbsPanelTransactionState, IPanelHoveredState{
			public AbsPanelHoveredState(IPanelTransactionStateConstArg arg): base(arg){}
		}
		public interface IEquipToolPanelHoveredState: IPanelHoveredState, IEquipToolPanelTransactionState{
			void SetPickedEquippableII(IEquippableItemIcon pickedEqpII);
		}
		public class EquipToolPanelHoveredState: AbsPanelHoveredState, IEquipToolPanelHoveredState{
			public EquipToolPanelHoveredState(IEquipToolPanelTransactionStateConstArg arg): base(arg){
				thisEqpIITAM = arg.eqpIITAM;
			}
			IEquippableItemIcon thisPickedEqpII;
			IEquipToolPanel thisEqpToolPanel{get{return (IEquipToolPanel)thisPanel;}}
			readonly IEquippableIITAManager thisEqpIITAM;
			public void SetPickedEquippableII(IEquippableItemIcon pickedEqpII){
				thisPickedEqpII = pickedEqpII;
			}
			public override void OnEnter(){
				thisEqpToolPanel.BecomeSelected();
				thisEqpToolPanel.CheckAndAddEmptyAddTarget(thisPickedEqpII);
				thisEqpToolPanel.HoverDefaultTransactionTargetEqpII(thisPickedEqpII);
				if(thisEqpToolPanel is IEquipToolPoolItemsPanel){
					thisEqpIITAM.SetEqpIIToUnequip(thisPickedEqpII);
				}else{
					thisEqpIITAM.SetEqpIIToEquip(thisPickedEqpII);
				}
			}
			public override void OnExit(){
				this.thisPickedEqpII = null;
				thisEqpToolPanel.CheckAndRemoveEmptyEqpIIs();
				thisEqpIITAM.ClearHoveredEqpII();
				thisEqpIITAM.ClearEqpIIsToEquipAndUnequip();
			}
		}
	/* Const */
		public interface IPanelTransactionStateConstArg{
			IIconPanel panel{get;}
		}
		public class PanelTransactionStateConstArg: IPanelTransactionStateConstArg{
			public PanelTransactionStateConstArg(IIconPanel panel){
				thisPanel = panel;
			}
			public IIconPanel panel{get{return thisPanel;}}
			readonly IIconPanel thisPanel;
		}
		public interface IEquipToolPanelTransactionStateConstArg: IPanelTransactionStateConstArg{
			IEquipToolPanel eqpToolPanel{get;}
			IEquippableIITAManager eqpIITAM{get;}
		}
		public class EquipToolPanelTransactionStateConstArg: PanelTransactionStateConstArg,IEquipToolPanelTransactionStateConstArg{
			public EquipToolPanelTransactionStateConstArg(IEquippableIITAManager eqpIITAM, IEquipToolPanel eqpToolPanel, IEquipTool eqpTool) :base(eqpToolPanel){
				thisEqpIITAM = eqpIITAM;
			}
			public IEquipToolPanel eqpToolPanel{
				get{return (IEquipToolPanel)this.panel;}
			}
			readonly IEquippableIITAManager thisEqpIITAM;
			public IEquippableIITAManager eqpIITAM{
				get{return this.thisEqpIITAM;}
			}
		}
	/*  */
}
