using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
public interface IPanelTransactionStateEngine: ISwitchableStateEngine<IPanelTransactionState>, IHoverabilityStateHandler{}
	public class PanelTransactonStateEngine: AbsSwitchableStateEngine<IPanelTransactionState>, IPanelTransactionStateEngine{
		public PanelTransactonStateEngine(IEquippableIITAManager eqpIITAM){
			// this.waitingForPickUpState = new Waiti
			this.eqpIITAM = eqpIITAM;
		}
		readonly IEquipToolPanelWaitingForPickUpState waitingForPickUpState;
		readonly IEquipToolPanelHoverableState hoverableState;
		readonly IEquipToolPanelUnhoverableState unhoverableState;
		readonly IEquipToolPanelHoveredState hoveredState;
		readonly IEquippableIITAManager eqpIITAM;
		/*  */
		public void WaitForPickUp(){
			TrySwitchState(waitingForPickUpState);
		}
		public void BecomeHoverable(){
			TrySwitchState(hoverableState);
		}
		public void BecomeUnhoverable(){
			TrySwitchState(unhoverableState);
		}
		public void BecomeHovered(){
			hoveredState.SetPickedEquippableII((IEquippableItemIcon)eqpIITAM.GetPickedII());
			TrySwitchState(hoveredState);
		}
		public bool IsHoverable(){
			return curState is IPanelHoverableState;
		}
		public bool IsHovered(){
			return curState is IPanelHoveredState;
		}
		/*  */
	}
	/* states */
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
			/* eqp tool sub */
			public abstract class AbsEquipToolPanelTransactionState: IPanelTransactionState{
				public AbsEquipToolPanelTransactionState(IEquipToolPanelTransactionStateConstArg arg){
					this.eqpToolPanel = arg.eqpToolPanel;
					this.eqpIITAM = arg.eqpIITAM;
				}
				protected readonly IEquipToolPanel eqpToolPanel;
				protected readonly IEquippableIITAManager eqpIITAM;
				public abstract void OnEnter();
				public abstract void OnExit();
			}
			public interface IEquipToolPanelWaitingForPickUpState: IPanelWaitingForPickUpState{}
			public interface IEquipToolPanelHoverableState: IPanelHoverableState{}
			public interface IEquipToolPanelUnhoverableState: IPanelUnhoverableState{}
			public interface IEquipToolPanelHoveredState: IPanelHoveredState{
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
			public class PanelHoverableState: AbsEquipToolPanelTransactionState, IPanelHoverableState{
				public PanelHoverableState(IEquipToolPanelTransactionStateConstArg arg): base(arg){}
				public override void OnEnter(){
					eqpToolPanel.BecomeSelectable();
				}
				public override void OnExit(){

				}
			}
			public class PanelUnhoverableState: AbsEquipToolPanelTransactionState, IPanelUnhoverableState{
				public PanelUnhoverableState(IEquipToolPanelTransactionStateConstArg arg): base(arg){}
				public override void OnEnter(){
					eqpToolPanel.BecomeUnselectable();
				}
				public override void OnExit(){

				}
			}
			public class PanelHoveredState: AbsEquipToolPanelTransactionState, IPanelHoveredState{
				public PanelHoveredState(IEquipToolPanelTransactionStateConstArg arg): base(arg){}
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
		public class PanelTrasactionStateConstArg: IPanelTransactionStateConstArg{
			public PanelTrasactionStateConstArg(IIconPanel panel){
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
	/*  */
}
