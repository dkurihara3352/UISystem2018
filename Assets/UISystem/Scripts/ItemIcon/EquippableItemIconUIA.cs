using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IEquipToolElementUIA: IUIAdaptor{
	}
	public interface IEquippableItemIconUIA: IItemIconUIAdaptor, IEquipToolElementUIA{
	}
	public class EquippableItemIconUIA: AbsItemIconUIAdaptor<IEquippableItemIcon>, IEquippableItemIconUIA{
		public override void GetReadyForActivation(IUIAActivationData passedArg){
			if(passedArg is IEquipToolActivationData){
				IEquipToolActivationData eqpToolUIAArg = passedArg as IEquipToolActivationData;
				this.eqpIITAM = eqpToolUIAArg.eqpIITAM;
				this.eqpTool = eqpToolUIAArg.eqpTool;
				base.GetReadyForActivation(passedArg);
			}else
				throw new System.ArgumentException("passedArg must be of type IEquipToolUIAActivationArg");
		}
		IEquippableIITAManager eqpIITAM;/* not used? */
		IEquipTool eqpTool;/* not used? */
		IEquippableUIItem eqpItem;
		public void SetEquippableItem(IEquippableUIItem item){
			this.eqpItem = item;
		}
		IEquippableItemIcon eqpII{
			get{return this.GetUIElement() as IEquippableItemIcon;}//safe
		}
		protected override IEquippableItemIcon CreateUIElement(IUIElementFactory factory){
			IEquipToolUIEFactory eqpToolFactory = null;
			if(factory is IEquipToolUIEFactory)
				eqpToolFactory = factory as IEquipToolUIEFactory;
			return eqpToolFactory.CreateEquippableItemIcon(this, eqpItem);
		}
	}
}

