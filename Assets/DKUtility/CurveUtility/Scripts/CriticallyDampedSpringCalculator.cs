using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DKUtility.CurveUtility{
	public interface ICachedCriticallyDampedSpringCalculator{
		float GetSpringValue(float normalizedT);
		float GetApproximateSpringValue(float normalizedT);
	}
	public interface IRealTimeCriticallyDampedSpringCalculator{
		float GetSpringValue(float elapsedT);
	}
	public abstract class AbsSpringValueCalculator: ICachedCriticallyDampedSpringCalculator{
		protected float[] thisCurvePoints;
		protected int thisResolution;
		const int minimumResolution = 10;
		public float GetSpringValue(float normalizedT){
			normalizedT = NormalizedValueInRange(normalizedT);
			float resolutionAdjustedNormalizedT = normalizedT * thisResolution;
			int indexOfCeiling = Mathf.CeilToInt(resolutionAdjustedNormalizedT);
			if(indexOfCeiling == 0)
				return thisCurvePoints[0];
			else{
				float ceilingF = thisCurvePoints[indexOfCeiling];
				int indexOfFloor = indexOfCeiling - 1;
				float floorF = thisCurvePoints[indexOfFloor];
				float interpolateValue = resolutionAdjustedNormalizedT - indexOfFloor;
				return Mathf.Lerp(floorF, ceilingF, interpolateValue);
			}
		}
		public float GetApproximateSpringValue(float normalizedT){
			normalizedT = NormalizedValueInRange(normalizedT);
			float resolutionAdjustedNormalizedT = normalizedT * thisResolution;
			int indexOfCeiling = Mathf.CeilToInt(resolutionAdjustedNormalizedT);
			return thisCurvePoints[indexOfCeiling];
		}
		float NormalizedValueInRange(float normalizedT){
			float result = normalizedT;
			if(normalizedT < 0f)
				result = 0f;
			else if( normalizedT > 1f)
				result = 1f;
			return result;
		}
	}
	public class NormalizedSpringValueCalculator: AbsSpringValueCalculator{
		public NormalizedSpringValueCalculator(int resolution){
			if(resolution < minimumResolution)
				resolution = minimumResolution;
			thisResolution = resolution;
			thisCurvePoints = BuildSpringLookUpArray();
		}
		const int minimumResolution = 10;
		const float coefficient = 5f;
		float[] BuildSpringLookUpArray(){
			List<float> curvePoints = new List<float>();
			for(int i = 0; i < thisResolution + 1; i ++){
				float t = 0f;
				if(i != 0)
					t = i/ (thisResolution * 1f);
				float x = coefficient * t;
				x += 1f;

				float e = 2.718f;
				float power = -t * coefficient;
				e = Mathf.Pow(e, power);

				x *= e;

				curvePoints.Add(1f - x);
			}
			curvePoints[0] = 0f;
			curvePoints[curvePoints.Count -1] = 1f;
			return curvePoints.ToArray();
		}
	}
	public interface IFullCriticallyDampedSpringCalculator: ICachedCriticallyDampedSpringCalculator{}
	public class FullCriticallyDampedSpringCalculator: AbsSpringValueCalculator, IFullCriticallyDampedSpringCalculator{
		public FullCriticallyDampedSpringCalculator(float initialValue, float terminalValue, float initialVelocity, float coefficient, int resolution){
			if(terminalValue == initialValue)
				throw new System.InvalidOperationException("initialValue and terminalValue must be different");
			else{
				thisResolution = resolution;
				thisCurvePoints = BuildSpringLookUpArray(initialValue, terminalValue, initialVelocity, coefficient, resolution);
			}
		}
		float[] BuildSpringLookUpArray(float initialValue, float terminalValue, float initialVelocity, float coefficient, int resolution){
			float deltaValue = initialValue - terminalValue;
			float[] curvePoints = new float[resolution + 1];
			for(int i = 0; i < resolution + 1; i ++){
				float t = 0f;
				if(i != 0)
					t = i/ (resolution * 1f);
				float x = deltaValue * coefficient;
				x += initialVelocity;
				x *= t;
				x += deltaValue;

				float e = 2.718f;
				float power = -t * coefficient;
				e = Mathf.Pow(e, power);

				x *= e;

				x += terminalValue;

				curvePoints[i] = x;
			}
			curvePoints[0] = initialValue;
			curvePoints[curvePoints.Length - 1] = terminalValue;
			return curvePoints;
		}
	}
	public class RealTimeCriticallyDampedSpringCalculator: IRealTimeCriticallyDampedSpringCalculator{
		public RealTimeCriticallyDampedSpringCalculator(float initialValue, float terminalValue, float initialVelocity, float springCoefficient){
			thisInitialValue = initialValue;
			thisTerminalValue = terminalValue;
			thisInitialVelocity = initialVelocity;
			thisSpringCoefficient = springCoefficient;
		}
		readonly float thisInitialValue;
		readonly float thisTerminalValue;
		readonly float thisInitialVelocity;
		readonly float thisSpringCoefficient;

		public float GetSpringValue(float elapsedT){
			float deltaValue = thisInitialValue - thisTerminalValue;
			
			float x = deltaValue * thisSpringCoefficient;
			x += thisInitialVelocity;
			x *= elapsedT;
			x += deltaValue;

			float e = 2.718f;
			float power = - elapsedT * thisSpringCoefficient;
			e = Mathf.Pow(e, power);

			x *= e;

			x += thisTerminalValue;
			return x;
		}
	}
}
