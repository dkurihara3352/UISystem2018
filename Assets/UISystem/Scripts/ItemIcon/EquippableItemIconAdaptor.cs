using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IEquipToolElementUIA: IUIAdaptor{
	}
	public interface IEquippableItemIconAdaptor: IItemIconUIAdaptor, IEquipToolElementUIA{
		void SetInitializationFields(IEquippableUIItem item);
	}
	public class EquippableItemIconAdaptor: AbsItemIconUIAdaptor<IEquippableItemIcon>, IEquippableItemIconAdaptor{
		public override void GetReadyForActivation(IUIAActivationData passedArg){
			if(passedArg is IEquipToolActivationData){
				IEquipToolActivationData eqpToolUIAArg = passedArg as IEquipToolActivationData;
				thisEqpIITAM = eqpToolUIAArg.eqpIITAM;
				thisEqpTool = eqpToolUIAArg.eqpTool;
				base.GetReadyForActivation(passedArg);
			}else
				throw new System.ArgumentException("passedArg must be of type IEquipToolUIAActivationArg");
		}
		IEquippableIITAManager thisEqpIITAM;/* not used? */
		IEquipTool thisEqpTool;/* not used? */
		IEquippableUIItem thisEqpItem;
		public void SetInitializationFields(IEquippableUIItem item){
			thisEqpItem = item;
		}
		IEquippableItemIcon eqpII{
			get{return this.GetUIElement() as IEquippableItemIcon;}//safe
		}
		protected override IEquippableItemIcon CreateUIElement(){
			IUIAActivationData data = thisDomainActivationData;
			IDragImageImplementorConstArg dragImageImplementorConstArg = new DragImageImplementorConstArg(1f, 1f, data.processFactory, data.pickUpManager);
			IDragImageImplementor dragImageImplementor = new DragImageImplementor(dragImageImplementorConstArg);
			IEqpIITransactionStateEngine eqpIITAStateEngine = new EqpIITransactionStateEngine(thisEqpIITAM, thisEqpTool);
			IItemIconPickUpImplementor itemIconPickUpImplementor = new ItemIconPickUpImplementor(thisEqpIITAM);
			IItemIconEmptinessStateEngine emptinessStateEngine = new ItemIconEmptinessStateEngine();
			IEquippableItemIconConstArg arg = new EquippableItemIconConstArg(data.uim, this, null, thisEqpTool, dragImageImplementor, thisEqpIITAM, thisEqpItem, eqpIITAStateEngine, itemIconPickUpImplementor, emptinessStateEngine);
			
			return new EquippableItemIcon(arg);
		}
	}
}

