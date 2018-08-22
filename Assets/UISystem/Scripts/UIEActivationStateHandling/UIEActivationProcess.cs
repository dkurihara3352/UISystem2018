using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DKUtility;

namespace UISystem{
	public interface IUIEActivationProcess: IProcess{
	}
	public interface INonActivatorUIEActivationProcess: IUIEActivationProcess{}
	public class NonActivatorUIEActivationProcess: GenericWaitAndExpireProcess, INonActivatorUIEActivationProcess{
		public NonActivatorUIEActivationProcess(
			INonActivatorUIEActivationProcessConstArg arg
		): 
		base(
			arg
		){
			thisEngine = arg.engine;
			thisDoesActivate = arg.doesActivate;
		}
		protected readonly IUIEActivationStateEngine thisEngine;
		protected readonly bool thisDoesActivate;
		protected override void ExpireImple(){
			if(thisDoesActivate)
				thisEngine.SetToActivationCompletedState();
			else
				thisEngine.SetToDeactivationCompletedState();
		}
	}
	public interface INonActivatorUIEActivationProcessConstArg: IGenericWaitAndExpireProcessConstArg{
		IUIEActivationStateEngine engine{get;}
		bool doesActivate{get;}
	}
	public class NonActivatorUIEActivationProcessConstArg: GenericWaitAndExpireProcessConstArg, INonActivatorUIEActivationProcessConstArg{
		public NonActivatorUIEActivationProcessConstArg(
			IProcessManager processManager,
			float expireT,

			IUIEActivationStateEngine engine,
			bool doesActivate
		): base(
			processManager,
			expireT
		){
			thisEngine = engine;
			thisDoesActivate = doesActivate;
		}
		readonly IUIEActivationStateEngine thisEngine;
		public IUIEActivationStateEngine engine{get{return thisEngine;}}
		readonly bool thisDoesActivate;
		public bool doesActivate{get{return thisDoesActivate;}}
	}
}
