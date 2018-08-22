using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IPointerUpInputState: IUIAdaptorInputState{}
	public abstract class AbsPointerUpInputState: AbsUIAdaptorInputState, IPointerUpInputState{
		public AbsPointerUpInputState(
			IUIAdaptorInputStateConstArg arg
		): base(
			arg
		){}

		public override void OnPointerUp(ICustomEventData eventData){
			throw new System.InvalidOperationException("OnPointerUp should not be called while pointer is already held up");
		}
		public override void OnBeginDrag(ICustomEventData eventData){
			throw new System.InvalidOperationException("OnBeginDrag should not be called while pointer is held up");
		}
		public override void OnDrag(ICustomEventData eventData){
			throw new System.InvalidOperationException("OnDrag should be impossible when pointer is held up, something's wrong");
		}
		public override void OnPointerEnter(ICustomEventData eventData){return;}
		public override void OnPointerExit(ICustomEventData eventData){return;}
	}
	
	public abstract class AbsPointerUpInputProcessState<T>: AbsPointerUpInputState where T: class, IUIAdaptorInputProcess{
		public AbsPointerUpInputProcessState(
			IPointerUpInputProcessStateConstArg arg
		): base(
			arg
		){
			thisProcessFactory = arg.processFactory;
		}
		protected readonly IUISystemProcessFactory thisProcessFactory;
		protected T thisProcess;
		public override void OnEnter(){
			thisProcess = CreateProcess();
			thisProcess.Run();
		}
		public override void OnExit(){
			if(thisProcess.IsRunning())
				thisProcess.Stop();
			thisProcess = null;
		}
		protected abstract T CreateProcess();
		public virtual void ExpireProcess(){
			if(thisProcess.IsRunning())
				thisProcess.Expire();
		}
	}

	public interface IPointerUpInputProcessStateConstArg: IUIAdaptorInputStateConstArg{
		IUISystemProcessFactory processFactory{get;}
	}
	public class PointerUpInputProcessStateConstArg: UIAdaptorInputStateConstArg, IPointerUpInputProcessStateConstArg{
		public PointerUpInputProcessStateConstArg(
			IUIAdaptorInputStateEngine engine,
			IUISystemProcessFactory processFactory
		): base(
			engine
		){
			thisProcessFactory = processFactory;
		}
		readonly IUISystemProcessFactory thisProcessFactory;
		public IUISystemProcessFactory processFactory{get{return thisProcessFactory;}}
	}
}
