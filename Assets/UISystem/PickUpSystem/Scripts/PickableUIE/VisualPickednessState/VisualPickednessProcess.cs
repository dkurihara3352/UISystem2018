using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DKUtility;

namespace UISystem.PickUpUISystem{
	public interface IVisualPickednessProcess: IWaitAndExpireProcess{
	}
	public class VisualPickednessProcess: AbsWaitAndExpireProcess, IVisualPickednessProcess{
		public VisualPickednessProcess(IProcessManager processManager, IWaitAndExpireProcessState state, float expireT, IPickableUIImage pickableUIImage, float sourcePickedness, float targetPickedness): base(processManager, state, expireT){
			thisInterpolator = new PickableUIImageVisualPickednessInterpolator(pickableUIImage, sourcePickedness, targetPickedness);
		}
		IPickableUIImageVisualPickednessInterpolator thisInterpolator;
		protected override void UpdateProcessImple(float deltaT){
			thisInterpolator.Interpolate(thisNormlizedT);
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
