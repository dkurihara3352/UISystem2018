using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem.PickUpUISystem{
	public interface IItemIconUIAdaptor: IUIAdaptor{
		void SetUIItem(IUIItem item);
	}
	public abstract class AbsItemIconUIAdaptor<T>: UIAdaptor, IItemIconUIAdaptor where T: IItemIcon{
		IPickUpSystemUIAActivationData thisPickUpSystemDomainActivationData{
			get{return (IPickUpSystemUIAActivationData)thisDomainActivationData;}
		}
		public void SetUIItem(IUIItem item){
			thisItem = item;
		}
		protected IUIItem thisItem;
	}
}

