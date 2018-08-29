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
		
		/* Rect Calculation fields */
			public RectConstraintType firstConstraintType;
			public FixedRectValueType firstFixedConstraintValueType;
			public RectTransform firstFixedConstraintReferenceRect;
			public Vector2 firstConstraintValue; 

			public RectConstraintType secondConstraintType;
			public FixedRectValueType secondFixedConstraintValueType;
			public RectTransform secondFixedConstraintReferenceRect;
			public Vector2 secondConstraintValue; 
		/*  */
		protected override void Awake(){
			base.Awake();
			MakeSureEachConstraintsAreProperlySet(
				firstConstraintType,
				firstFixedConstraintValueType,
				firstFixedConstraintReferenceRect
			);
			MakeSureEachConstraintsAreProperlySet(
				secondConstraintType,
				secondFixedConstraintValueType,
				secondFixedConstraintReferenceRect
			);
			MakeSureCombinationOfConstraintsIsValid();
		}
		void MakeSureEachConstraintsAreProperlySet(
			RectConstraintType constraintType,
			FixedRectValueType fixedRectConstraintValueType,
			RectTransform referenceRect
		){
			if(
				constraintType == RectConstraintType.FixedGroupLength ||
				constraintType == RectConstraintType.FixedElementLength ||
				constraintType == RectConstraintType.FixedPadding 
			){
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
		void MakeSureConstraintsDoNotShareSameFixedConstraintType(){
			bool shared = false;
			if(firstConstraintType == RectConstraintType.FixedGroupLength)
				if(secondConstraintType == RectConstraintType.FixedGroupLength)
					shared = true;
			if(firstConstraintType == RectConstraintType.FixedElementLength)
				if(secondConstraintType == RectConstraintType.FixedElementLength)
					shared = true;
			if(firstConstraintType == RectConstraintType.FixedPadding)
				if(secondConstraintType == RectConstraintType.FixedPadding)
					shared = true;
			if(shared)
				throw new System.InvalidOperationException(
					"same fixedConstraintType is set on both first and second constraint"
				);
		}
		void MakeSureBothAreNotSetToRatioConstraint(){
			if(
				firstConstraintType == RectConstraintType.GroupToElementRatio ||
				firstConstraintType == RectConstraintType.ElementToPaddingRatio ||
				firstConstraintType == RectConstraintType.GroupToPaddingRatio 
			){
				if(
					secondConstraintType == RectConstraintType.GroupToElementRatio ||
					secondConstraintType == RectConstraintType.ElementToPaddingRatio ||
					secondConstraintType == RectConstraintType.GroupToPaddingRatio 
				)
					throw new System.InvalidOperationException(
						"at least one of constraints must be set to fixedRectConstraint"
					);
			}
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
		protected override void SetUpUIElementReferenceImple(){
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
				firstConstraintType,
				firstFixedConstraintReferenceRect,
				firstConstraintValue
			);
			IRectConstraint secondConstraint = CreateRectConstraint(
				secondConstraintType,
				secondFixedConstraintReferenceRect,
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
			RectConstraintType constraintType,
			RectTransform referenceRect,
			Vector2 constraintValue
		){
			IRectConstraint rectConstraint;
			if(ConstraintIsFixedType(constraintType)){
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
				switch(constraintType){
					case RectConstraintType.FixedGroupLength:
						rectConstraint = new FixedGroupLengthConstraint(
							fixedConstraintValue
						);
						break;
					case RectConstraintType.FixedElementLength:
						rectConstraint = new FixedElementLengthConstraint(
							fixedConstraintValue
						);
						break;
					case RectConstraintType.FixedPadding:
						rectConstraint = new FixedPaddingConstraint(
							fixedConstraintValue
						);
						break;
					default: 
						throw new System.InvalidOperationException(
							"fixed constraint must be one of the three"
						);
				}
			}else{// first constraint is ratio
				switch(constraintType){
					case RectConstraintType.GroupToElementRatio: 
						rectConstraint = new GroupToElementRatioRectConstraint(
							constraintValue
						);
						break;
					case RectConstraintType.ElementToPaddingRatio: 
						rectConstraint = new ElementToPaddingRatioRectConstraint(
							constraintValue
						);
						break;
					case RectConstraintType.GroupToPaddingRatio: 
						rectConstraint = new GroupToPaddingRatioRectConstraint(
							constraintValue
						);
						break;
					default:
						throw new System.InvalidOperationException(
							"ratio constraint must be one of three"
						);
				}
			}
			return rectConstraint;
		}
		public enum RectConstraintType{
			FixedGroupLength,
			FixedElementLength,
			FixedPadding,

			GroupToElementRatio,
			ElementToPaddingRatio,
			GroupToPaddingRatio
		}
		bool ConstraintIsFixedType(RectConstraintType type){
			return 
			type == RectConstraintType.FixedGroupLength ||
			type == RectConstraintType.FixedElementLength ||
			type == RectConstraintType.FixedPadding
			;
		}
		public enum FixedRectValueType{
			ConstantValue,
			ReferenceRect
		}
	}
}

