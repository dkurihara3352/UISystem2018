using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	// public interface IItemIconFactory{
	// 	IItemIcon CreateItemIcon(IUIItem item);
	// }
	// public abstract class AbsItemIconFactory{
	// 	protected readonly IUIManager uim;
	// 	public abstract IItemIcon CreateItemIcon(IUIItem item);
	// 		/*  No need to access an prefab through inspector. Code it all up
	// 				Instantiate an gameObject
	// 					attach components
	// 					Instantiate ItemIconUIAdaptor class obj
	// 					Instantiate ItemIcon class obj
	// 				uia
	// 					GetReadyForActivation
	// 						=> inst uie
	// 				the go
	// 					childed to pickupContextUIE's uia transform
	// 					local pos is set to reserve pos
	// 						reserveWorldPos = pickupContext.Get-
	// 						reserveLocalPos = pickUpContextUIA.GetLocalPosition(reserveWorPos)
	// 						uia.SetLocalPosition( reserveLocalPos)
	// 				ii uie
	// 					inst'ed and init'ed in uia GetReadyForActivation
	// 		*/
	// }
	// public interface IEquippableItemIconFactory: IItemIconFactory{}
	// public class EquippableItemIconFactory: AbsItemIconFactory, IEquippableItemIconFactory{
	// 	public EquippableItemIconFactory(IEquipTool eqpTool, IEquippableIITAManager eqpIITAM){
	// 		this.eqpTool = eqpTool;
	// 		this.eqpIITAM = eqpIITAM;
	// 	}
	// 	readonly IEquipTool eqpTool;
	// 	readonly IEquippableIITAManager eqpIITAM;
	// 	public override IItemIcon CreateItemIcon(IUIItem item){
	// 		GameObject iiGO = new GameObject("iiGO");
	// 		IEquippableItemIconUIA eqpIIUIA = iiGO.AddComponent<EquippableItemIconUIA>() as IEquippableItemIconUIA;
	// 		IEquippableUIItem eqpItem;
	// 		if(item is IEquippableUIItem)
	// 			eqpItem = item as IEquippableUIItem;
	// 		else
	// 			throw new System.ArgumentException("item must be of type IEquippableUIItem");
	// 		eqpIIUIA.SetEquippableItem(eqpItem);
	// 		eqpIIUIA.SetEquipTool(eqpTool);
	// 		eqpIIUIA.SetEquipIITAM(eqpIITAM);
	// 		eqpIIUIA.GetReadyForActivation(uim);
	// 		IEquippableItemIcon itemIcon = eqpIIUIA.GetEquippableItemIcon();
	// 		return itemIcon;
	// 	}
	// }
}
