using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IEquipToolElementUIA: IUIAdaptor{
		void SetEquipIITAM(IEquippableIITAManager eqpIITAM);
		void SetEquipTool(IEquipTool tool);
	}
	public interface IEquippableItemIconUIA: IItemIconUIAdaptor, IEquipToolElementUIA{
		void SetEquippableItem(IEquippableUIItem item);
	}
	public class EquippableItemIconUIA: AbsItemIconUIAdaptor<IEquippableItemIcon>, IEquippableItemIconUIA{
		public override void GetReadyForActivation(IUIAActivationData passedArg){
			if(passedArg is IEquipToolActivationData){
				IEquipToolActivationData eqpToolUIAArg = passedArg as IEquipToolActivationData;
				this.SetEquipIITAM(eqpToolUIAArg.eqpIITAM);
				this.SetEquipTool(eqpToolUIAArg.eqpTool);
				base.GetReadyForActivation(passedArg);
			}else
				throw new System.ArgumentException("passedArg must be of type IEquipToolUIAActivationArg");
		}
		IEquippableUIItem eqpItem;
		public void SetEquippableItem(IEquippableUIItem item){
			this.eqpItem = item;
		}
		IEquippableItemIcon eqpII{
			get{return this.GetUIElement() as IEquippableItemIcon;}
		}
		IEquippableIITAManager eqpIITAM;
		public void SetEquipIITAM(IEquippableIITAManager eqpIITAM){
			this.eqpIITAM = eqpIITAM;
		}
		IEquipTool eqpTool;
		public void SetEquipTool(IEquipTool tool){
			this.eqpTool = tool;
		}
		protected override IEquippableItemIcon CreateUIElement(IUIElementFactory factory){
			IEquipToolUIEFactory eqpToolFactory = null;
			if(factory is IEquipToolUIEFactory)
				eqpToolFactory = factory as IEquipToolUIEFactory;
			return eqpToolFactory.CreateEquippableItemIcon(this, eqpItem);
		}
	}
}

