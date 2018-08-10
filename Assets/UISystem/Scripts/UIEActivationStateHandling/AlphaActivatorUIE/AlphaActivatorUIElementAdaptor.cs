using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UISystem{
	public interface IAlphaActivatorUIElementAdaptor: IUIAdaptor{
		float GetNormalizedGroupAlpha();
		void SetNormalizedGroupAlpha(float groupAlpha);
	}
	[RequireComponent(typeof(CanvasGroup))]
	public abstract class AbsAlphaActivatorUIElementAdaptor : UIAdaptor, IAlphaActivatorUIElementAdaptor {

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
	}
}
