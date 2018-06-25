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
		bool IsWaitingForImageInit();
		void InitImage();
	}
	public interface IItemIconEmptinessStateEngine: IEmptinessStateHandler{
		void SetItemIcon(IItemIcon itemIcon);
	}
	public class ItemIconEmptinessStateEngine: AbsSwitchableStateEngine<IItemIconEmptinessState>, IItemIconEmptinessStateEngine{
		public ItemIconEmptinessStateEngine(){
			/* inst and set states here */
		}
		public void SetItemIcon(IItemIcon itemIcon){
			thisItemIcon = itemIcon;
			SetItemIconOnAllStates(itemIcon);
		}
		void SetItemIconOnAllStates(IItemIcon itemIcon){
			thisWFImageInitState.SetItemIcon(itemIcon);
		}
		IItemIcon thisItemIcon;
		public bool IsEmpty(){
			return false;
		}
		public void DisemptifyInstantly(IUIItem item){}
		public void EmptifyInstantly(){}
		public void Disemptify(IUIItem item){}
		public void Emptify(){}
		public bool IsWaitingForImageInit(){
			return thisCurState is IWaitingForImageInitState;
		}
		readonly IWaitingForImageInitState thisWFImageInitState;
		public void InitImage(){}
	}
	public interface IItemIconEmptinessState: ISwitchableState{
		void SetItemIcon(IItemIcon itemIcon);
	}
	public interface IWaitingForImageInitState: IItemIconEmptinessState{
	}
}
