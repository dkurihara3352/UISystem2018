using System.Collections;
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
		IEquipToolUIAActivationData eqpUIAActivationData{
			get{return this.activationData as IEquipToolUIAActivationData;}
		}
		IEquipToolUIE eqpToolUIE;
		protected override IEquipToolUIE GetPickUpContextUIE(){
			return eqpToolUIE;
		}
		public override IUIAActivationData CreateDomainActivationData(IUIManager uim){
			/*  Instantiate and set up
					IITAM
					Tool
					Factory
				pass them
			*/
			IEquipTool eqpTool = new EquipTool();
			IEquippableIITAManager eqpIITAM  = new EquippableIITAManager(this.eqpItemsPanel, this.poolItemsPanel, eqpTool);
			IEquipToolUIEFactory factory = new EquipToolUIEFactory(uim, eqpTool, eqpIITAM);

			return new EquipToolUIAActivationData(uim, factory, eqpIITAM, eqpTool);
		}
		protected override IEquipToolUIE CreateUIElement(IUIElementFactory factory){
			if(factory is IEquipToolUIEFactory){
				IEquipToolUIEFactory eqpToolFactory = factory as IEquipToolUIEFactory;
				return eqpToolFactory.CreateEquipToolUIE(this);
			}
			else
				throw new System.ArgumentException("factory must be of type IEquipToolUIEFactory");
		}

	}
}
