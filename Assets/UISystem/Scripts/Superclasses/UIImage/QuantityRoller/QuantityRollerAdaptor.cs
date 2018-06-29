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
	}
}
