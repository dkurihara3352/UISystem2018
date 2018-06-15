using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace UISystem{
	public interface IUIImage{
	/* 	implementor derives from MonoBehaviour
	*/
		float GetCurrentDarkness();/* range is from 0f to 1f */
		float GetDefaultDarkness();/* usually, 1f */
		float GetDarkenedDarkness();/* somewhere around .5f */
		void SetDarkness(float darkness);
		void DetachTo(IPickUpContextUIE pickUpContextUIE);
		void CopyPosition(IUIImage other);
		Transform GetTransform();
	}

	public class UIImage: MonoBehaviour, IUIImage{
		public Image image;
		public float GetCurrentDarkness(){
			return curDarkness;
		}
		float curDarkness;
		public float GetDefaultDarkness(){
			return defaultDarkness;
		}
		public float defaultDarkness;
		public float GetDarkenedDarkness(){
			return darkenedDarkness;
		}
		public float darkenedDarkness;
		public void SetDarkness(float darkness){
			Color newColor = GetColorFormDarkness(darkness);
			image.color = newColor;
			curDarkness = darkness;
		}
		Color GetColorFormDarkness(float darkness){
			return Color.Lerp(Color.black, Color.white, darkness);
		}
		public void DetachTo(IPickUpContextUIE contextUIE){
			IPickUpContextUIAdaptor contextUIA = (IPickUpContextUIAdaptor)contextUIE.GetUIAdaptor();
			this.transform.SetParent(contextUIA.GetTransform(), worldPositionStays:true);
		}
		public Transform GetTransform(){
			return this.transform;
		}
		public void CopyPosition(IUIImage other){
			Transform otherTrans = other.GetTransform();
			Vector2 otherLocalPosition = otherTrans.localPosition;
			this.transform.localPosition = otherLocalPosition;
		}
	}
}
