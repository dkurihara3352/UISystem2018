using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DKUtility{
	public interface IInterpolator{
		void Interpolate(float normalizedT);
		void Terminate();
	}
	public abstract class AbsInterpolator: IInterpolator{
		public void Interpolate(float zeroToOne){
			if(zeroToOne >= 1f){
				this.InterpolateImple(1f);
				Terminate();
			}else
				this.InterpolateImple(zeroToOne);
		}
		protected abstract void InterpolateImple(float normalizedT);
		public abstract void Terminate();
	}
}
