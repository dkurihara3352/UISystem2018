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
		public AbsSingleTravellerTravelProcess(ITravelProcessConstArg arg): base(arg){
			thisTravellingUIE = arg.travelableUIE;
		}
		protected ITravelableUIE thisTravellingUIE;
		public override void Run(){
			base.Run();
			thisTravellingUIE.SetRunningTravelProcess(this);
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
		protected override void ExpireImple(){
			base.ExpireImple();
			if(thisTravellingUIE.GetRunningTravelProcess() == this)
				thisTravellingUIE.SetRunningTravelProcess(null);
		}
	}
	public interface ITravelProcessConstArg: IProcessConstArg{
		ITravelableUIE travelableUIE{get;}
	}
	public class TravelProcessConstArg: ProcessConstArg, ITravelProcessConstArg{
		public TravelProcessConstArg(
			IProcessManager processManager,
			ITravelableUIE travelableUIE
		): base(
			processManager
		){
			thisTravelableUIE = travelableUIE;
		}
		readonly ITravelableUIE thisTravelableUIE;
		public ITravelableUIE travelableUIE{get{return thisTravelableUIE;}}
	}
}
