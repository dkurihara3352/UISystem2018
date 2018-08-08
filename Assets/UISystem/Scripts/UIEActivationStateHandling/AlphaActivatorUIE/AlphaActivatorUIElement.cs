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
			return ((IAlphaActivatorUIElementAdaptor)thisUIA).GetNormalizedGroupAlpha();
		}
		public void SetNormalizedGroupAlphaForActivation(float groupAlpha){
			((IAlphaActivatorUIElementAdaptor)thisUIA).SetNormalizedGroupAlpha(groupAlpha);
		}
	}
	public interface IAlphaActivatorUIElementAdaptor: IUIAdaptor{
		float GetNormalizedGroupAlpha();
		void SetNormalizedGroupAlpha(float groupAlpha);
	}
}
