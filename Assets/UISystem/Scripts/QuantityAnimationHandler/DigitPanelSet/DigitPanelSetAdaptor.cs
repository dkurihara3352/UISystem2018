using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IDigitPanelSetAdaptor: IResizableRectUIAdaptor{
		void SetInitializationFields(int digitPlace, Vector2 panelDim, Vector2 padding);
	}
	public class DigitPanelSetAdaptor: AbsResizableRectUIAdaptor<DigitPanelSet>, IDigitPanelSetAdaptor{
		public void SetInitializationFields(int digitPlace, Vector2 panelDim, Vector2 padding){
			thisDigitPlace = digitPlace;
			thisPanelDim = panelDim;
			thisPadding = padding;
		}
		int thisDigitPlace;
		Vector2 thisPanelDim;
		Vector2 thisPadding;
		protected override DigitPanelSet CreateUIElement(){
			IDigitPanelSetConstArg arg = new DigitPanelSetConstArg(thisDomainActivationData.uim, thisDomainActivationData.processFactory, thisDomainActivationData.uiElementFactory, this, null, thisDigitPlace, thisPanelDim, thisPadding);
			DigitPanelSet digitPanelSet = new DigitPanelSet(arg);
			return digitPanelSet;
		}
	}
}
