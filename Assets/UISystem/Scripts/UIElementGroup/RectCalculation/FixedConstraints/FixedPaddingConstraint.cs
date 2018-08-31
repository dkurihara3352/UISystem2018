using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IFixedPaddingConstraint: IRectConstraint{}
	public class FixedPaddingConstraint: AbsFixedRectConstraint, IFixedPaddingConstraint{
		public FixedPaddingConstraint(
			IFixedRectConstraintValueData valueData
		): base(
			valueData
		){}
		public override void CalculateRects(IRectConstraint otherConstraint){
			thisRectCalculationData.SetPadding(
				thisValue
			);
			thisRectCalculationData.SetGroupLength(
				otherConstraint.CalcGroupLengthFromFixedPadding()
			);
			thisRectCalculationData.SetElementLength(
				otherConstraint.CalcElementLengthFromFixedPadding()
			);
		}
		public override Vector2 CalcElementLengthFromFixedGroupLength(){
			return CalcElementLengthFromFixedGroupLengthAndPadding(
				thisGroupLength,
				thisValue
			);
		}
		public override Vector2 CalcPaddingFromFixedGroupLength(){
			return thisValue;
		}
		public override Vector2 CalcGroupLengthFromFixedElementLength(){
			return CalcGroupLengthFromFixedElementLengthAndPadding(
				thisElementLength,
				thisValue
			);
		}
		public override Vector2 CalcPaddingFromFixedElementLength(){
			return thisValue;
		}
		public override Vector2 CalcGroupLengthFromFixedPadding(){
			throw new System.InvalidOperationException(
				"both constraints must not be FixedPaddingConstraint"
			);
		}
		public override Vector2 CalcElementLengthFromFixedPadding(){
			throw new System.InvalidOperationException(
				"both constraints must not be FixedPaddingConstraint"
			);
		}
	}
}
