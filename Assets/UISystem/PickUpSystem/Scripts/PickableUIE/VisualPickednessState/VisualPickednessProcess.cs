using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DKUtility;

namespace UISystem.PickUpUISystem{
	public interface IVisualPickednessProcess: IWaitAndExpireProcess{
	}
	public class VisualPickednessProcess: AbsInterpolatorProcess<IPickableUIImageVisualPickednessInterpolator>, IVisualPickednessProcess{
		public VisualPickednessProcess(IProcessManager processManager, float expireT, IPickableUIImage pickableUIImage, float targetPickedness): base(processManager, ProcessConstraint.expireTime, expireT, 0.05f, false){
			thisPickableUIImage = pickableUIImage;
			thisTargetPickedness = targetPickedness;
		}
		readonly IPickableUIImage thisPickableUIImage;
		readonly float thisTargetPickedness;
		protected override float GetLatestInitialValueDifference(){
			return thisTargetPickedness - thisPickableUIImage.GetVisualPickedness();
		}
		protected override IPickableUIImageVisualPickednessInterpolator InstantiateInterpolatorWithValues(){
			return new PickableUIImageVisualPickednessInterpolator(thisPickableUIImage, thisPickableUIImage.GetVisualPickedness(), thisTargetPickedness);
		}
	}
	public interface IPickableUIImageVisualPickednessInterpolator: IInterpolator{}
	public class PickableUIImageVisualPickednessInterpolator: AbsInterpolator, IPickableUIImageVisualPickednessInterpolator{
		public PickableUIImageVisualPickednessInterpolator(IPickableUIImage pickableUIImage, float sourceVisualPickedness, float targetVisualPickedness){
			thisPickableUIImage = pickableUIImage;
			thisSourceVisualPickedness = sourceVisualPickedness;
			thisTargetVisualPickedness = targetVisualPickedness;
		}
		readonly IPickableUIImage thisPickableUIImage;
		readonly float thisSourceVisualPickedness;
		readonly float thisTargetVisualPickedness;
		protected override void InterpolateImple(float normalizedT){
			float newVisualPickedness = Mathf.Lerp(thisSourceVisualPickedness, thisTargetVisualPickedness, normalizedT);
			thisPickableUIImage.SetVisualPickedness(newVisualPickedness);
		}
		public override void Terminate(){
			thisPickableUIImage.SetVisualPickedness(thisTargetVisualPickedness);
		}
	}
}
