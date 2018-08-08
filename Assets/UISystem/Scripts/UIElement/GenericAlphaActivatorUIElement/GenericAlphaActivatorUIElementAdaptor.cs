using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UISystem{
	public interface IGenericAlphaActivatorUIElementAdaptor: IAlphaActivatorUIElementAdaptor{}
	[RequireComponent(typeof(CanvasGroup))]
	public class GenericAlphaActivatorUIElementAdaptor : UIAdaptor, IGenericAlphaActivatorUIElementAdaptor {
		public override void GetReadyForActivation(IUIAActivationData passedData){
			base.GetReadyForActivation(passedData);
			thisCanvasGroup = transform.GetComponent<CanvasGroup>();
		}
		CanvasGroup thisCanvasGroup;
		public float GetNormalizedGroupAlpha(){
			return MakeSureAlphaIsInRange(thisCanvasGroup.alpha);
		}
		public void SetNormalizedGroupAlpha(float normalizedAlpha){
			thisCanvasGroup.alpha = MakeSureAlphaIsInRange(normalizedAlpha);
		}
		float MakeSureAlphaIsInRange(float source){
			if(source < 0f)
				return 0f;
			else if(source > 1f)
				return 1f;
			return source;
		}
		protected override IUIElement CreateUIElement(IUIImage image){
			IUIElementConstArg arg = new UIElementConstArg(thisDomainActivationData.uim, thisDomainActivationData.processFactory, thisDomainActivationData.uiElementFactory, this, image);
			return new GenericAlphaActivatorUIElement(arg);
		}
	}
}
