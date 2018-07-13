using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IResizableRectUIAdaptor: IUIAdaptor{
		void SetRectDimension(float height, float width, float localPosX, float localPosY);
	}
	public abstract class AbsResizableRectUIAdaptor<T>: AbsUIAdaptor<T>, IResizableRectUIAdaptor where T: IUIElement{
		public void SetRectDimension(float height, float width, float localPosX, float localPosY){
			Rect rect = GetRect();
			rect.height = height;
			rect.width = width;
			this.transform.localPosition = new Vector2(localPosX, localPosY);
		}
	}
	public interface IQuantityRollerAdaptor: IResizableRectUIAdaptor{
		void SetInitializationFields(int maxQuantity, Vector2 panelDim, Vector2 padding, Vector2 normalizedPos);
	}
	public class QuantityRollerAdaptor: AbsResizableRectUIAdaptor<IQuantityRoller>, IQuantityRollerAdaptor{
		public void SetInitializationFields(int maxQuantity, Vector2 panelDim, Vector2 padding, Vector2 normalizedPos){
			thisMaxQuantity = maxQuantity;
			thisPanelDim = panelDim;
			thisPadding = padding;
			thisRollerNormalizedPos = normalizedPos;
		}
		public int thisMaxQuantity;
		public Vector2 thisPanelDim;
		public Vector2 thisPadding;
		public Vector2 thisRollerNormalizedPos;
		protected override IQuantityRoller CreateUIElement(){
			IQuantityRollerConstArg arg = new QuantityRollerConstArg(thisDomainActivationData.uim, thisDomainActivationData.processFactory, thisDomainActivationData.uiElementFactory, this, null, thisMaxQuantity, thisPanelDim, thisPadding, thisRollerNormalizedPos);
			QuantityRoller quantityRoller = new QuantityRoller(arg);
			return quantityRoller;
		}
	}
}
