using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace UISystem{
	public interface IUIImage: IQuantityAnimationHandler{
		/* Darkness */
		float GetCurrentDarkness();/* range is from 0f to 1f */
		float GetDefaultDarkness();/* usually, 1f */
		float GetDarkenedDarkness();/* somewhere around .5f */
		void SetDarkness(float darkness);
		/* Transform */
		void DetachTo(IPickUpContextUIE pickUpContextUIE);
		void CopyPosition(IUIImage other);
		Transform GetTransform();
		Vector2 GetWorldPosition();
		void SetWorldPosition(Vector2 worldPos);
	}

	public class UIImage: MonoBehaviour, IUIImage{
		public UIImage(IUIImageConstArg arg){
			thisQuantityAnimationEngine = arg.quantityAnimationEngine;
			thisQuantityAnimationEngine.SetUIImage(this);
		}
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
		public Vector2 GetWorldPosition(){
			Vector3 worldPosV3 = GetTransform().position;
			return new Vector2(worldPosV3.x, worldPosV3.y);
		}
		public void SetWorldPosition(Vector2 worldPos){
			Vector3 newWorldPosV3 = new Vector3(worldPos.x, worldPos.y, 0f);
			GetTransform().position = newWorldPosV3;
		}
		/* Quantity Image animation handling */
		readonly IQuantityAnimationEngine thisQuantityAnimationEngine;
		public void AnimateQuantityImageIncrementally(int sourceQuantity, int targetQuantity){
			thisQuantityAnimationEngine.AnimateQuantityImageIncrementally(sourceQuantity, targetQuantity);
		}
		public void AnimateQuantityImageAtOnce(int sourceQuantity, int targetQuantity){
			thisQuantityAnimationEngine.AnimateQuantityImageAtOnce(sourceQuantity, targetQuantity);
		}
	}
	/* const */
	public interface IUIImageConstArg{
		IQuantityAnimationEngine quantityAnimationEngine{get;}
	}
}
