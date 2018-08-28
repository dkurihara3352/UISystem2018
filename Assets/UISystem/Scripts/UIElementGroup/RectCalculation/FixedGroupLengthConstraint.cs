using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IFixedGroupLengthConstraint: IFixedRectConstraint{}
	public class FixedGroupLengthConstraint: AbsFixedRectConstraint, IFixedGroupLengthConstraint{
		public FixedGroupLengthConstraint(
			IFixedRectConstraintValue value
		): base(
			value
		){}
		public override void CalculateRects(
			IRectConstraint otherConstraint
		){
			thisGroupLength = thisConstraintValue.GetValue();
			thisElementLength = otherConstraint.CalcElementLengthFromFixedGroupLength();
			thisPadding = otherConstraint.CalcPaddingFromFixedGroupLength();
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
			return thisConstraintValue.GetValue();
		}
		public override Vector2 CalcPaddingFromFixedElementLength(){
			return CalcPaddingFromGroupAndElementLength();
		}
	}
}
