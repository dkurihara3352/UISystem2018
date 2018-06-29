using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IDigitPanelSetAdaptor: IResizableRectUIAdaptor{
		void SetAdaptorFields(int digitPlace, Vector2 panelDim, Vector2 padding);
	}
	public class DigitPanelSetAdaptor: AbsResizableRectUIAdaptor<DigitPanelSet>, IDigitPanelSetAdaptor{
		public void SetAdaptorFields(int digitPlace, Vector2 panelDim, Vector2 padding){
			thisDigitPlace = digitPlace;
			thisPanelDim = panelDim;
			thisPadding = padding;
		}
		int thisDigitPlace;
		Vector2 thisPanelDim;
		Vector2 thisPadding;
		protected override DigitPanelSet CreateUIElement(){
			IDigitPanelSetConstArg arg = new DigitPanelSetConstArg(thisDomainActivationData.uim, this, null, thisDomainActivationData.tool, thisDigitPlace, thisPanelDim, thisPadding);
			DigitPanelSet digitPanelSet = new DigitPanelSet(arg);
			return digitPanelSet;
		}
	}
}
