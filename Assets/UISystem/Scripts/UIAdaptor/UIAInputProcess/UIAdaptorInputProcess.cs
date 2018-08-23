using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DKUtility;

namespace UISystem{
	public interface IUIAdaptorInputProcess: IProcess{}
	public abstract class AbsUIAdaptorInputProcess : AbsConstrainedProcess, IUIAdaptorInputProcess{

		public AbsUIAdaptorInputProcess(IUIAdaptorInputProcessConstArg arg): base(arg){
		
			thisState = arg.state;
			thisEngine = arg.engine;
		}
		readonly protected IUIAdaptorInputState thisState;
		readonly protected IUIAdaptorInputStateEngine thisEngine;
	}

	public interface IUIAdaptorInputProcessConstArg: IConstrainedProcessConstArg{
		IUIAdaptorInputState state{get;}
		IUIAdaptorInputStateEngine engine{get;}
	}
	public class UIADaptorInputProcessConstArg: ConstrainedProcessConstArg, IUIAdaptorInputProcessConstArg{
		public UIADaptorInputProcessConstArg(
			IProcessManager processManager,
			ProcessConstraint processConstraint,
			float expireT,

			IUIAdaptorInputState state,
			IUIAdaptorInputStateEngine engine
		): base(
			processManager,
			processConstraint,
			expireT
		){
			thisState = state;
			thisEngine = engine;
		}
		readonly IUIAdaptorInputState thisState;
		public IUIAdaptorInputState state{get{return thisState;}}
		readonly IUIAdaptorInputStateEngine thisEngine;
		public IUIAdaptorInputStateEngine engine{get{return thisEngine;}}
	}
}
