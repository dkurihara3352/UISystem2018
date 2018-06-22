﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IEquipToolUIAdaptor: IPickUpContextUIAdaptor{
	}
	public class EquipToolUIAdaptor: AbsPickUpContextUIAdaptor<IEquipToolUIE>, IEquipToolUIAdaptor{
		/* assigned in the insp */
			public IIconPanel eqpItemsPanel;
			public IIconPanel poolItemsPanel;
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
			IEquipTool eqpTool = new EquipTool();
			IEqpIITAMStateEngine eqpIITAMStateEngine = new EqpIITAMStateEngine(eqpTool);
			IEquippableIITAManager eqpIITAM  = new EquippableIITAManager(eqpIITAMStateEngine, this.eqpItemsPanel, this.poolItemsPanel, eqpTool);
			IEquipToolUIEFactory factory = new EquipToolUIEFactory(passedData.uim, eqpTool, eqpIITAM);

			return new EquipToolUIAActivationData(passedData.uim, factory, eqpIITAM, eqpTool);
		}
		protected override IEquipToolUIE CreateUIElement(IUIElementFactory factory){
			IEquipToolUIEFactory eqpToolFactory = (IEquipToolUIEFactory)factory;
			return eqpToolFactory.CreateEquipToolUIE(this);
		}

	}
}