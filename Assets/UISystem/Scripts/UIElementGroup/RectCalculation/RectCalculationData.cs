using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IRectCalculationData{
		IRectConstraint[] constraints{get;}
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
	public class RectCalculationData: IRectCalculationData{
		public RectCalculationData(
			IRectConstraint[] constraints
		){
			thisConstraints = constraints;
			foreach(IRectConstraint constraint in thisConstraints)
				constraint.SetRectCalculationData(this);
		}
		readonly IRectConstraint[] thisConstraints;
		public IRectConstraint[] constraints{get{return thisConstraints;}}
		public void SetColumnAndRowCount(
			int numOfColumns,
			int numOfRows
		){
			thisNumOfColumns = numOfColumns;
			thisNumOfRows = numOfRows;
		}
		int thisNumOfColumns;
		public int GetColumnCount(){
			return thisNumOfColumns;
		}
		int thisNumOfRows;
		public int GetRowCount(){
			return thisNumOfRows;
		}
		public void CalculateRects(){
			if(thisConstraints[0] is IFixedRectConstraint)
				((IFixedRectConstraint)thisConstraints[0]).CalculateRects(thisConstraints[1]);
			else
				((IFixedRectConstraint)thisConstraints[1]).CalculateRects(thisConstraints[0]);
		}
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
	public enum FixedRectConstraintType{
		None,
		FixedGroupRectLength,
		FixedGroupElementLength,
		FixedPadding
	}
	public enum FixedRectValueType{
		ConstantValue,
		ReferenceRect
	}
	public enum RatioRectConstraintType{
		None,
		GroupToElement,
		ElementToPadding,
		GroupToPadding
	}	
}
