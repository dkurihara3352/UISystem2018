using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface ITestUIElementFactory: IUIElementFactory{
		IUIElement CreateUIElementWithIndexText(int index, Vector2 sizeDelta, IUISystemProcessFactory processFactory);
	}
	public class TestUIElementFactory : UIElementFactory, ITestUIElementFactory {

		public TestUIElementFactory(IUIManager uim, Font textFont, int fontSize, Color imageColor): base(uim){
			thisFont = textFont;
			thisImageColor = imageColor;
			thisFontSize = fontSize;
		}
		readonly Font thisFont;
		readonly Color thisImageColor;
		readonly int thisFontSize;
		public IUIElement CreateUIElementWithIndexText(int index, Vector2 sizeDelta, IUISystemProcessFactory processFactory){
			IUIElementWithIndexTextAdaptorInitializationData initData = new UIElementWithIndexTextAdaptorInitializationData(index, thisFont, thisFontSize, thisImageColor);
			IUIElementWithIndexTextAdaptorInstantiationData instData = new UIElementWithIndexTextAdaptorInstantiationData(sizeDelta, initData);
			UIElementWithIndexTextAdaptor uia = this.CreateInstatiableUIA<UIElementWithIndexTextAdaptor>(instData);
			IUIAActivationData activationData = new RootUIAActivationData(thisUIM, processFactory, this);
			uia.GetReadyForActivation(activationData);
			return uia.GetUIElement();
		}
	}
}
