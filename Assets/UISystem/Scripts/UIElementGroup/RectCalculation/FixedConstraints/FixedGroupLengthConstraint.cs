using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IFixedGroupLengthConstraint: IRectConstraint{}
	public class FixedGroupLengthConstraint: AbsFixedRectConstraint, IFixedGroupLengthConstraint{
		public FixedGroupLengthConstraint(
			IFixedRectConstraintValueData valueData
		): base(
			valueData
		){}
		public override void CalculateRects(
			IRectConstraint otherConstraint
		){
			thisRectCalculationData.SetGroupLength(
				thisValue
			);

			thisRectCalculationData.SetElementLength(
				otherConstraint.CalcElementLengthFromFixedGroupLength()
			);

			thisRectCalculationData.SetPadding(
				otherConstraint.CalcPaddingFromFixedGroupLength()
			);
		}

		public override Vector2 CalcElementLengthFromFixedGroupLength(){
			throw new System.InvalidOperationException(
				"Both constraints cannot be fixed group length constraint"
			);
		}
		public override Vector2 CalcPaddingFromFixedGroupLength(){
			throw new System.InvalidOperationException(
				"Both constraints cannot be fixed group length constraint"
			);
		}
		public override Vector2 CalcGroupLengthFromFixedElementLength(){
			return thisValue;
		}
		public override Vector2 CalcPaddingFromFixedElementLength(){
			return CalcPaddingFromFixedGroupAndElementLength(
				thisValue,
				thisElementLength
			);	
		}
		public override Vector2 CalcGroupLengthFromFixedPadding(){
			return thisValue;
		}
		public override Vector2 CalcElementLengthFromFixedPadding(){
			return CalcElementLengthFromFixedGroupLengthAndPadding(
				thisValue,
				thisPadding
			);
		}
	}
}
