using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IElementToPaddingRatioRectConstraint: IRatioRectConstraint{}
	public class ElementToPaddingRatioRectConstraint: AbsRatioRectConstraint, IElementToPaddingRatioRectConstraint{
		public ElementToPaddingRatioRectConstraint(
			Vector2 value
		):base(
			value
		){}
		public override Vector2 CalcElementLengthFromFixedGroupLength(){
			Vector2 multiplier = GetMultiplier();
			return CalcRelativeRectLength(
				thisGroupLength,
				multiplier
			);
		}
		Vector2 GetMultiplier(){
			int[] gridCounts = GetGridCounts();
			Vector2 denominator = new Vector2(
				gridCounts[0] * (thisValue.x + 1f) + 1f,
				gridCounts[1] * (thisValue.y + 1f) + 1f
			);
			return new Vector2(
				thisValue.x/ denominator.x,
				thisValue.y/ denominator.y
			);
		}
		Vector2 GetModifiedMultiplier(){
			Vector2 multiplier = GetMultiplier();
			return new Vector2(
				multiplier.x * thisValue.x,
				multiplier.y * thisValue.y
			);
		}
		Vector2 GetDevidedMultiplier(){
			Vector2 multiplier = GetMultiplier();
			return new Vector2(
				multiplier.x / thisValue.x,
				multiplier.y / thisValue.y
			);
		}
		public override Vector2 CalcPaddingFromFixedGroupLength(){
			Vector2 devidedMult = GetDevidedMultiplier();
			return CalcRelativeRectLength(
				thisGroupLength,
				devidedMult
			);
		}
		public override Vector2 CalcGroupLengthFromFixedElementLength(){
			Vector2 multiplier = GetMultiplier();
			Vector2 inverseMult = GetInverseVector(multiplier);
			return CalcRelativeRectLength(
				thisElementLength,
				inverseMult
			);
		}
		public override Vector2 CalcPaddingFromFixedElementLength(){
			return CalcRelativeRectLength(
				thisElementLength,
				thisInverseValue
			);
		}
		public override Vector2 CalcGroupLengthFromFixedPadding(){
			Vector2 modifiedMultipler = GetModifiedMultiplier();
			return CalcRelativeRectLength(
				thisPadding,
				modifiedMultipler
			);
		}
		public override Vector2 CalcElementLengthFromFixedPadding(){
			return CalcRelativeRectLength(
				thisPadding,
				thisValue
			);
		}
	}
}

