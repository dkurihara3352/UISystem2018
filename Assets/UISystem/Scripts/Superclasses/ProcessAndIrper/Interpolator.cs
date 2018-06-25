using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IInterpolator{
		void Interpolate(float zeroToOne);
		void InterpolateImple(float zeroToOne);
		void Terminate();
	}
	public abstract class AbsInterpolator: IInterpolator{
		public abstract void InterpolateImple(float zeroToOne);
		public void Interpolate(float zeroToOne){
			this.InterpolateImple(zeroToOne);
			if(zeroToOne >= 1f)
				this.Terminate();
		}
		public abstract void Terminate();
	}
}
