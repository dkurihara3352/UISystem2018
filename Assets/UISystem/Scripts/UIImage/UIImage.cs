using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DKUtility;

namespace UISystem{
	public interface IUIImage{
		void TurnToSelectableBrightness();
		void TurnToUnselectableBrightness();
		/* Transform */
		void CopyPosition(IUIImage other);
		Transform GetTransform();
		Vector2 GetWorldPosition();
		void SetWorldPosition(Vector2 worldPos);
		Color GetOriginalColor();
		Color GetDefaultColor();
		Color GetDarkenedColor();
		Color GetColor();
		void SetColor(Color color);
		void TurnTo(Color color);
		void Flash(Color color);
		void TurnToOriginalColor();
	}

	public class UIImage: IUIImage{
		public UIImage(
			Graphic graphicComponent, 
			Transform imageTrans, 
			float defaultBrightness, 
			float darkenedBrightness,
			IUISystemProcessFactory processFactory

		){
			thisGraphicComponent = graphicComponent;
			thisOriginalColor = GetColor();
			thisImageTrans = imageTrans;
			thisDefaultBrightness = defaultBrightness;
			thisDarkenedBrightness = darkenedBrightness;
			thisDefaultColor = GetColorAtBrightness(thisDefaultBrightness);
			thisDarkenedColor = GetColorAtBrightness(thisDarkenedBrightness);
			SetColor(thisDefaultColor);
			thisProcessFactory = processFactory;
		}
		readonly protected Graphic thisGraphicComponent;
		Color thisOriginalColor;
		public Color GetOriginalColor(){
			return thisOriginalColor;
		}
		Color thisDefaultColor;
		public Color GetDefaultColor(){
			return thisDefaultColor;
		}
		Color thisDarkenedColor;
		public Color GetDarkenedColor(){
			return thisDarkenedColor;
		}
		public float GetCurrentBrightness(){
			Color curColor = GetColor();
			float h;
			float s;
			float v;
			Color.RGBToHSV(curColor, out h, out s, out v);
			return v;
		}
		readonly float thisDefaultBrightness;
		readonly float thisDarkenedBrightness;
		Color GetColorAtBrightness(float darkness){
			float a = thisOriginalColor.a;
			float h;
			float s;
			float v;
			Color.RGBToHSV(thisOriginalColor, out h, out s, out v);
			v = darkness;
			Color newColor = Color.HSVToRGB(h, s, v);
			newColor.a = a;

			return newColor;
		}

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
		/*  */
		IUISystemProcessFactory thisProcessFactory;
		public void TurnTo(Color color){
			IImageColorTurnProcess process = thisProcessFactory.CreateGenericImageColorTurnProcess(this, color);
			process.Run();
			SetRunningTurnColorProcess(process);
		}
		public void Flash(Color color){
			IImageColorTurnProcess process = thisProcessFactory.CreateFalshColorProcess(this, color);
			process.Run();
			SetRunningTurnColorProcess(process);
		}
		public void TurnToOriginalColor(){
			IImageColorTurnProcess process = thisProcessFactory.CreateGenericImageColorTurnProcess(this, thisOriginalColor);
			process.Run();
			SetRunningTurnColorProcess(process);
		}
		public void TurnToSelectableBrightness(){
			IImageColorTurnProcess process = thisProcessFactory.CreateGenericImageColorTurnProcess(this, thisDefaultColor);
			process.Run();
			SetRunningTurnColorProcess(process);
		}
		public void TurnToUnselectableBrightness(){
			IImageColorTurnProcess process = thisProcessFactory.CreateGenericImageColorTurnProcess(this, thisDarkenedColor);
			process.Run();
			SetRunningTurnColorProcess(process);
		}
		IImageColorTurnProcess thisCurrentRunningProcess;
		void SetRunningTurnColorProcess(IImageColorTurnProcess process){
			if(thisCurrentRunningProcess != null){
				thisCurrentRunningProcess.Stop();				
			}
			thisCurrentRunningProcess = process;
		}
		public Color GetColor(){
			return thisGraphicComponent.color;
		}
		public void SetColor(Color color){
			thisGraphicComponent.color = color;
		}
	}
}
