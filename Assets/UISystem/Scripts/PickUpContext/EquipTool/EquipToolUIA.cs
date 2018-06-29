using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IEquipToolUIAdaptor: IPickUpContextUIAdaptor{
	}
	public class EquipToolUIAdaptor: AbsPickUpContextUIAdaptor<IEquipToolUIE>, IEquipToolUIAdaptor{
		/* assigned in the insp */
			public IEquipToolPanel thisEqpItemsPanel;
			public IEquipToolPanel thisPoolItemsPanel;
		/*  */
		IEquipToolActivationData eqpUIAActivationData{
			get{return this.thisDomainActivationData as IEquipToolActivationData;}
		}
		IEquipToolUIE eqpToolUIE;
		protected override IEquipToolUIE GetPickUpContextUIE(){
			return eqpToolUIE;
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

			return new EquipToolUIAActivationData(uim, eqpIITAM, eqpTool);
		}
		protected override IEquipToolUIE CreateUIElement(){
			IUIElementConstArg arg = new UIElementConstArg(thisDomainActivationData.uim, this, null, thisDomainActivationData.tool);
			IEquipToolUIE uie = new EquipToolUIE(arg);
			return uie;
		}

	}
}
