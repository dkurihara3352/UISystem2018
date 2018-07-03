using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UISystem{
	public interface IDigitPanelAdaptor: IUIAdaptor{
		void SetInitializationFields(Vector2 panelDim, float localPosY);
		void SetImageNumber(int number);
	}
	[RequireComponent(typeof(Text))]
	public class DigitPanelAdaptor: AbsResizableRectUIAdaptor<IDigitPanel>, IDigitPanelAdaptor{
		public Vector2 thisPanelDim;
		public float thisLocalPosY;
		public void SetInitializationFields(Vector2 panelDim, float localPosY){
			thisPanelDim = panelDim;
			thisLocalPosY = localPosY;
		}
		public override void GetReadyForActivation(IUIAActivationData passedData){
			base.GetReadyForActivation(passedData);
			thisText = this.GetComponent<Text>();
		}
		public Text thisText;
		public void SetImageNumber(int number){
			if(number == -1)
				thisText.text = "";
			else
				thisText.text = number.ToString();
		}
		protected override IDigitPanel CreateUIElement(){
			IDigitPanelConstArg arg = new DigitPanelConstArg(thisDomainActivationData.uim, this, null, thisDomainActivationData.tool, thisPanelDim, thisLocalPosY);
			return new DigitPanel(arg);
		}
	}
}
