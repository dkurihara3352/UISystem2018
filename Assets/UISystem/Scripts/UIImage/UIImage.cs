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

	public class UIImage: IUIImage{
		public UIImage(Graphic graphicComponent, Transform imageTrans, float defaultDarkness, float darkenedDarkness){
			thisGraphicComponent = graphicComponent;
			thisImageTrans = imageTrans;
			thisDefaultDarkness = defaultDarkness;
			thisDarkenedDarkness = darkenedDarkness;
			SetDarkness(thisDefaultDarkness);
		}
		readonly protected Graphic thisGraphicComponent;
		public float GetCurrentDarkness(){
			Color curColor = thisGraphicComponent.color;
			float h;
			float s;
			float v;
			Color.RGBToHSV(curColor, out h, out s, out v);
			return v;
		}
		float curDarkness;
		public float GetDefaultDarkness(){
			return thisDefaultDarkness;
		}
		readonly float thisDefaultDarkness;
		public float GetDarkenedDarkness(){
			return thisDarkenedDarkness;
		}
		readonly float thisDarkenedDarkness;
		public void SetDarkness(float darkness){
			// Color newColor = GetColorFormDarkness(darkness);
			// thisImage.color = newColor;
			// curDarkness = darkness;
			Color curColor = thisGraphicComponent.color;
			float a = curColor.a;
			float h;
			float s;
			float v;
			Color.RGBToHSV(curColor, out h, out s, out v);
			v = darkness;
			Color newColor = Color.HSVToRGB(h, s, v);
			newColor.a = a;
			thisGraphicComponent.color = newColor;
		}
		// Color GetColorFormDarkness(float darkness){
		// 	return Color.Lerp(Color.black, Color.white, darkness);
		// }
		readonly protected Transform thisImageTrans;
		public Transform GetTransform(){
			return thisImageTrans;
		}
		public void CopyPosition(IUIImage other){
			Transform otherTrans = other.GetTransform();
			Vector2 otherLocalPosition = otherTrans.localPosition;
			thisImageTrans.localPosition = otherLocalPosition;
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
