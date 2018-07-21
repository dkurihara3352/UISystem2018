using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DKUtility;

namespace UISystem{
	public interface IAlphaActivatorUIElement: IUIElement{
		float GetNormalizedGroupAlphaForActivation();
		void SetNormalizedGroupAlphaForActivation(float groupAlpha);
	}
	public abstract class AbsAlphaActivatorUIElement: AbsUIElement, IAlphaActivatorUIElement{
		public AbsAlphaActivatorUIElement(IUIElementConstArg arg):base(arg){}
		protected override IUIEActivationStateEngine CreateUIEActivationStateEngine(){
			return new AlphaActivatorUIEActivationStateEngine(thisProcessFactory, this);
		}
		public float GetNormalizedGroupAlphaForActivation(){
			return ((IAlphaActivatorUIAdaptor)thisUIA).GetNormalizedGroupAlpha();
		}
		public void SetNormalizedGroupAlphaForActivation(float groupAlpha){
			((IAlphaActivatorUIAdaptor)thisUIA).SetNormalizedGroupAlpha(groupAlpha);
		}
	}
	public interface IAlphaActivatorUIAdaptor: IUIAdaptor{
		float GetNormalizedGroupAlpha();
		void SetNormalizedGroupAlpha(float groupAlpha);
	}
}
