using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IRectCalculationData{
		void SetColumnAndRowCount(
			int numOfColumns, 
			int numOfRows
		);
		int GetColumnCount();
		int GetRowCount();
		void CalculateRects();
		Vector2 groupLength{get;}
		Vector2 elementLength{get;}
		Vector2 padding{get;}
		void SetGroupLength(Vector2 groupLength);
		void SetElementLength(Vector2 elementLength);
		void SetPadding(Vector2 padding);
	}
	public abstract class AbsRectCalculationData: IRectCalculationData{
		public void SetColumnAndRowCount(
			int numOfColumns,
			int numOfRows
		){
			thisNumOfColumns = numOfColumns;
			thisNumOfRows = numOfRows;
		}
		protected int thisNumOfColumns;
		public int GetColumnCount(){
			return thisNumOfColumns;
		}
		protected int thisNumOfRows;
		public int GetRowCount(){
			return thisNumOfRows;
		}
		public abstract void CalculateRects();
		Vector2 thisGroupLength;
		public void SetGroupLength(Vector2 groupLength){
			thisGroupLength = groupLength;
		}
		public Vector2 groupLength{get{return thisGroupLength;}}
		Vector2 thisElementLength;
		public void SetElementLength(Vector2 elementLength){
			thisElementLength = elementLength;
		}
		public Vector2 elementLength{get{return thisElementLength;}}
		Vector2 thisPadding;
		public void SetPadding(Vector2 padding){
			thisPadding = padding;
		}
		public Vector2 padding{get{return thisPadding;}}
	}


	public interface ITwoConstraintsRectCalculationData: IRectCalculationData{
		IRectConstraint[] constraints{get;}
	}
	public class TwoConstriantsRectCalculationData: AbsRectCalculationData, ITwoConstraintsRectCalculationData{
		public TwoConstriantsRectCalculationData(
			IRectConstraint[] constraints
		){
			thisConstraints = constraints;
			foreach(IRectConstraint constraint in thisConstraints)
				constraint.SetRectCalculationData(this);
		}
		readonly IRectConstraint[] thisConstraints;
		public IRectConstraint[] constraints{get{return thisConstraints;}}
		public override void CalculateRects(){
			if(thisConstraints[0] is IFixedRectConstraint)
				((IFixedRectConstraint)thisConstraints[0]).CalculateRects(thisConstraints[1]);
			else
				((IFixedRectConstraint)thisConstraints[1]).CalculateRects(thisConstraints[0]);
		}
	}


	public interface IScrollerConstraintRectCalculationData: IRectCalculationData{

	}
	public class ScrollerConstraintRectCalculationData: AbsRectCalculationData, IScrollerConstraintRectCalculationData{
		public ScrollerConstraintRectCalculationData(
			Vector2 elementToPaddingRatio,
			IUIAdaptor parentUIAdaptor,
			Vector2 groupLengthAsNonScrollerElement
		){
			thisElementToPaddingRatio = elementToPaddingRatio;
			if(parentUIAdaptor != null){
				if(parentUIAdaptor is IUIElementGroupScrollerAdaptor){
					parentIsGroupScroller = true;
					IUIElementGroupScrollerAdaptor groupScrollerAdaptor = (IUIElementGroupScrollerAdaptor)parentUIAdaptor;
					thisGroupScrollerLength = groupScrollerAdaptor.GetRect().size;
					thisCursorSize = groupScrollerAdaptor.GetCursorSize();
					return;
				}
			}
			thisGroupLengthAsNonScrollerElement = groupLengthAsNonScrollerElement;
		}
		bool parentIsGroupScroller = false;
		readonly Vector2 thisElementToPaddingRatio;
		readonly Vector2 thisGroupScrollerLength;
		readonly Vector2 thisGroupLengthAsNonScrollerElement;
		readonly int[] thisCursorSize;
		int[] thisGridCounts{
			get{return new int[]{thisNumOfColumns, thisNumOfRows};}
		}
		int[] thisElementsArrayCount{
			get{
				if(parentIsGroupScroller)
					return thisCursorSize;
				else
					return thisGridCounts;
			}
		}
		Vector2 referenceLength{
			get{
				if(parentIsGroupScroller)
					return thisGroupScrollerLength;
				else
					return thisGroupLengthAsNonScrollerElement;
			}
		}
		public override void CalculateRects(){

			Vector2 groupScrollerLength = thisGroupScrollerLength;
			Vector2 elementLength = CalcElementLength();
			Vector2 padding = CalcPadding(
				elementLength
			);
			Vector2 groupLength = CalcGroupLength(
				elementLength,
				padding
			);
			SetGroupLength(groupLength);
			SetElementLength(elementLength);
			SetPadding(padding);
		}
		Vector2 CalcElementLength(){
			Vector2 result = new Vector2();
			for(int i = 0; i < 2; i++){
				if(thisElementToPaddingRatio[i] != 0f){
					float denominator = thisElementsArrayCount[i] * (thisElementToPaddingRatio[i] + 1f) + 1f;
					float multiplier = thisElementToPaddingRatio[i] / denominator;
					result[i] = referenceLength[i] * multiplier;
				}else{
					result[i] = referenceLength[i];
				}
			}
			return result;
		}
		Vector2 CalcPadding(
			Vector2 elementLength
		){
			Vector2 result = new Vector2();
			for(int i = 0; i < 2; i ++){
				if(thisElementToPaddingRatio[i] != 0f)
					result[i] = elementLength[i] / thisElementToPaddingRatio[i];
				else
					result[i] = 0f;
			}
			return result;
		}
		Vector2 GetInverseVector(Vector2 source){
			return new Vector2(
				1f/ source.x,
				1f/ source.y
			);
		}
		Vector2 CalcGroupLength(
			Vector2 elementLength,
			Vector2 padding
		){
			if(parentIsGroupScroller)
				return new Vector2(
					elementLength.x * thisGridCounts[0] + padding.x * (thisGridCounts[0] + 1), 
					elementLength.y * thisGridCounts[1] + padding.y * (thisGridCounts[1] + 1) 
				);
			else
				return thisGroupLengthAsNonScrollerElement;
		}
	}
}
