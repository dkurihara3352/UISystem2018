using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem.PickUpUISystem{
	public interface IPickableUIImage: IUIImage{
		void DetachTo(IPickUpContextUIE pickUpContextUIE);
		void SetVisualPickedness(float pickedness);
		float GetVisualPickedness();
	}
	public class PickableUIImage: UIImage, IPickableUIImage{
		public void DetachTo(IPickUpContextUIE contextUIE){
			IPickUpContextUIAdaptor contextUIA = (IPickUpContextUIAdaptor)contextUIE.GetUIAdaptor();
			this.transform.SetParent(contextUIA.GetTransform(), worldPositionStays:true);
		}
		public void SetVisualPickedness(float pickedness){}
		public float GetVisualPickedness(){return 0f;}
	}
}
