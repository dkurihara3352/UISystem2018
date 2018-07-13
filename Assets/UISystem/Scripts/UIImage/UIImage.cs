using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace UISystem{
	public interface IUIImage{
		/* Darkness */
		float GetCurrentDarkness();/* range is from 0f to 1f */
		float GetDefaultDarkness();/* usually, 1f */
		float GetDarkenedDarkness();/* somewhere around .5f */
		void SetDarkness(float darkness);
		/* Transform */
		void CopyPosition(IUIImage other);
		Transform GetTransform();
		Vector2 GetWorldPosition();
		void SetWorldPosition(Vector2 worldPos);
	}

	public class UIImage: MonoBehaviour, IUIImage{
		public Image thisImage;
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
			thisImage.color = newColor;
			curDarkness = darkness;
		}
		Color GetColorFormDarkness(float darkness){
			return Color.Lerp(Color.black, Color.white, darkness);
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
	}
}
