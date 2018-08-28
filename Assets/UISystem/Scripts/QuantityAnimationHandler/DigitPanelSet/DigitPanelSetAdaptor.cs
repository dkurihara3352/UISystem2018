using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IDigitPanelSetAdaptor: IResizableRectUIAdaptor, IInstatiableUIAdaptor{
	}
	public class DigitPanelSetAdaptor: AbsResizableRectUIAdaptor<DigitPanelSet>, IDigitPanelSetAdaptor{
		public void SetInitializationFields(IUIAInitializationData data){
			if(data is IDigitPanelSetAdaptorInitializationData){
				IDigitPanelSetAdaptorInitializationData dpsaData = (IDigitPanelSetAdaptorInitializationData)data;
				thisDigitPlace = dpsaData.digitPlace;
				thisPanelDim = dpsaData.panelDim;
				thisPadding = dpsaData.padding;
			}else
				throw new System.ArgumentException("data must be of type IDigitPanelSetAdaptorInitializationData");
		}
		int thisDigitPlace;
		Vector2 thisPanelDim;
		Vector2 thisPadding;
		protected override IUIElement CreateUIElement(IUIImage image){
			IDigitPanelSetConstArg arg = new DigitPanelSetConstArg(thisDomainInitializationData.uim, thisDomainInitializationData.processFactory, thisDomainInitializationData.uiElementFactory, this, image, thisDigitPlace, thisPanelDim, thisPadding);
			DigitPanelSet digitPanelSet = new DigitPanelSet(arg);
			return digitPanelSet;
		}
	}
	public interface IDigitPanelSetAdaptorInitializationData: IUIAInitializationData{
		int digitPlace{get;}
		Vector2 panelDim{get;}
		Vector2 padding{get;}
	}
	public class DigitPanelSetAdaptorInitializationData: IDigitPanelSetAdaptorInitializationData{
		public DigitPanelSetAdaptorInitializationData(int digitPlace, Vector2 panelDim, Vector2 padding){
			thisDigitPlace = digitPlace;
			thisPanelDim = panelDim;
			thisPadding = padding;
		}
		readonly int thisDigitPlace;
		public int digitPlace{get{return thisDigitPlace;}}
		readonly Vector2 thisPanelDim;
		public Vector2 panelDim{get{return thisPanelDim;}}
		readonly Vector2 thisPadding;
		public Vector2 padding{get{return thisPadding;}}
	}
	public interface IDigitPanelSetInstantiationData: IInstantiableUIAdaptorInstantiationData{}
	public struct DigitPanelSetInstantiationData: IDigitPanelSetInstantiationData{
		public DigitPanelSetInstantiationData(Vector2 sizeDelta, IDigitPanelSetAdaptorInitializationData initializationData){
			thisSizeDelta = sizeDelta;
			thisInitData = initializationData;
		}
		readonly Vector2 thisSizeDelta;
		public Vector2 sizeDelta{get{return thisSizeDelta;}}
		readonly IUIAInitializationData thisInitData;
		public IUIAInitializationData initializationData{get{return thisInitData;}}
	}
}
