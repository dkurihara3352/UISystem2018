using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface ITurnImageDarknessProcess: IProcess{}
	public class TurnImageDarknessProcess: AbsProcess, ITurnImageDarknessProcess{
		public TurnImageDarknessProcess(IProcessManager procManager, IUIImage image, float targetDarkness): base(procManager){
			thisImage = image;
			targetDarkness = CheckAndMakeDarknessValueIsInRange(targetDarkness);
			thisTargetDarkness = targetDarkness;
		}
		readonly IUIImage thisImage;
		readonly float thisTargetDarkness;
		readonly float rateOfChange = 1f;
			/*  time it takes from complete darkness 0f to zero darkness 1f
				it's set to 1sec here
			*/
		readonly float diffThreshold = .05f;
		float elapsedT;
		float totalReqTime;
			/* actual time it takes from init to tarDarkness
				,this is computed in CalcAndSetComputationFields
			 */
		ImageDarknessInterpolator thisIrper;
		float CheckAndMakeDarknessValueIsInRange(float darkness){
			/* clamp the given value within 0 - 1 range */
			if(darkness < 0f)
				return 0f;
			else if( darkness > 1f)
				return 1f;
			else
				return darkness;
		}
		public override void Run(){
			ResetFields();
			float initDarkness = thisImage.GetCurrentDarkness();
			initDarkness = CheckAndMakeDarknessValueIsInRange(initDarkness);
			if(DifferenceIsBigEnough(thisTargetDarkness - initDarkness)){
				ImageDarknessInterpolator newIrper = new ImageDarknessInterpolator(thisImage, initDarkness, thisTargetDarkness);
				CalcAndSetComputationFields(initDarkness, thisTargetDarkness);
				thisIrper = newIrper;
				newIrper.Interpolate(0f);
				base.Run();
			}else{
				thisImage.SetDarkness(thisTargetDarkness);
				return;
			}
		}
		bool DifferenceIsBigEnough(float diff){
			if(diff >= 0f)
				return diff > diffThreshold;
			else
				return diff < -diffThreshold;
		}
		void CalcAndSetComputationFields(float initDarkness, float targetDarkness){
			/*	Comp detail
				Dx (max possible diff: constant)
				Tx (max possible total time: constant)
				r (rate of Change: constant)
				{
					Dx/Tx = r
					Dx = 1f
					Tx = 1f sec
					r = 1f
				}
				Da (actual diff)
				Ta(time it takes to irp actual diff)
				{
					Da/Ta = r
					Ta = Da/r
				}
				Ti (irperT)
				Te (elapsedT)
				{
					Ti = Te/Ta
				}
			*/
			float actualDiff = targetDarkness - initDarkness;
			if(actualDiff < 0f)
				actualDiff *= -1f;
			totalReqTime = actualDiff/rateOfChange;
		}
		void ResetFields(){
			thisIrper = null;
			elapsedT = 0f;
			totalReqTime = 0f;
		}
		public override void UpdateProcess(float deltaT){
			/* see above for comp detail */
			elapsedT += deltaT;
			float irperT = elapsedT/ totalReqTime;
			if(irperT < 1f)
				thisIrper.Interpolate(irperT);
			else{
				thisIrper.Interpolate(1f);
				this.Expire();
			}
		}
		public override void Reset(){
			ResetFields();
		}
	}
}
