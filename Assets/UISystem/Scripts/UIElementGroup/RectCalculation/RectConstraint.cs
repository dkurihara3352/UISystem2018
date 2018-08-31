using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IRectConstraint{
		void SetRectCalculationData(ITwoConstraintsRectCalculationData data);
		Vector2 CalcElementLengthFromFixedGroupLength();
		Vector2 CalcPaddingFromFixedGroupLength();
		Vector2 CalcGroupLengthFromFixedElementLength();
		Vector2 CalcPaddingFromFixedElementLength();
		Vector2 CalcGroupLengthFromFixedPadding();
		Vector2 CalcElementLengthFromFixedPadding();
	}
	public abstract class AbsRectConstraint: IRectConstraint{

		public void SetRectCalculationData(ITwoConstraintsRectCalculationData data){
			thisRectCalculationData = data;
		}
		protected abstract Vector2 thisValue{get;}
		protected Vector2 thisInverseValue{
			get{
				return GetInverseVector(thisValue);
			}
		}
		protected Vector2 GetInverseVector(Vector2 source){
			return new Vector2(
				1f/ source.x,
				1f/ source.y
			);
		}
		protected ITwoConstraintsRectCalculationData thisRectCalculationData;
		protected Vector2 thisGroupLength{
			get{
				return thisRectCalculationData.groupLength;
			}
		}
		protected Vector2 thisElementLength{
			get{
				return thisRectCalculationData.elementLength;
			}
		}
		protected Vector2 thisPadding{
			get{
				return thisRectCalculationData.padding;
			}
		}

		public abstract Vector2 CalcElementLengthFromFixedGroupLength();
		public abstract Vector2 CalcPaddingFromFixedGroupLength();
		public abstract Vector2 CalcGroupLengthFromFixedElementLength();
		public abstract Vector2 CalcPaddingFromFixedElementLength();
		public abstract Vector2 CalcGroupLengthFromFixedPadding();
		public abstract Vector2 CalcElementLengthFromFixedPadding();
		protected Vector2 CalcPaddingFromGroupAndElementLength(){
			int[] gridCounts = GetGridCounts();
			Vector2 numerator = new Vector2(
				thisGroupLength.x - (thisElementLength.x * gridCounts[0]),
				thisGroupLength.y - (thisElementLength.y * gridCounts[1])
			);
			int[] denominator = new int[]{
				gridCounts[0] + 1,
				gridCounts[1] + 1
			};
			return new Vector2(
				numerator.x/ denominator[0],
				numerator.y/ denominator[1]
			);
		}
		protected Vector2 CalcElementLengthFromFixedGroupLengthAndPadding(){
			Vector2 totalPadding = CalcTotalPadding();
			int[] gridCounts = GetGridCounts();
			Vector2 numerator = new Vector2(
				thisGroupLength.x - totalPadding.x,
				thisGroupLength.y - totalPadding.y
			);
			return new Vector2(
				numerator.x/ gridCounts[0],
				numerator.y/ gridCounts[1]
			);
		}
		protected int[] GetGridCounts(){
			return new int[]{
				thisRectCalculationData.GetColumnCount(),
				thisRectCalculationData.GetRowCount()
			};
		}
		protected Vector2 CalcGroupLengthFromFixedElementLengthAndPadding(){
			Vector2 parcelLength = new Vector2(
				thisElementLength.x + thisPadding.x,
				thisElementLength.y + thisPadding.y
			);
			int[] gridCounts = GetGridCounts();

			return new Vector2(
				parcelLength.x * gridCounts[0],
				parcelLength.y * gridCounts[1]
			);
		}
		void MakeSureLengthIsNotGreaterThanParcelLength(Vector2 length, Vector2 parcelLength){
			for(int i = 0; i < 2; i ++)
				if(length[i] > parcelLength[i])
					throw new System.InvalidOperationException(
						"length is exceeding parcel. Make sure it is set small enough to fit"
					);
		}
		protected Vector2 CalcRelativeRectLength(
			Vector2 referenceLength,
			Vector2 multipllier
		){
			return new Vector2(
				referenceLength.x * multipllier.x,
				referenceLength.y * multipllier.y
			);
		}
		protected Vector2 CalcTotalPadding(){
			int[] gridCounts = GetGridCounts();
			return new Vector2(
				thisPadding.x * (gridCounts[0] + 1),
				thisPadding.y * (gridCounts[1] + 1)
			);
		}
	}
}
