using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IInterpolator{
		void Interpolate(float normalizedT);
		void Terminate();
	}
	public abstract class AbsInterpolator: IInterpolator{
		public void Interpolate(float zeroToOne){
			this.InterpolateImple(zeroToOne);
			if(zeroToOne >= 1f)
				this.Terminate();
		}
		protected abstract void InterpolateImple(float normalizedT);
		public abstract void Terminate();
	}
}
