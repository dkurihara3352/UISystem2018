using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DKUtility;

namespace UISystem.PickUpUISystem{
	public interface ITravelProcess: IProcess{
		void UpdateTravellingUIEFromTo(ITravelableUIE sourceUIE, ITravelableUIE targetUIE);
		void UnregisterTravellingUIE(ITravelableUIE travellingUIE);
	}
	public abstract class AbsSingleTravellerTravelProcess: AbsProcess, ITravelProcess{
		public AbsSingleTravellerTravelProcess(ITravelableUIE travelableUIE, IProcessManager procMan): base(procMan){
			thisTravellingUIE = travelableUIE;
		}
		protected ITravelableUIE thisTravellingUIE;
		public override void Run(){
			base.Run();
			thisTravellingUIE.SetRunningTravelProcess(this);
		}
		public override void Stop(){
			base.Stop();
			RemoveRefToThisInTravellingUIE();
		}
		public override void Expire(){
			base.Expire();
			RemoveRefToThisInTravellingUIE();
		}
		void RemoveRefToThisInTravellingUIE(){
			if(thisTravellingUIE != null){
				thisTravellingUIE.SetRunningTravelProcess(null);
				thisTravellingUIE = null;
			}
		}
		public void UnregisterTravellingUIE(ITravelableUIE travellingUIE){
			if(thisTravellingUIE == null)
				throw new System.InvalidOperationException("this process has already been stopped or expired");
			else{
				if(travellingUIE != thisTravellingUIE)
					throw new System.InvalidOperationException("travellingUIE must be same as thisTravellingUIE");
				else{
					RemoveRefToThisInTravellingUIE();
					this.Stop();
				}
			}
		}
		public void UpdateTravellingUIEFromTo(ITravelableUIE sourceUIE, ITravelableUIE targetUIE){
			if(thisTravellingUIE == null)
				throw new System.InvalidOperationException("this prcess has already been stopped or expired");
			else{
				if(sourceUIE != thisTravellingUIE)
					throw new System.InvalidOperationException("souceUIE does not match thisTravellingUIE");
				else{
					sourceUIE.SetRunningTravelProcess(null);
					thisTravellingUIE = targetUIE;
					targetUIE.SetRunningTravelProcess(this);
				}
			}
		}
	}
	// public abstract class AbsMutipleTravellersTravelProcess: ITravelProcess{
	// }
}
