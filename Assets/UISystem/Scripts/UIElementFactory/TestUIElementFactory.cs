using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface ITestUIElementFactory: IUIElementFactory{
		IUIElement CreateUIElementWithIndexText(int index, Vector2 sizeDelta, IUISystemProcessFactory processFactory);
	}
	public class TestUIElementFactory : UIElementFactory, ITestUIElementFactory {

		public TestUIElementFactory(
			IUIManager uim, 
			Font textFont, 
			int fontSize, 
			Color imageColor, 
			float imageDefaultDarkness, 
			float imageDarkenedDarkness
		): base(
			uim
		){
			thisFont = textFont;
			thisImageColor = imageColor;
			thisFontSize = fontSize;
			thisImageDefaultDarkness = imageDefaultDarkness;
			thisImageDarkenedDarkness = imageDarkenedDarkness;
		}
		readonly Font thisFont;
		readonly Color thisImageColor;
		readonly int thisFontSize;
		readonly float thisImageDefaultDarkness;
		readonly float thisImageDarkenedDarkness;
		public IUIElement CreateUIElementWithIndexText(
			int index, 
			Vector2 sizeDelta, 
			IUISystemProcessFactory processFactory
		){
			IIndexElementAdaptorInitializationData initData = new IndexElementAdaptorInitializationData
			(
				index, 
				thisFont, 
				thisFontSize, 
				thisImageColor, 
				thisImageDefaultDarkness, 
				thisImageDarkenedDarkness
			);
			IIndexElementAdaptorInstantiationData instData = new IndexElementAdaptorInstantiationData(
				initData
			);
			IndexElementAdaptor uia = this.CreateInstatiableUIA<IndexElementAdaptor>(instData);
			IUIAdaptorBaseInitializationData baseInitializationData = new RootUIAActivationData
			(
				thisUIM, 
				processFactory, 
				this
			);
			uia.GetReadyForActivation(baseInitializationData, false);
			return uia.GetUIElement();
		}
	}
}
