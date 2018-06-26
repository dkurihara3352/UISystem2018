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
		void IncreaseBy(int quantity, bool doesIncrement);
		void DecreaseBy(int quantity, bool doesIncrement, bool removesEmpty);
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
			return thisCurState is IItemIconEmptyState;
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
		public void IncreaseBy(int quantity, bool doesIncrement){
			thisCurState.IncreaseBy(quantity, doesIncrement);
		}
		public void DecreaseBy(int quantity, bool doesIncrement, bool removesEmpty){
			thisCurState.DecreaseBy(quantity, doesIncrement, removesEmpty);
		}
	}
}
