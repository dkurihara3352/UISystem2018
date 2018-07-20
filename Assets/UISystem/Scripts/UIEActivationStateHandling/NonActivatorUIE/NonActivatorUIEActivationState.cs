using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DKUtility;

namespace UISystem{
	public class NonActivatorUIEActivatingState: AbsUIEActivatingState{
		public NonActivatorUIEActivatingState(INonActivatorUIEActivationProcess process): base(process){}
	}
	public class NonActivatorUIEDeactivatingState: AbsUIEDeactivatingState{
		public NonActivatorUIEDeactivatingState(INonActivatorUIEActivationProcess process): base(process){}
	}
}


