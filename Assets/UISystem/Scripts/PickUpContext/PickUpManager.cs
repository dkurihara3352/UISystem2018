using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IPickUpManager{
		IPickableUIE GetPickedUIE();
		IPickUpReceiver GetHoveredPUReceiver();
		void ClearTAFields();
		IPickUpContextUIE GetPickUpContextUIE();
	}
	public abstract class AbsPickUpManager: IPickUpManager{
		public IPickableUIE GetPickedUIE(){
			return pickedUIE;
		}
		protected IPickableUIE pickedUIE;
		protected void SetPickedUIE(IPickableUIE pickedUIE){
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
		public abstract IPickUpContextUIE GetPickUpContextUIE();
	}
}