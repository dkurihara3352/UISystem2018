using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DKUtility;

namespace UISystem{
	public interface IImageColorTurnProcess: IProcess{}
	public class GenericImageColorTurnProcess : AbsInterpolatorProcess<IImageColorInterpolator>, IImageColorTurnProcess {
		public GenericImageColorTurnProcess(
			IImageColorTurnProcessConstArg arg
		):base(
			arg
		){
			thisUIImage = arg.uiImage;
			thisTargetColor = arg.targetColor;
			thisFlashes = arg.flashes;
		}
		readonly IUIImage thisUIImage;
		readonly Color thisTargetColor;
		readonly bool thisFlashes;
		protected override float GetLatestInitialValueDifference(){return 1f;}
		protected override IImageColorInterpolator InstantiateInterpolatorWithValues(){
			if(!thisFlashes)
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



	public interface IImageColorTurnProcessConstArg: IInterpolatorProcesssConstArg{
		IUIImage uiImage{get;}
		Color targetColor{get;}
		bool flashes{get;}
	}
	public class ImageColorTurnProcessConstArg: InterpolatorProcessConstArg, IImageColorTurnProcessConstArg{
		public ImageColorTurnProcessConstArg(
			IProcessManager processManager,
			float expireTime,

			IUIImage uiImage,
			Color targetColor,
			bool flashes
		): base(
			processManager,
			ProcessConstraint.ExpireTime,
			expireTime,
			false
		){
			thisUIImage = uiImage;
			thisTargetColor = targetColor;
			thisFlashes = flashes;
		}
		readonly IUIImage thisUIImage;
		public IUIImage uiImage{get{return thisUIImage;}}
		readonly Color thisTargetColor;
		public Color targetColor{get{return thisTargetColor;}}
		readonly bool thisFlashes;
		public bool flashes{get{return thisFlashes;}}
	}
}

