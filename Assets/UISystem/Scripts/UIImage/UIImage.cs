using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DKUtility;

namespace UISystem{
	public interface IUIImage{
		/* Darkness */
		float GetCurrentDarkness();/* range is from 0f to 1f */
		float GetDefaultDarkness();/* usually, 1f */
		float GetDarkenedDarkness();/* somewhere around .5f */
		void SetDarkness(float darkness);
		void TurnToSelectableDarkness();
		void TurnToUnselectableDarkenss();
		/* Transform */
		void CopyPosition(IUIImage other);
		Transform GetTransform();
		Vector2 GetWorldPosition();
		void SetWorldPosition(Vector2 worldPos);
		Color GetOriginalColor();
		Color GetColor();
		void SetColor(Color color);
		void TurnRed();
		void TurnGreen();
		void TurnToOriginalColor();
		void FlashRed();
		void FlashGreen();
	}

	public class UIImage: IUIImage{
		public UIImage(
			Graphic graphicComponent, 
			Transform imageTrans, 
			float defaultDarkness, 
			float darkenedDarkness,
			IUISystemProcessFactory processFactory

		){
			thisGraphicComponent = graphicComponent;
			thisOriginalColor = GetColor();
			thisImageTrans = imageTrans;
			thisDefaultDarkness = defaultDarkness;
			thisDarkenedDarkness = darkenedDarkness;
			SetDarkness(thisDefaultDarkness);
			thisProcessFactory = processFactory;
		}
		readonly protected Graphic thisGraphicComponent;
		Color thisOriginalColor;
		public Color GetOriginalColor(){
			return thisOriginalColor;
		}
		public float GetCurrentDarkness(){
			Color curColor = GetColor();
			float h;
			float s;
			float v;
			Color.RGBToHSV(curColor, out h, out s, out v);
			return v;
		}
		public float GetDefaultDarkness(){
			return thisDefaultDarkness;
		}
		readonly float thisDefaultDarkness;
		public float GetDarkenedDarkness(){
			return thisDarkenedDarkness;
		}
		readonly float thisDarkenedDarkness;
		public void SetDarkness(float darkness){
			Color newColor = GetColorAtDarkness(darkness);
			SetColor(newColor);
		}
		Color GetColorAtDarkness(float darkness){
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
		public void FlashRed(){
			IImageColorTurnProcess process = thisProcessFactory.CreateFalshColorProcess(this, Color.red);
			process.Run();
			SetRunningTurnColorProcess(process);
		}
		public void FlashGreen(){
			IImageColorTurnProcess process = thisProcessFactory.CreateFalshColorProcess(this, Color.green);
			process.Run();
			SetRunningTurnColorProcess(process);
		}
		public void TurnRed(){
			IImageColorTurnProcess process = thisProcessFactory.CreateGenericImageColorTurnProcess(this, Color.red);
			process.Run();
			SetRunningTurnColorProcess(process);
		}
		public void TurnGreen(){
			IImageColorTurnProcess process = thisProcessFactory.CreateGenericImageColorTurnProcess(this, Color.green);
			process.Run();
			SetRunningTurnColorProcess(process);
		}
		public void TurnToOriginalColor(){
			IImageColorTurnProcess process = thisProcessFactory.CreateGenericImageColorTurnProcess(this, thisOriginalColor);
			process.Run();
			SetRunningTurnColorProcess(process);
		}
		public void TurnToSelectableDarkness(){
			IImageColorTurnProcess process = thisProcessFactory.CreateGenericImageColorTurnProcess(this, GetColorAtDarkness(thisDefaultDarkness));
			process.Run();
			SetRunningTurnColorProcess(process);
		}
		public void TurnToUnselectableDarkenss(){
			IImageColorTurnProcess process = thisProcessFactory.CreateGenericImageColorTurnProcess(this, GetColorAtDarkness(thisDarkenedDarkness));
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
