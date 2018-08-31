using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UISystem.PickUpUISystem{
	public interface IEquipToolElementUIA: IInstatiableUIAdaptor{
	}
	public interface IEquippableItemIconAdaptor: IItemIconUIAdaptor, IEquipToolElementUIA{
	}
	public class EquippableItemIconAdaptor: AbsItemIconUIAdaptor<IEquippableItemIcon>, IEquippableItemIconAdaptor{
		public void SetInitializationFields(IUIAInitializationData data){
			if(data is IEquippableItemIconAdaptorInitializationData){
				IEquippableItemIconAdaptorInitializationData eqpIIAInitData = (IEquippableItemIconAdaptorInitializationData)data;
				thisEqpItem = eqpIIAInitData.equippableItem;
			}
		}
		IEquippableUIItem thisEqpItem;
		IEquippableIITAManager thisEqpIITAM;/* not used? */
		IEquipTool thisEqpTool;/* not used? */
		IEquipToolUIEFactory thisEqpToolUIEFactory;
		IEquippableItemIcon eqpII{
			get{return this.GetUIElement() as IEquippableItemIcon;}//safe
		}
		protected override IUIImage CreateUIImage(){
			Image image;
			Transform child = GetChildWithImage(out image);
			IItemIconImage itemIconImage = new ItemIconImage(thisItem, image, child, thisImageDefaultBrightness, thisImageDarkenedBrightness);
			return itemIconImage;
		}
		protected override IUIElement CreateUIElement(IUIImage image){
			IEquipToolActivationData data = (IEquipToolActivationData)thisDomainInitializationData;
			IDragImageImplementorConstArg dragImageImplementorConstArg = new DragImageImplementorConstArg(1f, 1f, data.pickUpSystemProcessFactory, data.pickUpManager);
			IDragImageImplementor dragImageImplementor = new DragImageImplementor(dragImageImplementorConstArg);
			IVisualPickednessStateEngine visualPickednessStateEngine = new VisualPickednessStateEngine(data.pickUpSystemProcessFactory);
			IEqpIITransactionStateEngine eqpIITAStateEngine = new EqpIITransactionStateEngine(thisEqpIITAM, thisEqpTool);
			IItemIconPickUpImplementor itemIconPickUpImplementor = new ItemIconPickUpImplementor(thisEqpIITAM, data.pickUpSystemUIElementFactory);
			IItemIconEmptinessStateEngine emptinessStateEngine = new ItemIconEmptinessStateEngine(data.pickUpSystemProcessFactory);
			IEqpIITransferabilityHandlerImplementor eqpIITransferabilityHandlerImplementor = new EqpIITransferabilityHandlerImplementor(thisEqpIITAM);
			IItemIconImage itemIconImage = image as IItemIconImage;
			IQuantityRoller quantityRoller = thisEqpToolUIEFactory.CreateItemIconQuantityRoller(this);
			IEquippableItemIconConstArg arg = new EquippableItemIconConstArg(data.uim, data.pickUpSystemProcessFactory, data.eqpToolUIEFactory, this, itemIconImage, thisEqpTool, dragImageImplementor, visualPickednessStateEngine, thisEqpIITAM, thisEqpItem, eqpIITAStateEngine, itemIconPickUpImplementor, emptinessStateEngine, eqpIITransferabilityHandlerImplementor, quantityRoller);
			
			return new EquippableItemIcon(arg);
		}
	}
	public interface IEquippableItemIconAdaptorInitializationData: IUIAInitializationData{
		IEquippableUIItem equippableItem{get;}
	}
	public class EquippableItemIconAdaptorInitializationData: IEquippableItemIconAdaptorInitializationData{
		public EquippableItemIconAdaptorInitializationData(IEquippableUIItem equippableItem){
			thisEquippableItem = equippableItem;
		}
		readonly IEquippableUIItem thisEquippableItem;
		public IEquippableUIItem equippableItem{get{return thisEquippableItem;}}
	}
}

