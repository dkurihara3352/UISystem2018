using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem.PickUpUISystem{
	public interface IPickUpContextUIAdaptor: IUIAdaptor{
		IUIAdaptorBaseInitializationData CreateDomainActivationData(IUIAdaptorBaseInitializationData passedData);
		Vector2 GetPickUpReserveWorldPos();
	}
	public abstract class AbsPickUpContextUIAdaptor<T>: UIAdaptor, IPickUpContextUIAdaptor where T: class, IPickUpContextUIE{
		/*  uia for tools and widgets that handles pickup
		*/
		protected abstract T GetPickUpContextUIE();
		public abstract IUIAdaptorBaseInitializationData CreateDomainActivationData(IUIAdaptorBaseInitializationData passedData);
		public Transform pickUpReserveTrans;/* assigned in the inspector */
		public Vector2 GetPickUpReserveWorldPos(){
			return new Vector2(pickUpReserveTrans.position.x, pickUpReserveTrans.position.y);
		}
	}
}
