using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public class IndexElementGroupAdaptor : GenericUIElementGroupAdaptor {
		TestUIElementFactory testUIElementFactory;
		public int groupElementCount;
		public Font font;
		public int fontSize;
		public Color imageColor;
		public override void GetReadyForActivation(IUIAActivationData passedData){
			base.GetReadyForActivation(passedData);
			testUIElementFactory = new TestUIElementFactory(thisUIM, font, fontSize, imageColor);
			List<IUIElement> groupElements = CreateUIEs();
			IUIElementGroup uieGroup = this.GetUIElement() as IUIElementGroup;
			uieGroup.SetUpElements(groupElements);
		}
		List<IUIElement> CreateUIEs(){
			List<IUIElement> result = new List<IUIElement>();
			for(int i = 0; i < groupElementCount; i ++){
				result.Add(testUIElementFactory.CreateUIElementWithIndexText(i, groupElementLength, thisDomainActivationData.processFactory));
			}
			return result;
		}
	}
}
