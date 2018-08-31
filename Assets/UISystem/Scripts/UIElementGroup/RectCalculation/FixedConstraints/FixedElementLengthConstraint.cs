using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IFixedElementLengthConstraint: IRectConstraint{}
	public class FixedElementLengthConstraint: AbsFixedRectConstraint, IFixedElementLengthConstraint{
		public FixedElementLengthConstraint(
			IFixedRectConstraintValueData valueData
		):base(
			valueData
		){}
		public override void CalculateRects(
			IRectConstraint otherConstraint
		){
			thisRectCalculationData.SetElementLength(
				thisValue
			);
			thisRectCalculationData.SetGroupLength(
				otherConstraint.CalcGroupLengthFromFixedElementLength()
			);
			thisRectCalculationData.SetPadding(
				otherConstraint.CalcPaddingFromFixedElementLength()
			);
		}
		public override Vector2 CalcElementLengthFromFixedGroupLength(){
			return thisValue;
		}
		public override Vector2 CalcPaddingFromFixedGroupLength(){
			return CalcPaddingFromFixedGroupAndElementLength(
				thisGroupLength,
				thisValue
			);
		}
		public override Vector2 CalcGroupLengthFromFixedElementLength(){
			throw new System.InvalidOperationException(
				"both constraints cannot be FixedElementLengthConstraint"
			);
		}
		public override Vector2 CalcPaddingFromFixedElementLength(){
			throw new System.InvalidOperationException(
				"both constraints cannot be FixedElementLengthConstraint"
			);
		}
		public override Vector2 CalcGroupLengthFromFixedPadding(){
			return CalcGroupLengthFromFixedElementLengthAndPadding(
				thisValue,
				thisPadding
			);
		}
		public override Vector2 CalcElementLengthFromFixedPadding(){
			return thisValue;
		}
	}
}
