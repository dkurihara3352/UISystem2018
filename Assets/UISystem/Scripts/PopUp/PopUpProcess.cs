using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DKUtility;

namespace UISystem{
	public interface IPopUpProcess: IProcess{}
	public interface IAlphaPopUpProcess: IPopUpProcess{}
	public class AlphaPopUpProcess : AbsInterpolatorProcess<IGroupAlphaInterpolator>, IAlphaPopUpProcess {
		public AlphaPopUpProcess(
			IProcessManager processManager,
			float expireTime,
			IPopUp popUp,
			IPopUpStateEngine engine,
			bool hides
		): base(
			processManager,
			ProcessConstraint.expireTime,
			expireTime,
			0.05f,
			false,
			null
		){
			thisPopUp = popUp;
			thisEngine = engine;
			thisHides = hides;
			thisPopUpAdaptor = popUp.GetUIAdaptor();
		}
		readonly IPopUp thisPopUp;
		readonly IUIAdaptor thisPopUpAdaptor;
		readonly IPopUpStateEngine thisEngine;
		readonly bool thisHides;
		float targetAlpha{
			get{
				return thisHides? 0f: 1f;
			}
		}
		public override void Expire(){
			base.Expire();
			if(thisHides)
				thisEngine.SwitchToHiddenState();
			else
				thisEngine.SwitchToShownState();
		}
		protected override float GetLatestInitialValueDifference(){
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
	}
}
