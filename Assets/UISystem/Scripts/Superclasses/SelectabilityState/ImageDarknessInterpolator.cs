using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public class ImageDarknessInterpolator: AbsInterpolator{
		IUIImage image;
		float initDarkness;
		float targetDarkness;
		public ImageDarknessInterpolator(IUIImage image, float sourceDarkness, float targetDarkness){
			this.image = image;
			this.initDarkness = sourceDarkness;
			this.targetDarkness = targetDarkness;
		}
		public override void InterpolateImple(float zeroToOne){
			float newDarkness = Mathf.Lerp(initDarkness, targetDarkness, zeroToOne);
			image.SetDarkness(newDarkness);
		}
		public override void Terminate(){return;}
	}
}
