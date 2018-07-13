using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem.PickUpUISystem{
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
				thisEqpToolUIEFactory = eqpToolUIAArg.eqpToolUIEFactory;
				base.GetReadyForActivation(passedArg);
			}else
				throw new System.ArgumentException("passedArg must be of type IEquipToolUIAActivationArg");
		}
		IEquippableIITAManager thisEqpIITAM;/* not used? */
		IEquipTool thisEqpTool;/* not used? */
		IEquippableUIItem thisEqpItem;
		IEquipToolUIEFactory thisEqpToolUIEFactory;
		public void SetInitializationFields(IEquippableUIItem item){
			thisEqpItem = item;
		}
		IEquippableItemIcon eqpII{
			get{return this.GetUIElement() as IEquippableItemIcon;}//safe
		}
		protected override IEquippableItemIcon CreateUIElement(){
			IEquipToolActivationData data = (IEquipToolActivationData)thisDomainActivationData;
			IDragImageImplementorConstArg dragImageImplementorConstArg = new DragImageImplementorConstArg(1f, 1f, data.pickUpSystemProcessFactory, data.pickUpManager);
			IDragImageImplementor dragImageImplementor = new DragImageImplementor(dragImageImplementorConstArg);
			IVisualPickednessStateEngine visualPickednessStateEngine = new VisualPickednessStateEngine(data.pickUpSystemProcessFactory);
			IEqpIITransactionStateEngine eqpIITAStateEngine = new EqpIITransactionStateEngine(thisEqpIITAM, thisEqpTool);
			IItemIconPickUpImplementor itemIconPickUpImplementor = new ItemIconPickUpImplementor(thisEqpIITAM, data.pickUpSystemUIElementFactory);
			IItemIconEmptinessStateEngine emptinessStateEngine = new ItemIconEmptinessStateEngine(data.pickUpSystemProcessFactory);
			IEqpIITransferabilityHandlerImplementor eqpIITransferabilityHandlerImplementor = new EqpIITransferabilityHandlerImplementor(thisEqpIITAM);
			IQuantityRoller quantityRoller = thisEqpToolUIEFactory.CreateItemIconQuantityRoller(this);
			IEquippableItemIconConstArg arg = new EquippableItemIconConstArg(data.uim, data.pickUpSystemProcessFactory, data.eqpToolUIEFactory, this, null, thisEqpTool, dragImageImplementor, visualPickednessStateEngine, thisEqpIITAM, thisEqpItem, eqpIITAStateEngine, itemIconPickUpImplementor, emptinessStateEngine, eqpIITransferabilityHandlerImplementor, quantityRoller);
			
			return new EquippableItemIcon(arg);
		}
	}
}

