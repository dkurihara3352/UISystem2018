using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public class ImageDarknessInterpolator: AbsInterpolator{
		public ImageDarknessInterpolator(IUIImage image, float sourceDarkness, float targetDarkness){
			thisImage = image;
			thisInitDarkness = sourceDarkness;
			thisTargetDarkness = targetDarkness;
		}
		readonly IUIImage thisImage;
		readonly float thisInitDarkness;
		readonly float thisTargetDarkness;
		protected override void InterpolateImple(float normalizedT){
			float newDarkness = Mathf.Lerp(thisInitDarkness, thisTargetDarkness, normalizedT);
			thisImage.SetDarkness(newDarkness);
		}
		public override void Terminate(){return;}
	}
}
