using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DKUtility;

namespace UISystem{
	public interface IGroupAlphaInterpolator: IInterpolator{}
	public class GroupAlphaInterpolator: IGroupAlphaInterpolator{
		public GroupAlphaInterpolator(IUIAdaptor uiAdaptor, float targetGroupAlpha){
			thisUIAdaptor = uiAdaptor;
			thisTargetGroupAlpha = targetGroupAlpha;
			thisInitialGroupAlpha = uiAdaptor.GetGroupAlpha();
		}
		readonly IUIAdaptor thisUIAdaptor;
		readonly float thisTargetGroupAlpha;
		readonly float thisInitialGroupAlpha;
		public void Interpolate(float t){
			float newAlpha = Mathf.Lerp(thisInitialGroupAlpha, thisTargetGroupAlpha, t);
			thisUIAdaptor.SetGroupAlpha(newAlpha);
		}
		public void Terminate(){
		}
	}
}

