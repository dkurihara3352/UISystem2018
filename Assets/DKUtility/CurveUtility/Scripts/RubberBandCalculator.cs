using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DKUtility.CurveUtility{
	public interface IRubberBandCalculator{
		float CalcRubberBandValue(float originalDisplacement);
	}
	public class RubberBandCalculator: IRubberBandCalculator{
		public RubberBandCalculator(float suppleness, float limitLength){
			suppleness = GetSupplenessInRrange(suppleness);
			thisSuppleness = suppleness;
			thisLimitLength = limitLength;
		}
		readonly float thisSuppleness;
		readonly float thisLimitLength;
		float GetSupplenessInRrange(float original){
			/*  if original exceeds 1f, rubberBand value exceeds original displacement
			*/
			if(original > 1f)
				return 1f;
			else
				return original;
		}
		public float CalcRubberBandValue(float originalDisplacement){
			float denominator = thisLimitLength + (thisSuppleness * originalDisplacement);
			float numerator = originalDisplacement * thisLimitLength * thisSuppleness;
			if(denominator != 0f && numerator != 0f)
				return numerator/ denominator;
			else
				return 0f;
		}
	}
}
