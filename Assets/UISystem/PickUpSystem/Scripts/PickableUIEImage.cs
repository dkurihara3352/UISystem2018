using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UISystem.PickUpUISystem{
	public interface IPickableUIImage: IUIImage{
		void DetachTo(IPickUpContextUIE pickUpContextUIE);
		void SetVisualPickedness(float pickedness);
		float GetVisualPickedness();
	}
	public class PickableUIImage: UIImage, IPickableUIImage{
		public PickableUIImage(Image image, Transform imageTrans, float defaultDarkness, float darkenedDarkness): base(image, imageTrans, defaultDarkness, darkenedDarkness, null){}
		public void DetachTo(IPickUpContextUIE contextUIE){
			IPickUpContextUIAdaptor contextUIA = (IPickUpContextUIAdaptor)contextUIE.GetUIAdaptor();
			thisImageTrans.SetParent(contextUIA.GetTransform(), worldPositionStays:true);
		}
		public void SetVisualPickedness(float pickedness){}
		public float GetVisualPickedness(){return 0f;}
	}
}
