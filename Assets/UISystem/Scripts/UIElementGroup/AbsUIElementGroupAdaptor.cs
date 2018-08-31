using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IUIElementGroupAdaptor: IUIAdaptor{
	}
	public abstract class AbsUIElementGroupAdaptor: UIAdaptor, IUIElementGroupAdaptor{
		public int columnCountConstraint;
		public int rowCountConstraint;
		public bool topToBottom;
		public bool leftToRight;
		public bool rowToColumn;
	
		protected override void Awake(){
			base.Awake();
			MakeSureConstraintIsProperlySet();
		}
		protected abstract void MakeSureConstraintIsProperlySet();

		protected override IUIElement CreateUIElement(IUIImage image){
			IUIElementGroupConstArg arg = new UIElementGroupConstArg(
				columnCountConstraint,
				rowCountConstraint,
				topToBottom,
				leftToRight,
				rowToColumn,

				thisDomainInitializationData.uim,
				thisDomainInitializationData.processFactory,
				thisDomainInitializationData.uiElementFactory,
				this,
				image,
				activationMode
			);
			IGenericUIElementGroup uie = new GenericUIElementGroup(arg);
			return uie;
		}
		protected override void SetUpUIElementReferenceImple(){
			base.SetUpUIElementReferenceImple();
			List<IUIElement> groupElements = GetGroupElements();
			IRectCalculationData rectCalculationData = CreateRectCalculationData(groupElements);

			IUIElementGroup uieGroup = (IUIElementGroup)this.GetUIElement();
			
			uieGroup.SetUpElements(groupElements);
			uieGroup.SetUpRects(rectCalculationData);
			uieGroup.PlaceElements();
		}
		protected abstract List<IUIElement> GetGroupElements();
		protected abstract IRectCalculationData CreateRectCalculationData(List<IUIElement> groupElements);

	}
}

