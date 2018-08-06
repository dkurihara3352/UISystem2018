using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public class GenericUIElementGroupAdaptor : UIAdaptor, IUIElementGroupAdaptor {
		public int columnCountConstraint;
		public int rowCountConstraint;
		public bool topToBottom;
		public bool leftToRight;
		public bool rowToColumn;
		public Vector2 elementLength;
		public Vector2 padding;
		protected override IUIElement CreateUIElement(IUIImage image){
			IUIElementGroupConstArg arg = new UIElementGroupConstArg(columnCountConstraint, rowCountConstraint, topToBottom, leftToRight, rowToColumn, elementLength, padding, thisUIM, thisDomainActivationData.processFactory, thisDomainActivationData.uiElementFactory, this, image);
			IUIElementGroup uie = new GenericUIElementGroup(arg);
			return uie;
		}
	}
}
