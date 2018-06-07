using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IItemIconFactory{
		IItemIcon CreateItemIcon(IUIItem item);
	}
	public abstract class AbsItemIconFactory{
		protected readonly IUIManager uim;
		public abstract IItemIcon CreateItemIcon(IUIItem item);
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
	}
	public class EqpItemIconFactory: AbsItemIconFactory{
		readonly IEquipTool eqpTool;
		public override IItemIcon CreateItemIcon(IUIItem item){
			GameObject iiGO = new GameObject("iiGO");
			// IItemIconUIAdaptor iiUIA = iiGO.AddComponent<ItemIconUIAdaptor>() as IItemIconUIAdaptor;
			IEqpItemIconUIA eqpIIUIA = iiGO.AddComponent<IEqpItemIconUIA>() as IEqpItemIconUIA;
			eqpIIUIA.SetItemForActivationPreparation(item);
			eqpIIUIA.SetEqpTool(eqpTool);
			eqpIIUIA.GetReadyForActivation(uim);
			IEquippableItemIcon itemIcon = eqpIIUIA.GetItemIcon();

			return itemIcon;
		}
	}
}
