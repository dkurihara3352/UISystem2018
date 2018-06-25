using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface ITransferableUIElement{
		bool IsTransferable();
		int GetTransferableQuantity();
		void UpdateTransferableQuantity(int pickedQuantity);
		void TravelTransfer(IIconGroup destIG);
		void SpotTransfer(IIconGroup destIG);
	}
}
