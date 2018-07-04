using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface ITurnImageDarknessProcess: IProcess{
	}
	public class TurnImageDarknessProcess: AbsInterpolatorProcess<IImageDarknessInterpolator>, ITurnImageDarknessProcess{
		public TurnImageDarknessProcess(IProcessManager processManager, ProcessConstraint rateOfChangeConstraint, float rateOfChange, float diffThreshold, IUIImage image, float targetDarkness): base(processManager, rateOfChangeConstraint, rateOfChange, null, diffThreshold){
			thisImage = image;
			float tarD = ClampValueZeroToOne(targetDarkness);
			thisTargetDarkness = targetDarkness;
		}
		readonly IUIImage thisImage;
		readonly float thisTargetDarkness;
		protected override float GetNormalizedValueDiff(){
			float curD = thisImage.GetCurrentDarkness();
			curD = ClampValueZeroToOne(curD);
			float diff = thisTargetDarkness - curD;
			if(diff < 0f)
				diff *= -1f;
			return diff;
		}
		protected override void SetTerminalValue(){
			thisImage.SetDarkness(thisTargetDarkness);
		}
		protected override IImageDarknessInterpolator InstantiateInterpolatorWithValues(){
			IImageDarknessInterpolator irper = new ImageDarknessInterpolator(thisImage, thisImage.GetCurrentDarkness(), thisTargetDarkness);
			return irper;
		}
	}
}
