using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem.PickUpUISystem{
	public interface IPickUpSystemUIA: IUIAdaptor{
	}
	public abstract class AbsPickUpSystemUIA<T>: UIAdaptor where T: IUIElement{
		public override void OnPointerEnter(UnityEngine.EventSystems.PointerEventData eventData){
			if(this.GetUIElement() is IPickUpReceiver){
				IPickUpReceiver receiver = (IPickUpReceiver)this.GetUIElement();
				receiver.CheckForHover();
			}
			base.OnPointerEnter(eventData);
		}
	}
}
