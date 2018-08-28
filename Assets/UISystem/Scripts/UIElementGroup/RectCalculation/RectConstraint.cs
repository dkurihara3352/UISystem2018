using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IRectConstraint{
		void SetColumnAndRowCount(int numOfColumns, int numOfRows);
		Vector2 CalcElementLengthFromFixedGroupLength();
		Vector2 CalcPaddingFromFixedGroupLength();
		Vector2 CalcGroupLengthFromFixedElementLength();
		Vector2 CalcPaddingFromFixedElementLength();
	}
	public interface IFixedRectConstraint: IRectConstraint{
		Vector2 GetValue();
		void CalculateRects(
			IRectConstraint otherConstraint
		);
		Vector2 GetGroupLength();
		Vector2 GetElementLength();
		Vector2 GetPadding();
	}



	public abstract class AbsFixedRectConstraint: IFixedRectConstraint{
		public AbsFixedRectConstraint(
			IFixedRectConstraintValue value
		){
			thisConstraintValue = value;
		}
		protected readonly IFixedRectConstraintValue thisConstraintValue;
		public Vector2 GetValue(){
			return thisConstraintValue.GetValue();
		}
		public abstract void CalculateRects(
			IRectConstraint otherConstraint
		);
		public Vector2 GetGroupLength(){
			return thisGroupLength;
		}
		protected Vector2 thisGroupLength;
		public Vector2 GetElementLength(){
			return thisElementLength;
		}
		protected Vector2 thisElementLength;
		public Vector2 GetPadding(){
			return thisPadding;
		}
		protected Vector2 thisPadding;

		protected int thisNumOfColumns;
		protected int thisNumOfRows;
		public void SetColumnAndRowCount(
			int numOfColumns,
			int numOfRows
		){
			thisNumOfColumns = numOfColumns;
			thisNumOfRows = numOfRows;
		}
		public abstract Vector2 CalcElementLengthFromFixedGroupLength();
		public abstract Vector2 CalcPaddingFromFixedGroupLength();
		public abstract Vector2 CalcGroupLengthFromFixedElementLength();
		public abstract Vector2 CalcPaddingFromFixedElementLength();
		protected Vector2 CalcPaddingFromGroupAndElementLength(){
			Vector2 parcelLength = CalcParcelLengthFromFixedGroupLength();
			MakeSureElementLengthIsNotGreaterThanParcelLength(parcelLength);
			return new Vector2(
				(parcelLength.x - thisElementLength.x) / 2f,
				(parcelLength.y - thisElementLength.y) / 2f
			);
		}
		void MakeSureElementLengthIsNotGreaterThanParcelLength(Vector2 parcelLength){
			for(int i = 0; i < 2; i ++)
				if(thisElementLength[i] > parcelLength[i])
					throw new System.InvalidOperationException(
						"Element length is exceeding parcel. Make sure it is set small enough to fit"
					);
		}
		Vector2 CalcParcelLengthFromFixedGroupLength(){
			return new Vector2(
						thisGroupLength.x/ thisNumOfColumns,
						thisGroupLength.y/ thisNumOfRows
					);
		}
		// protected Vector2 CalcElementLengthFromGroupLengthAndPadding(){

		// }
		// protected Vector2 CalcRelativeRect(
		// 	Vector2 referenceRect,
		// 	Vector2 multiplier,
		// 	bool invert
		// ){

		// }
		// protected abstract Vector2 GetParcelLength(
		// 	int numOfColumns,
		// 	int numOfRows
		// ){

		// }
	}
	public interface IRatioRectConstraint: IRectConstraint{
		RatioRectConstraintType constraintType{get;}
		Vector2 GetValue();
	}
	public struct RatioRectConstraint: IRatioRectConstraint{
		public RatioRectConstraint(
			RatioRectConstraintType type,
			Vector2 value
		){
			thisType = type;
			thisValue = value;
		}
		readonly RatioRectConstraintType thisType;
		public RatioRectConstraintType constraintType{
			get{return thisType;}
		}
		readonly Vector2 thisValue;
		public Vector2 GetValue(){return thisValue;}
	}
}
