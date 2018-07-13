using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DKUtility;

namespace UISystem.PickUpUISystem{
	public interface ITravelInterpolator: IInterpolator{
		void UpdateTravellingUIE(ITravelableUIE newTravelableUIE);
		void DisconnectFromDrivingInterpolatorProcess();
		/*  At the beginning (initialization?) store a ref to its driving 		process locally in order for premature termination
		*/
	}
	public class TravelInterpolator: AbsInterpolator, ITravelInterpolator{
		public TravelInterpolator(ITravelableUIE travelableUIE, IInterpolatorProcess drivingProcess){
			thisTravelableUIE = travelableUIE;
			thisDrivingProcess = drivingProcess;
		}
		ITravelableUIE thisTravelableUIE;
		public void UpdateTravellingUIE(ITravelableUIE newTravelableUIE){
			thisTravelableUIE = newTravelableUIE;
			/*  Updating prev travelableUIE's runningTravellingIrper field 		is taken care outside
			*/
		}
		IInterpolatorProcess thisDrivingProcess;
		public void DisconnectFromDrivingInterpolatorProcess(){
			thisDrivingProcess.DisconnectInterpolater(this);
			thisDrivingProcess = null;
		}
		protected override void InterpolateImple(float normalizedT){
			
		}
		public override void Terminate(){}
	}
}
