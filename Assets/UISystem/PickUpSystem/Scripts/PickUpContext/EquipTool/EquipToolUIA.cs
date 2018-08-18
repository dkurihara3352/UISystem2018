using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem.PickUpUISystem{
	public interface IEquipToolUIAdaptor: IPickUpContextUIAdaptor{
	}
	public class EquipToolUIAdaptor: AbsPickUpContextUIAdaptor<IEquipToolUIE>, IEquipToolUIAdaptor{
		/* assigned in the insp */
			public IEquipToolPanel thisEqpItemsPanel;
			public IEquipToolPanel thisPoolItemsPanel;
			public DKUtility.IProcessManager processManager;
		/*  */
		IEquipToolActivationData eqpUIAActivationData{
			get{return this.thisDomainActivationData as IEquipToolActivationData;}
		}
		protected override IEquipToolUIE GetPickUpContextUIE(){
			return (IEquipToolUIE)GetUIElement();
		}
		public override IUIAActivationData CreateDomainActivationData(IUIAActivationData passedData){
			/*  Instantiate and set up
					IITAM
					Tool
					Factory
				pass them
			*/
			IUIManager uim = passedData.uim;
			IEqpIITAMStateEngine eqpIITAMStateEngine = new EqpIITAMStateEngine();
			IPickUpReceiverSwitch<IEquippableItemIcon> hoveredEqpIISwitch = new PickUpReceiverSwitch<IEquippableItemIcon>();
			IPickUpReceiverSwitch<IEquipToolPanel> hoveredEqpToolPanelSwitch = new PickUpReceiverSwitch<IEquipToolPanel>();
			IEquipToolIGManager eqpToolIGManager = new EquipToolIGManager();
			IEqpIITAMConstArg arg = new EqpIITAMConstArg(eqpIITAMStateEngine, thisEqpItemsPanel, thisPoolItemsPanel,hoveredEqpIISwitch, hoveredEqpToolPanelSwitch, eqpToolIGManager);
			IEquippableIITAManager eqpIITAM  = new EquippableItemIconTransactionManager(arg);
			IEquipTool eqpTool = new EquipTool(uim, eqpIITAM);
			IPickUpSystemProcessFactory pickUpSystemProcessFactory = new PickUpSystemProcessFactory(processManager, uim);
			IEquipToolUIEFactory equipToolUIEFactory = new EquipToolUIEFactory(uim, pickUpSystemProcessFactory, eqpTool, eqpIITAM);

			return new EquipToolUIAActivationData(uim, pickUpSystemProcessFactory, equipToolUIEFactory, eqpIITAM, eqpTool);
		}
		protected override IUIElement CreateUIElement(IUIImage image){
			IUIElementConstArg arg = new UIElementConstArg(
				thisDomainActivationData.uim, 
				thisDomainActivationData.processFactory, 
				thisDomainActivationData.uiElementFactory, 
				this, 
				image,
				ActivationMode.Alpha
			);
			IEquipToolUIE uie = new EquipToolUIE(arg);
			return uie;
		}

	}
}
