using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IPickUpManager{
		IPickableUIElement GetPickedUIE();
		IPickUpReceiver GetHoveredPUReceiver();
		void ClearTAFields();
	}
	public abstract class AbsPickUpManager: IPickUpManager{
		public IPickableUIElement GetPickedUIE(){
			return pickedUIE;
		}
		protected IPickableUIElement pickedUIE;
		protected void SetPickedUIE(IPickableUIElement pickedUIE){
			this.pickedUIE = pickedUIE;
		}
		public IPickUpReceiver GetHoveredPUReceiver(){
			return hoveredPUReceiver;
		}
		protected IPickUpReceiver hoveredPUReceiver;
		protected void SetHoveredPUReceiver(IPickUpReceiver puReceiver){
			this.hoveredPUReceiver = puReceiver;
		}
		public virtual void ClearTAFields(){
			SetPickedUIE(null);
			SetHoveredPUReceiver(null);
		}
	}
}