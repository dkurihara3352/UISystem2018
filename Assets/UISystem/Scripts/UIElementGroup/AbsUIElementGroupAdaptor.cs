using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IUIElementGroupAdaptor{
	}
	public abstract class AbsUIElementGroupAdaptor: UIAdaptor, IUIElementGroupAdaptor{
		public int columnCountConstraint;
		public int rowCountConstraint;
		public bool topToBottom;
		public bool leftToRight;
		public bool rowToColumn;
		
		/* Rect Calculation fields */
			public FixedRectConstraintType firstFixedConstraintType;
			public FixedRectValueType firstFixedConstraintValueType;
			public RectTransform firstFixedConstraintReferenceRect;
			public RatioRectConstraintType firstRatioConstraintType;
			public Vector2 firstConstraintValue; 

			public FixedRectConstraintType secondFixedConstraintType;
			public FixedRectValueType secondFixedConstraintValueType;
			public RectTransform secondFixedConstraintReferenceRect;
			public RatioRectConstraintType secondRatioConstraintType;
			public Vector2 secondConstraintValue; 
		/*  */
		protected override void Awake(){
			base.Awake();
			MakeSureEachConstraintsAreProperlySet(
				firstFixedConstraintType,
				firstFixedConstraintValueType,
				firstFixedConstraintReferenceRect,
				firstRatioConstraintType
			);
			MakeSureEachConstraintsAreProperlySet(
				secondFixedConstraintType,
				secondFixedConstraintValueType,
				secondFixedConstraintReferenceRect,
				secondRatioConstraintType
			);
			MakeSureCombinationOfConstraintsIsValid();
		}
		void MakeSureEachConstraintsAreProperlySet(
			FixedRectConstraintType fixedRectConstraintType,
			FixedRectValueType fixedRectConstraintValueType,
			RectTransform referenceRect,
			RatioRectConstraintType ratioRectConstraintType
		){
			if(fixedRectConstraintType == FixedRectConstraintType.None){
				if(ratioRectConstraintType == RatioRectConstraintType.None)
					throw new System.InvalidOperationException(
						"Constraint should be either of Fixed or Ratio"
					);
			}else{
				if(ratioRectConstraintType != RatioRectConstraintType.None)
					throw new System.InvalidOperationException(
						"Constraint cannot be both Fixed and Ratio at the same time"
					);
				if(fixedRectConstraintValueType == FixedRectValueType.ReferenceRect)
					if(referenceRect == null)
						throw new System.InvalidOperationException(
							"Reference rectTransform must be set"
						);
			}
		}
		void MakeSureCombinationOfConstraintsIsValid(){
			MakeSureConstraintsDoNotShareSameFixedConstraintType();
			MakeSureBothAreNotSetToRatioConstraint();
		}

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
		protected override void SetUpUIElementReference(){
			List<IUIElement> groupElements = GetGroupElements();
			IRectCalculationData rectCalculationData = CreateRectCalculationData(groupElements);

			IUIElementGroup uieGroup = (IUIElementGroup)this.GetUIElement();
			
			uieGroup.SetUpElements(groupElements);
			uieGroup.SetUpRects(rectCalculationData);
			uieGroup.PlaceElements();
		}
		protected abstract List<IUIElement> GetGroupElements();
		IRectCalculationData CreateRectCalculationData(List<IUIElement> groupElements){
			IRectConstraint firstConstraint = CreateRectConstraint(
				firstFixedConstraintType,
				firstFixedConstraintReferenceRect,
				firstRatioConstraintType,
				firstConstraintValue
			);
			IRectConstraint secondConstraint = CreateRectConstraint(
				secondFixedConstraintType,
				secondFixedConstraintReferenceRect,
				secondRatioConstraintType,
				secondConstraintValue
			);
			IRectCalculationData data = new RectCalculationData(
				new IRectConstraint[2]{
					firstConstraint,
					secondConstraint
				}
			);
			
			return data;		
		}
		IRectConstraint CreateRectConstraint(
			FixedRectConstraintType fixedConstraintType,
			RectTransform referenceRect,
			RatioRectConstraintType ratioConstraintType,
			Vector2 constraintValue
		){
			IRectConstraint rectConstraint;
			if(fixedConstraintType != FixedRectConstraintType.None){
				IFixedRectConstraintValueData fixedConstraintValue;
				if(firstFixedConstraintValueType == FixedRectValueType.ConstantValue)
					fixedConstraintValue = new ConstantFixedRectConstraintValueData(
						constraintValue
					);
				else
					fixedConstraintValue = new ReferenceFixedRectConstraintValueData(
						referenceRect.sizeDelta,
						constraintValue
					);
				rectConstraint = new AbsFixedRectConstraint(
					fixedConstraintType,
					fixedConstraintValue
				);
			}else{// first constraint is ratio
				rectConstraint = new RatioRectConstraint(
					ratioConstraintType,
					constraintValue
				);
			}
			return rectConstraint;
		}
	}
}

