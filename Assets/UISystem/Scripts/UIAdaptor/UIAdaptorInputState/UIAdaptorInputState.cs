using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DKUtility;

namespace UISystem{
	public interface IRawInputHandler{
		void OnPointerDown(ICustomEventData eventData);
		void OnPointerUp(ICustomEventData eventData);
		void OnBeginDrag(ICustomEventData eventData);
		void OnDrag(ICustomEventData eventData);
		void OnPointerEnter(ICustomEventData eventData);
		void OnPointerExit(ICustomEventData eventData);
	}
	/* States */
	public interface IUIAdaptorInputState: IRawInputHandler, ISwitchableState{
	}
	public abstract class AbsUIAdaptorInputState: IUIAdaptorInputState{
		public AbsUIAdaptorInputState(
			IUIAdaptorInputStateConstArg arg
		){
			thisEngine = arg.engine;
		}
		protected readonly IUIAdaptorInputStateEngine thisEngine;
		public abstract void OnEnter();
		public abstract void OnExit();
		public abstract void OnPointerDown(ICustomEventData eventData);
		public abstract void OnPointerUp(ICustomEventData eventData);
		public abstract void OnBeginDrag(ICustomEventData eventData);
		public abstract void OnDrag(ICustomEventData eventData);
		public abstract void OnPointerEnter(ICustomEventData eventData);
		public abstract void OnPointerExit(ICustomEventData eventData);
	}
	public interface IUIAdaptorInputStateConstArg{
		IUIAdaptorInputStateEngine engine{get;}
	}
	public class UIAdaptorInputStateConstArg: IUIAdaptorInputStateConstArg{
		public UIAdaptorInputStateConstArg(
			IUIAdaptorInputStateEngine engine
		){
			thisEngine = engine;
		}
		readonly IUIAdaptorInputStateEngine thisEngine;
		public IUIAdaptorInputStateEngine engine{get{return thisEngine;}}
	}
}

