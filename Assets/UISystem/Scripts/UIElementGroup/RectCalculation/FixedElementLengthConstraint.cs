using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IFixedElementLengthConstraint: IFixedRectConstraint{}
	public class FixedElementLengthConstraint: AbsFixedRectConstraint, IFixedElementLengthConstraint{
		public FixedElementLengthConstraint(
			IFixedRectConstraintValue value
		):base(
			value
		){}
		public override void CalculateRects(
			IRectConstraint otherConstraint
		){
			thisElementLength = thisConstraintValue.GetValue();
			thisGroupLength = otherConstraint.CalcGroupLengthFromFixedElementLength();
			thisPadding = otherConstraint.CalcPaddingFromFixedElementLength();
		}
		public override Vector2 CalcElementLengthFromFixedGroupLength(){

		}
	}
}
