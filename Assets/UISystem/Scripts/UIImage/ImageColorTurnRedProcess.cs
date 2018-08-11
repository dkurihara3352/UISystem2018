using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DKUtility;

namespace UISystem{
	public interface IImageColorTurnProcess: IProcess{}
	public class GenericImageColorTurnProcess : AbsInterpolatorProcess<IImageColorInterpolator>, IImageColorTurnProcess {
		public GenericImageColorTurnProcess(
			IProcessManager processManager,
			float expireTime,
			IUIImage uiImage, 
			Color targetColor,
			bool flash
		):base(
			processManager,
			ProcessConstraint.expireTime,
			expireTime,
			0.05f,
			false,
			null
		){
			thisUIImage = uiImage;
			thisTargetColor = targetColor;
			thisFlash = flash;
		}
		readonly IUIImage thisUIImage;
		readonly Color thisTargetColor;
		readonly bool thisFlash;
		protected override float GetLatestInitialValueDifference(){return 1f;}
		protected override IImageColorInterpolator InstantiateInterpolatorWithValues(){
			if(!thisFlash)
				return new ImageColorInterpolator(thisUIImage, thisTargetColor);
			else
				return new ImageColorFlashInterpolator(thisUIImage, thisTargetColor);
		}
	}
	public interface IImageColorInterpolator: IInterpolator{

	}
	public class ImageColorInterpolator: AbsInterpolator, IImageColorInterpolator{
		public ImageColorInterpolator(IUIImage uiImage, Color targetColor){
			thisTargetColor = targetColor;
			thisUIImage = uiImage;
			thisInitialColor = uiImage.GetColor();
		}
		readonly Color thisTargetColor;
		readonly IUIImage thisUIImage;
		readonly Color thisInitialColor;

		protected override void InterpolateImple(float normalizedT){
			Color newColor = Color.Lerp(thisInitialColor, thisTargetColor, normalizedT);
			thisUIImage.SetColor(newColor);
		}
		public override void Terminate(){}
	}
	public class ImageColorFlashInterpolator: AbsInterpolator, IImageColorInterpolator{
		public ImageColorFlashInterpolator(IUIImage uiImage, Color targetColor){
			thisUIImage = uiImage;
			thisTargetColor = targetColor;
			thisInitialColor = uiImage.GetDefaultColor();
		}
		readonly IUIImage thisUIImage;
		readonly Color thisTargetColor;
		readonly Color thisInitialColor;
		protected override void InterpolateImple(float normalizedT){
			Color newColor;
			if(normalizedT < .5f)
				newColor = Color.Lerp(thisInitialColor, thisTargetColor, normalizedT * 2f);
			else
				newColor = Color.Lerp(thisTargetColor, thisInitialColor, (normalizedT - .5f) * 2f);
			thisUIImage.SetColor(newColor);
		}
		public override void Terminate(){}
	}
}

