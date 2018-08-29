using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IGroupToElementRatioRectConstraint: IRatioRectConstraint{}
	public class GroupToElementRatioRectConstraint: AbsRatioRectConstraint, IGroupToElementRatioRectConstraint{
		public GroupToElementRatioRectConstraint(
			Vector2 value
		): base(
			value
		){}
		public override Vector2 CalcElementLengthFromFixedGroupLength(){
			return CalcRelativeRectLength(
				thisGroupLength, 
				thisInverseValue
			);
		}
		public override Vector2 CalcPaddingFromFixedGroupLength(){
			Vector2 multiplier = GetMultiplier();
			return CalcRelativeRectLength(
				thisGroupLength,
				multiplier
			);
		}
		Vector2 GetMultiplier(){
			int[] gridCounts = GetGridCounts();
			Vector2 numerator = new Vector2(
				thisValue.x - gridCounts[0],
				thisValue.y - gridCounts[1]
			);
			Vector2 denominator = new Vector2(
				thisValue.x * (gridCounts[0] + 1f),
				thisValue.y * (gridCounts[1] + 1f)
			);
			return new Vector3(
				numerator.x / denominator.x,
				numerator.y / denominator.y
			);	
		}
		Vector2 GetModifiedMultiplier(){
			Vector2 multiplier = GetMultiplier();
			Vector2 modifiedMult = new Vector2(
				multiplier.x * thisValue.x,
				multiplier.y * thisValue.y
			);
			return modifiedMult;
		}
		public override Vector2 CalcGroupLengthFromFixedElementLength(){
			return CalcRelativeRectLength(
				thisElementLength,
				thisValue
			);
		}
		public override Vector2 CalcPaddingFromFixedElementLength(){
			Vector2 modifiedMult = GetModifiedMultiplier();
			return CalcRelativeRectLength(
				thisElementLength,
				modifiedMult
			);
		}
		public override Vector2 CalcGroupLengthFromFixedPadding(){
			Vector2 multiplier = GetMultiplier();
			Vector2 inverseMult = GetInverseVector(multiplier);
			return CalcRelativeRectLength(
				thisPadding,
				inverseMult
			);
		}
		public override Vector2 CalcElementLengthFromFixedPadding(){
			Vector2 modifiedMult = GetModifiedMultiplier();
			Vector2 inverseModMult = GetInverseVector(modifiedMult);
			return CalcRelativeRectLength(
				thisPadding,
				inverseModMult
			);
		}
	}
}

