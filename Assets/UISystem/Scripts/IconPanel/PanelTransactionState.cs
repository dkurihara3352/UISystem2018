using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	/* super */
		public interface IPanelTransactionState: ISwitchableState{}
		public abstract class AbsPanelTransactionState: IPanelTransactionState{
			public AbsPanelTransactionState(IPanelTransactionStateConstArg arg){
				this.panel = arg.panel;
			}
			protected readonly IIconPanel panel;
			public abstract void OnEnter();
			public abstract void OnExit();
		}
		public interface IPanelWaitingForPickUpState: IPanelTransactionState{}
		public interface IPanelHoverableState: IPanelTransactionState{}
		public interface IPanelUnhoverableState: IPanelTransactionState{}
		public interface IPanelHoveredState: IPanelTransactionState{}
		/* eqp tool super */
		public interface IEquipToolPanelTransactionState: IPanelTransactionState{}
		public abstract class AbsEquipToolPanelTransactionState: IEquipToolPanelTransactionState{
			public AbsEquipToolPanelTransactionState(IEquipToolPanelTransactionStateConstArg arg){
				this.eqpToolPanel = arg.eqpToolPanel;
				this.eqpIITAM = arg.eqpIITAM;
			}
			protected readonly IEquipToolPanel eqpToolPanel;
			protected readonly IEquippableIITAManager eqpIITAM;
			public abstract void OnEnter();
			public abstract void OnExit();
		}
		public interface IEquipToolPanelWaitingForPickUpState: IPanelWaitingForPickUpState, IEquipToolPanelTransactionState{}
		public interface IEquipToolPanelHoverableState: IPanelHoverableState, IEquipToolPanelTransactionState{}
		public interface IEquipToolPanelUnhoverableState: IPanelUnhoverableState, IEquipToolPanelTransactionState{}
		public interface IEquipToolPanelHoveredState: IPanelHoveredState, IEquipToolPanelTransactionState{
			void SetPickedEquippableII(IEquippableItemIcon pickedEqpII);
		}
	/* sub */
		public class EquipToolPanelWaitingForPickUpState: AbsEquipToolPanelTransactionState, IEquipToolPanelWaitingForPickUpState{
			public EquipToolPanelWaitingForPickUpState(IEquipToolPanelTransactionStateConstArg arg): base(arg){}
			public override void OnEnter(){
				eqpToolPanel.BecomeSelectable();
			}
			public override void OnExit(){

			}
		}
		public class EquipToolPanelHoverableState: AbsEquipToolPanelTransactionState, IEquipToolPanelHoverableState{
			public EquipToolPanelHoverableState(IEquipToolPanelTransactionStateConstArg arg): base(arg){}
			public override void OnEnter(){
				eqpToolPanel.BecomeSelectable();
			}
			public override void OnExit(){

			}
		}
		public class EquipToolPanelUnhoverableState: AbsEquipToolPanelTransactionState, IEquipToolPanelUnhoverableState{
			public EquipToolPanelUnhoverableState(IEquipToolPanelTransactionStateConstArg arg): base(arg){}
			public override void OnEnter(){
				eqpToolPanel.BecomeUnselectable();
			}
			public override void OnExit(){

			}
		}
		public class EquipToolPanelHoveredState: AbsEquipToolPanelTransactionState, IEquipToolPanelHoveredState{
			public EquipToolPanelHoveredState(IEquipToolPanelTransactionStateConstArg arg): base(arg){}
			IEquippableItemIcon pickedEqpII;
			public void SetPickedEquippableII(IEquippableItemIcon pickedEqpII){
				this.pickedEqpII = pickedEqpII;
			}
			public override void OnEnter(){
				eqpToolPanel.BecomeSelected();
				eqpToolPanel.CheckAndAddEmptyAddTarget(pickedEqpII);
				eqpToolPanel.HoverDefaultTransactionTargetEqpII(pickedEqpII);
				if(eqpToolPanel is IEquipToolPoolItemsPanel){
					eqpIITAM.SetEqpIIToUnequip(pickedEqpII);
				}else{
					eqpIITAM.SetEqpIIToEquip(pickedEqpII);
				}
			}
			public override void OnExit(){
				this.pickedEqpII = null;
				eqpToolPanel.CheckAndRemoveEmptyEqpIIs();
				eqpIITAM.ClearHoveredEqpII();
				eqpIITAM.ClearEqpIIsToEquipAndUnequip();
			}
		}
	/* Const */
		public interface IPanelTransactionStateConstArg{
			IIconPanel panel{get;}
		}
		public class PanelTransactionStateConstArg: IPanelTransactionStateConstArg{
			public PanelTransactionStateConstArg(IIconPanel panel){
				this._panel = panel;
			}
			public IIconPanel panel{
				get{return _panel;}
			}
			readonly IIconPanel _panel;
		}
		public interface IEquipToolPanelTransactionStateConstArg{
			IEquipToolPanel eqpToolPanel{get;}
			IEquippableIITAManager eqpIITAM{get;}
		}
		public class EquipToolPanelTransactionStateConstArg: PanelTransactionStateConstArg,IEquipToolPanelTransactionStateConstArg{
			public EquipToolPanelTransactionStateConstArg(IEquippableIITAManager eqpIITAM, IEquipToolPanel eqpToolPanel, IEquipTool eqpTool) :base(eqpToolPanel){
				this._eqpIITAM = eqpIITAM;
			}
			public IEquipToolPanel eqpToolPanel{
				get{
					return this.panel as IEquipToolPanel;//safe
				}
			}
			readonly IEquippableIITAManager _eqpIITAM;
			public IEquippableIITAManager eqpIITAM{
				get{return this._eqpIITAM;}
			}
		}
	/*  */
}
