using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DKUtility;

namespace UISystem{
	public interface IPopUpProcess: IProcess{
		bool IsHiding();
	}
	public interface IAlphaPopUpProcess: IPopUpProcess{}
	public class AlphaPopUpProcess : AbsInterpolatorProcess<IGroupAlphaInterpolator>, IAlphaPopUpProcess {
		public AlphaPopUpProcess(IAlphaPopUpProcessConstArg arg): base(arg){
			thisPopUp = arg.popUp;
			thisEngine = arg.engine;
			thisHides = arg.hides;
			thisPopUpAdaptor = thisPopUp.GetUIAdaptor();
		}
		readonly IPopUp thisPopUp;
		readonly IUIAdaptor thisPopUpAdaptor;
		readonly IPopUpStateEngine thisEngine;
		readonly bool thisHides;
		public bool IsHiding(){
			return thisHides;
		}
		float targetAlpha{
			get{
				return thisHides? 0f: 1f;
			}
		}
		protected override float GetLatestInitialValueDifference(){
			/*  never gets called
			*/
			float curAlpha = thisPopUpAdaptor.GetGroupAlpha();
			return targetAlpha - curAlpha;
		}
		protected override IGroupAlphaInterpolator InstantiateInterpolatorWithValues(){
			IGroupAlphaInterpolator interpolator = new GroupAlphaInterpolator(
				thisPopUpAdaptor, 
				targetAlpha
			);
			return interpolator;
		}
		protected override void ExpireImple(){
			base.ExpireImple();
			if(thisHides)
				thisEngine.SwitchToHiddenState();
			else
				thisEngine.SwitchToShownState();
		}
	}





	public interface IAlphaPopUpProcessConstArg: IInterpolatorProcesssConstArg{
		IPopUp popUp{get;}
		IPopUpStateEngine engine{get;}
		bool hides{get;}
	}
	public class AlphaPopUpProcessConstArg: InterpolatorProcessConstArg, IAlphaPopUpProcessConstArg{
		public AlphaPopUpProcessConstArg(
			IProcessManager processManager,
			float expireTime,

			IPopUpStateEngine engine,
			IPopUp popUp,
			bool hides
		): base(
			processManager,
			ProcessConstraint.ExpireTime,
			expireTime,
			false
		){

			thisPopUp = popUp;
			thisHides = hides;
			thisEngine = engine;
		}
		readonly IPopUp thisPopUp;
		public IPopUp popUp{get{return thisPopUp;}}
		readonly bool thisHides;
		public bool hides{get{return thisHides;}}
		readonly IPopUpStateEngine thisEngine;
		public IPopUpStateEngine engine{get{return thisEngine;}}
	}
}
