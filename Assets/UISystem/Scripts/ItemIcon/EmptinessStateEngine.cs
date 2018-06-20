using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IEmptinessStateHandler{
		bool IsEmpty();
		void DisemptifyInstantly(IUIItem item);
		void EmptifyInstantly();
		void Disemptify(IUIItem item);
		void Emptify();
	}
	public interface IItemIconEmptinessStateEngine: IEmptinessStateHandler{
		void SetItemIcon(IItemIcon itemIcon);
	}
	public class ItemIconEmptinessStateEngine: AbsSwitchableStateEngine<IItemIconEmptinessState>, IItemIconEmptinessStateEngine{
		public ItemIconEmptinessStateEngine(){
		}
		public void SetItemIcon(IItemIcon itemIcon){
			thisItemIcon = itemIcon;
		}
		IItemIcon thisItemIcon;
		public bool IsEmpty(){
			return false;
		}
		public void DisemptifyInstantly(IUIItem item){}
		public void EmptifyInstantly(){}
		public void Disemptify(IUIItem item){}
		public void Emptify(){}
	}
	public interface IItemIconEmptinessState: ISwitchableState{}
}
