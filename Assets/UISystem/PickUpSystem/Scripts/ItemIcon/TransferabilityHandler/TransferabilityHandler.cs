using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem.PickUpUISystem{
	public interface ITransferabilityHandler{
		bool IsTransferable();
		int GetTransferableQuantity();
		void UpdateTransferableQuantity(int pickedQuantity);
		void TravelTransfer(IIconGroup destIG);
		void SpotTransfer(IIconGroup destIG);
	}
	public interface ITransferabilityHandlerImplementor: ITransferabilityHandler{
		void SetItemIcon(IItemIcon itemIcon);
	}
	public abstract class AbsTransferabilityHandlerImplementor: ITransferabilityHandlerImplementor{
		protected IItemIcon thisItemIcon;
		public virtual void SetItemIcon(IItemIcon itemIcon){
			thisItemIcon = itemIcon;
		}
		int thisTransferableQuantity;
		public bool IsTransferable(){
			return thisTransferableQuantity > 0;
		}
		public int GetTransferableQuantity(){
			return thisTransferableQuantity;
		}
		public void UpdateTransferableQuantity(int pickedQuantity){
			int transQ = this.CalcTransferableQuantity(pickedQuantity);
			thisTransferableQuantity = transQ;
		}
		int CalcTransferableQuantity(int pickedQuantity){
			int diff = GetMaxTransferableQuantity() - pickedQuantity;
			if(diff >= 0)
				return diff;
			else
				throw new System.InvalidOperationException("pickedQuantity must not exceed max transferable quantity");
		}
		protected abstract int GetMaxTransferableQuantity();
		public void TravelTransfer(IIconGroup destIG){
			destIG.ReceiveTravelTransfer(thisItemIcon);
		}
		public void SpotTransfer(IIconGroup destIG){
			destIG.ReceiveSpotTransfer(thisItemIcon);
		}
	} 
}
