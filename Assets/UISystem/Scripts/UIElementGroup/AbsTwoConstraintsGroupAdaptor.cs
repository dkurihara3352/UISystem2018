using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface ITwoConstraintsGroupAdaptor: IUIElementGroupAdaptor{}
	public abstract class AbsTwoConstraintsGroupAdaptor: AbsUIElementGroupAdaptor, ITwoConstraintsGroupAdaptor {
		/* Rect Calculation fields */
			public RectConstraintType firstConstraintType;
			public FixedRectValueType firstFixedConstraintValueType;
			public RectTransform firstFixedConstraintReferenceRect;
			public Vector2 firstConstraintValue; 

			public RectConstraintType secondConstraintType;
			public FixedRectValueType secondFixedConstraintValueType;
			public RectTransform secondFixedConstraintReferenceRect;
			public Vector2 secondConstraintValue; 
		/* making sure constraints are valid */
			protected override void MakeSureConstraintIsProperlySet(){
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
		/* Creating Calc Data */
			protected override IRectCalculationData CreateRectCalculationData(
				List<IUIElement> groupElements
			){
				IRectConstraint firstConstraint = CreateRectConstraint(
					firstConstraintType,
					firstFixedConstraintValueType,
					firstFixedConstraintReferenceRect,
					firstConstraintValue
				);
				IRectConstraint secondConstraint = CreateRectConstraint(
					secondConstraintType,
					secondFixedConstraintValueType,
					secondFixedConstraintReferenceRect,
					secondConstraintValue
				);
				IRectCalculationData data = new TwoConstriantsRectCalculationData(
					new IRectConstraint[2]{
						firstConstraint,
						secondConstraint
					}
				);
				
				return data;		
			}
			IRectConstraint CreateRectConstraint(
				RectConstraintType constraintType,
				FixedRectValueType fixedValueType,
				RectTransform referenceRect,
				Vector2 constraintValue
			){
				IRectConstraint rectConstraint;
				if(ConstraintIsFixedType(constraintType)){
					IFixedRectConstraintValueData fixedConstraintValueData = CreateFixedConstraintValueData(
						fixedValueType,
						referenceRect,
						constraintValue
					);
					
					switch(constraintType){
						case RectConstraintType.FixedGroupLength:
							rectConstraint = new FixedGroupLengthConstraint(
								fixedConstraintValueData
							);
							break;
						case RectConstraintType.FixedElementLength:
							rectConstraint = new FixedElementLengthConstraint(
								fixedConstraintValueData
							);
							break;
						case RectConstraintType.FixedPadding:
							rectConstraint = new FixedPaddingConstraint(
								fixedConstraintValueData
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
			IFixedRectConstraintValueData CreateFixedConstraintValueData(
				FixedRectValueType valuType,
				RectTransform referenceRect,
				Vector2 constraintValue
			){
				IFixedRectConstraintValueData result;
				switch(valuType){
					case FixedRectValueType.ConstantValue:
						result = new ConstantFixedRectConstraintValueData(
							constraintValue
						);
						break;
					case FixedRectValueType.ReferenceRect:
						result = new ReferenceFixedRectConstraintValueData(
							referenceRect.sizeDelta,
							constraintValue
						);
						break;
					case FixedRectValueType.RelativeToScreen:
						result = new RelativeToScreenRectConstraintValueData(
							constraintValue
						);
						break;
					default:
						throw new System.InvalidOperationException(
							"cannot be anything other than above"
						);
				}
				return result;
			}
			public enum RectConstraintType{
				FixedGroupLength,
				FixedElementLength,
				FixedPadding,

				GroupToElementRatio,
				ElementToPaddingRatio,
				GroupToPaddingRatio
			}
			protected bool ConstraintIsFixedType(RectConstraintType type){
				return 
				type == RectConstraintType.FixedGroupLength ||
				type == RectConstraintType.FixedElementLength ||
				type == RectConstraintType.FixedPadding
				;
			}
			public enum FixedRectValueType{
				ConstantValue,
				ReferenceRect,
				RelativeToScreen
			}
	}
}
