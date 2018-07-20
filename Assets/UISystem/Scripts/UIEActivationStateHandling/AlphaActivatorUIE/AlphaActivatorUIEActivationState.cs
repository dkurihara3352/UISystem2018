using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DKUtility;

namespace UISystem{
	public class AlphaActivatorUIEActivatingState: AbsUIEActivatingState{
		public AlphaActivatorUIEActivatingState(IAlphaActivatorUIEActivationProcess process):base(process){}
		IAlphaActivatorUIEActivationProcess typedProcess{get{return (IAlphaActivatorUIEActivationProcess)thisProcess;}}
		public override void SetInitializationFields(IUIEActivationStateEngine engine, IUIElement uiElement){
			IAlphaActivatorUIElement alphaActivatorUIE = (IAlphaActivatorUIElement)uiElement;
			typedProcess.SetAlphaActivatorUIE(alphaActivatorUIE);
		}
	}
	public class AlphaActivatorUIEDeactivatingState: AbsUIEDeactivatingState{
		public AlphaActivatorUIEDeactivatingState(IAlphaActivatorUIEActivationProcess process): base(process){}
		IAlphaActivatorUIEActivationProcess typedProcess{get{return (IAlphaActivatorUIEActivationProcess)thisProcess;}}
		public override void SetInitializationFields(IUIEActivationStateEngine engine, IUIElement uiElement){
			IAlphaActivatorUIElement alphaActivatorUIE = (IAlphaActivatorUIElement)uiElement;
			typedProcess.SetAlphaActivatorUIE(alphaActivatorUIE);
		}
	}
}
