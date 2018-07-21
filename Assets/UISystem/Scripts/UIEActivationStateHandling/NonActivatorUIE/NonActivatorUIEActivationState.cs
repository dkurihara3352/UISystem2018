using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DKUtility;

namespace UISystem{
	public class NonActivatorUIEActivatingState: AbsUIEActivatingState{
		public NonActivatorUIEActivatingState(IUIEActivationStateEngine engine, INonActivatorUIElement nonActivatorUIE, IUISystemProcessFactory processFactory): base(engine, nonActivatorUIE, processFactory){}
		protected override IUIEActivationProcess CreateUIEActivationProcess(IUISystemProcessFactory processFactory){
			return processFactory.CreateNonActivatorUIEActivationProcess(this, true);
		}
	}
	public class NonActivatorUIEDeactivatingState: AbsUIEDeactivatingState{
		public NonActivatorUIEDeactivatingState(IUIEActivationStateEngine engine, INonActivatorUIElement nonActivatorUIE, IUISystemProcessFactory processFactory): base(engine, nonActivatorUIE, processFactory){}
		protected override IUIEActivationProcess CreateUIEActivationProcess(IUISystemProcessFactory processFactory){
			return processFactory.CreateNonActivatorUIEActivationProcess(this, false);
		}
	}
}


