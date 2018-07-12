using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem.PickUpUISystem{
	public interface IEqpIITransferabilityHandlerImplementor: ITransferabilityHandlerImplementor{}
	public class EqpIITransferabilityHandlerImplementor: AbsTransferabilityHandlerImplementor, IEqpIITransferabilityHandlerImplementor{
		public EqpIITransferabilityHandlerImplementor(IEquippableIITAManager eqpIITAM){
			thisEqpIITAM = eqpIITAM;
		}
		public override void SetItemIcon(IItemIcon itemIcon){
			if(itemIcon is IEquippableItemIcon){
				base.SetItemIcon(itemIcon);
			}else{
				throw new System.InvalidOperationException("itemIcon needs to be of type IEquippableItemIcon");
			}
		}
		IEquippableItemIcon thisEqpII{get{return (IEquippableItemIcon)thisItemIcon;}}
		IEquippableUIItem thisEqpItem{get{return thisEqpII.GetEquippableItem();}}
		readonly IEquippableIITAManager thisEqpIITAM;
		protected override int GetMaxTransferableQuantity(){
			IItemTemplate thisItemTemp = thisEqpItem.GetItemTemplate();
				int thisQuantity = thisEqpII.GetItemQuantity();
				if(thisEqpII.IsBowOrWearItemIcon()){
					if(thisQuantity != 0)
						return 1;
					else
						return 0;
				}else{
					if(thisEqpII.IsInEqpIG())
						return thisQuantity;
					else{
						if(thisItemTemp.IsStackable())
							return thisQuantity;
						else{
							IEquipToolEquippedCarriedGearsIG relevantEqpCGIG = thisEqpIITAM.GetRelevantEquippedCarriedGearsIG();
							int equippedQuantity = relevantEqpCGIG.GetItemQuantity(thisEqpItem);
							int spaceInEqpIG = thisEqpItem.GetMaxEquippableQuantity() - equippedQuantity;
							return Mathf.Min(spaceInEqpIG, thisQuantity);
						}
					}
				}
		}
	}
}
