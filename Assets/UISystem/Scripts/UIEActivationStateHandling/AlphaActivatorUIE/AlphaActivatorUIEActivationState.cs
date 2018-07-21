﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DKUtility;

namespace UISystem{
	public class AlphaActivatorUIEActivatingState: AbsUIEActivatingState{
		public AlphaActivatorUIEActivatingState(IUIEActivationStateEngine engine, IAlphaActivatorUIElement alphaActivatorUIE, IUISystemProcessFactory processFactory): base(engine, alphaActivatorUIE, processFactory){
		}
		protected override IUIEActivationProcess CreateUIEActivationProcess(IUISystemProcessFactory processFactory){
			return processFactory.CreateAlphaActivatorUIEActivationProcess(this, true, (IAlphaActivatorUIElement)thisUIElement);
		}
	}
	public class AlphaActivatorUIEDeactivatingState: AbsUIEDeactivatingState{
		public AlphaActivatorUIEDeactivatingState(IUIEActivationStateEngine engine, IAlphaActivatorUIElement alphaActivatorUIE, IUISystemProcessFactory processFactory): base(engine, alphaActivatorUIE, processFactory){
		}
		protected override IUIEActivationProcess CreateUIEActivationProcess(IUISystemProcessFactory processFactory){
			return processFactory.CreateAlphaActivatorUIEActivationProcess(this, false, (IAlphaActivatorUIElement)thisUIElement);
		}
	}
}
