using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IItemIconFactory{
		IItemIcon CreateItemIcon(IUIItem item);
	}
	public abstract class AbsItemIconFactory{
		readonly IUIManager uim;
		public IItemIcon CreateItemIcon(IUIItem item){
			/*  No need to access an prefab through inspector. Code it all up
					Instantiate an gameObject
						attach components
						Instantiate ItemIconUIAdaptor class obj
						Instantiate ItemIcon class obj
					uia
						GetReadyForActivation
							=> inst uie
					the go
						childed to pickupContextUIE's uia transform
						local pos is set to reserve pos
							reserveWorldPos = pickupContext.Get-
							reserveLocalPos = pickUpContextUIA.GetLocalPosition(reserveWorPos)
							uia.SetLocalPosition( reserveLocalPos)
					ii uie
						inst'ed and init'ed in uia GetReadyForActivation
			*/
			GameObject iiGO = new GameObject("iiGO");
			IItemIconUIAdaptor iiUIA = iiGO.AddComponent<ItemIconUIAdaptor>() as IItemIconUIAdaptor;
			iiUIA.SetItemForActivationPreparation(item);
			iiUIA.GetReadyForActivation(uim);
			IItemIcon itemIcon = iiUIA.GetItemIcon();

			return itemIcon;
		}
	}
	public class EqpItemIconFactory: AbsItemIconFactory{

	}
}
