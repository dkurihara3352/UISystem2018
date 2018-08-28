using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IResizableRectUIAdaptor: IUIAdaptor{
		void SetRectDimension(float height, float width, float localPosX, float localPosY);
	}
	public abstract class AbsResizableRectUIAdaptor<T>: UIAdaptor, IResizableRectUIAdaptor where T: IUIElement{
		public void SetRectDimension(float height, float width, float localPosX, float localPosY){
			Rect rect = GetRect();
			rect.height = height;
			rect.width = width;
			this.transform.localPosition = new Vector2(localPosX, localPosY);
		}
	}
	public interface IQuantityRollerAdaptor: IResizableRectUIAdaptor, IInstatiableUIAdaptor{
	}
	public class QuantityRollerAdaptor: AbsResizableRectUIAdaptor<IQuantityRoller>, IQuantityRollerAdaptor{
		public void SetInitializationFields(IUIAInitializationData data){
			if(data is IQuantityRollerAdaptorInitializationData){
				IQuantityRollerAdaptorInitializationData qraInitData = (IQuantityRollerAdaptorInitializationData)data;
				thisMaxQuantity = qraInitData.maxQuantity;
				thisPanelDim = qraInitData.panelDim;
				thisPadding = qraInitData.padding;
				thisRollerNormalizedPos = qraInitData.normalizedPos;
			}
		}
		public int thisMaxQuantity;
		public Vector2 thisPanelDim;
		public Vector2 thisPadding;
		public Vector2 thisRollerNormalizedPos;
		protected override IUIElement CreateUIElement(IUIImage image){
			IQuantityRollerConstArg arg = new QuantityRollerConstArg(thisDomainInitializationData.uim, thisDomainInitializationData.processFactory, thisDomainInitializationData.uiElementFactory, this, image, thisMaxQuantity, thisPanelDim, thisPadding, thisRollerNormalizedPos);
			QuantityRoller quantityRoller = new QuantityRoller(arg);
			return quantityRoller;
		}
	}
	public interface IQuantityRollerAdaptorInitializationData: IUIAInitializationData{
		int maxQuantity{get;}
		Vector2 panelDim{get;}
		Vector2 padding{get;}
		Vector2 normalizedPos{get;}
	}
	public class QuantityRollerAdaptorInitializationData: IQuantityRollerAdaptorInitializationData{
		public QuantityRollerAdaptorInitializationData(int maxQuantity, Vector2 panelDim, Vector2 padding, Vector2 normalizedPos){
			thisMaxQuantity = maxQuantity;
			thisPanelDim = panelDim;
			thisPadding = padding;
			thisNormalizedPos = normalizedPos;
		}
		readonly int thisMaxQuantity;
		public int maxQuantity{get{return thisMaxQuantity;}}
		readonly Vector2 thisPanelDim;
		public Vector2 panelDim{get{return thisPanelDim;}}
		readonly Vector2 thisPadding;
		public Vector2 padding{get{return thisPadding;}}
		readonly Vector2 thisNormalizedPos;
		public Vector2 normalizedPos{get{return thisNormalizedPos;}}
	}
}
