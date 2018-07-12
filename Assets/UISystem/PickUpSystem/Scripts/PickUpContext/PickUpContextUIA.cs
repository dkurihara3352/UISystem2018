using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem.PickUpUISystem{
	public interface IPickUpContextUIAdaptor: IUIAdaptor{
		IUIAActivationData CreateDomainActivationData(IUIAActivationData passedData);
		Vector2 GetPickUpReserveWorldPos();
	}
	public abstract class AbsPickUpContextUIAdaptor<T>: AbsUIAdaptor<T>, IPickUpContextUIAdaptor where T: IPickUpContextUIE{
		/*  uia for tools and widgets that handles pickup
		*/
		protected abstract T GetPickUpContextUIE();
		public abstract IUIAActivationData CreateDomainActivationData(IUIAActivationData passedData);
		public Transform pickUpReserveTrans;/* assigned in the inspector */
		public Vector2 GetPickUpReserveWorldPos(){
			return new Vector2(pickUpReserveTrans.position.x, pickUpReserveTrans.position.y);
		}
	}
}
