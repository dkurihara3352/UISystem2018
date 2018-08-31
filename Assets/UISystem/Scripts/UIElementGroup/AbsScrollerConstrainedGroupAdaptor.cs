using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IScrollerConstrainedGroupAdaptor: IUIElementGroupAdaptor{}
	public abstract class AbsScrollerConstrainedGroupAdaptor : AbsUIElementGroupAdaptor, IScrollerConstrainedGroupAdaptor{
		public Vector2 elementToPaddingRatio = new Vector2(10f, 10f);
		protected override void MakeSureConstraintIsProperlySet(){}
		protected override IRectCalculationData CreateRectCalculationData(
			List<IUIElement> groupElements
		){
			IUIAdaptor parentUIAdaptor = thisParentUIAdaptor;
			return new ScrollerConstraintRectCalculationData(
				elementToPaddingRatio,
				parentUIAdaptor,
				GetRect().size
			);
		}
		string GetUIAName(IUIAdaptor uia){
			if(uia == null)
				return "null";
			else
				return uia.GetName();
		}
	
	}
}
