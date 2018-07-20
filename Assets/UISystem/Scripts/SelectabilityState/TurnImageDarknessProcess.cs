using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DKUtility;

namespace UISystem{
	public interface ITurnImageDarknessProcess: IProcess{
	}
	public class TurnImageDarknessProcess: AbsInterpolatorProcess<IImageDarknessInterpolator>, ITurnImageDarknessProcess{
		public TurnImageDarknessProcess(IProcessManager processManager, ProcessConstraint rateOfChangeConstraint, float rateOfChange, float diffThreshold, IUIImage image, float targetDarkness, bool useSpringT): base(processManager, rateOfChangeConstraint, rateOfChange, diffThreshold, useSpringT){
			thisImage = image;
			float tarD = ClampValueZeroToOne(targetDarkness);
			thisTargetDarkness = targetDarkness;
		}
		readonly IUIImage thisImage;
		readonly float thisTargetDarkness;
		protected override float GetLatestInitialValueDifference(){
			float curD = thisImage.GetCurrentDarkness();
			float diff = thisTargetDarkness - curD;
			return diff;
		}
		protected override IImageDarknessInterpolator InstantiateInterpolatorWithValues(){
			IImageDarknessInterpolator irper = new ImageDarknessInterpolator(thisImage, thisImage.GetCurrentDarkness(), thisTargetDarkness);
			return irper;
		}
	}
}
