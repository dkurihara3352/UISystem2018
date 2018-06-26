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
			IEquipTool eqpTool = new EquipTool();
			IEqpIITAMStateEngine eqpIITAMStateEngine = new EqpIITAMStateEngine(eqpTool);
			IPickUpReceiverSwitch<IEquippableItemIcon> hoveredEqpIISwitch = new PickUpReceiverSwitch<IEquippableItemIcon>();
			IPickUpReceiverSwitch<IEquipToolPanel> hoveredEqpToolPanelSwitch = new PickUpReceiverSwitch<IEquipToolPanel>();
			IEquipToolIGManager eqpToolIGManager = new EquipToolIGManager();
			IEqpIITAMConstArg arg = new EqpIITAMConstArg(eqpIITAMStateEngine, thisEqpItemsPanel, thisPoolItemsPanel, eqpTool, hoveredEqpIISwitch, hoveredEqpToolPanelSwitch, eqpToolIGManager);
			IEquippableIITAManager eqpIITAM  = new EquippableItemIconTransactionManager(arg);
			IEquipToolUIEFactory eqpUIEFactory = new EquipToolUIEFactory(uim, eqpTool, eqpIITAM);

			return new EquipToolUIAActivationData(uim, eqpUIEFactory, eqpIITAM, eqpTool);
		}
		protected override IEquipToolUIE CreateUIElement(IUIElementFactory factory){
			IEquipToolUIEFactory eqpToolFactory = (IEquipToolUIEFactory)factory;
			return eqpToolFactory.CreateEquipToolUIE(this);
		}

	}
}
