using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IPickUpManager{
		IPickableUIE GetPickedUIE();
		void ClearPickedUIE();
		IPickUpContextUIE GetPickUpContextUIE();
	}
	public abstract class AbsPickUpManager: IPickUpManager{
		public IPickableUIE GetPickedUIE(){
			return thisPickedUIE;
		}
		protected IPickableUIE thisPickedUIE;
		protected void SetPickedUIE(IPickableUIE pickedUIE){
			thisPickedUIE = pickedUIE;
		}
		public void ClearPickedUIE(){
			thisPickedUIE = null;
		}
		public abstract IPickUpContextUIE GetPickUpContextUIE();
	}
}