using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IGroupToPaddingRatioRectConstraint: IRatioRectConstraint{}
	public class GroupToPaddingRatioRectConstraint: AbsRatioRectConstraint, IGroupToPaddingRatioRectConstraint{
		public GroupToPaddingRatioRectConstraint(
			Vector2 value
		): base(
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
			Vector2 numerator = new Vector2(
				thisValue.x - (gridCounts[0] + 1f),
				thisValue.y - (gridCounts[1] + 1f)
			);
			Vector2 denominator = new Vector2(
				gridCounts[0] * thisValue.x, 
				gridCounts[1] * thisValue.y
			);
			Vector2 multiplier = new Vector2(
				numerator.x / denominator.x,
				numerator.y / denominator.y
			);
			return multiplier;
		}
		Vector2 GetModifiedMultiplier(){
			Vector2 multiplier = GetMultiplier();
			return new Vector2(
				multiplier.x * thisValue.x,
				multiplier.y * thisValue.y
			);
		}
		public override Vector2 CalcPaddingFromFixedGroupLength(){
			return CalcRelativeRectLength(
				thisGroupLength,
				thisInverseValue
			);
		}
		public override Vector2 CalcGroupLengthFromFixedElementLength(){
			Vector2 multiplier = GetMultiplier();
			Vector2 inverseMultiplier = GetInverseVector(multiplier);
			return CalcRelativeRectLength(
				thisElementLength,
				inverseMultiplier
			);
		}
		public override Vector2 CalcPaddingFromFixedElementLength(){
			Vector2 modifiedMult = GetModifiedMultiplier();
			Vector2 inverseModMult = GetInverseVector(modifiedMult);
			return CalcRelativeRectLength(
				thisElementLength,
				inverseModMult
			);
		}
		public override Vector2 CalcGroupLengthFromFixedPadding(){
			return CalcRelativeRectLength(
				thisPadding,
				thisValue
			);
		}
		public override Vector2 CalcElementLengthFromFixedPadding(){
			Vector2 modifiedMult = GetModifiedMultiplier();
			return CalcRelativeRectLength(
				thisPadding,
				modifiedMult
			);
		}
	}
}

