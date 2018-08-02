﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UISystem{
	public interface IDigitPanelAdaptor: IInstatiableUIAdaptor{
		void SetImageNumber(int number);
	}
	[RequireComponent(typeof(Text))]
	public class DigitPanelAdaptor: AbsResizableRectUIAdaptor<IDigitPanel>, IDigitPanelAdaptor{
		public Vector2 thisPanelDim;
		public float thisLocalPosY;
		public void SetInitializationFields(IUIAInitializationData data){
			if(data is IDigitPanelAdaptorInitializationData){
				IDigitPanelAdaptorInitializationData dpaData = (IDigitPanelAdaptorInitializationData)data;
				thisPanelDim = dpaData.panelDim;
				thisLocalPosY = dpaData.localPosY;
			}else
				throw new System.ArgumentException("data must be of type IDigitPanelAdaptorInitializationData");
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
		protected override IUIElement CreateUIElement(IUIImage image){
			IDigitPanelConstArg arg = new DigitPanelConstArg(thisDomainActivationData.uim, thisDomainActivationData.processFactory, thisDomainActivationData.uiElementFactory, this, image, thisPanelDim, thisLocalPosY);
			return new DigitPanel(arg);
		}
	}
	public interface IDigitPanelAdaptorInitializationData: IUIAInitializationData{
		Vector2 panelDim{get;}
		float localPosY{get;}
	}
	public class DigitPanelAdaptorInitializationData: IDigitPanelAdaptorInitializationData{
		public DigitPanelAdaptorInitializationData(Vector2 panelDim, float localPosY){
			thisPanelDim = panelDim;
			thisLocalPosY = localPosY;
		}
		readonly Vector2 thisPanelDim;
		public Vector2 panelDim{get{return thisPanelDim;}}
		readonly float thisLocalPosY;
		public float localPosY{get{return thisLocalPosY;}}
	}
}
