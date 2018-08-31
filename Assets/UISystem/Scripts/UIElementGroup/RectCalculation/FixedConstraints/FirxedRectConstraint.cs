using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IFixedRectConstraint: IRectConstraint{
		void CalculateRects(
			IRectConstraint otherConstraint
		);
	}
	public abstract class AbsFixedRectConstraint: AbsRectConstraint, IFixedRectConstraint{
		public AbsFixedRectConstraint(
			IFixedRectConstraintValueData valueData
		){
			thisValueData = valueData;
		}
		protected IFixedRectConstraintValueData thisValueData;
		protected override Vector2 thisValue{
			get{return thisValueData.GetValue();}
		}
		public abstract void CalculateRects(
			IRectConstraint otherConstraint
		);
		protected Vector2 CalcPaddingFromFixedGroupAndElementLength(
			Vector2 groupLength,
			Vector2 elementLength
		){
			int[] gridCounts = GetGridCounts();
			Vector2 numerator = new Vector2(
				groupLength.x - (gridCounts[0] * elementLength.x),
				groupLength.y - (gridCounts[1] * elementLength.y)
			);
			Vector2 denominator = new Vector2(
				gridCounts[0] + 1f,
				gridCounts[1] + 1f
			);
			return new Vector2(
				numerator.x / denominator.x,
				numerator.y / denominator.y
			);
		}
		protected Vector2 CalcElementLengthFromFixedGroupLengthAndPadding(
			Vector2 groupLength,
			Vector2 padding	
		){
			int[] gridCounts = GetGridCounts();
			Vector2 numerator = new Vector2(
				groupLength.x - (gridCounts[0] + 1) * padding.x,
				groupLength.y - (gridCounts[1] + 1) * padding.y
			);	
			return new Vector2(
				numerator.x/ (gridCounts[0] * 1f),
				numerator.y/ (gridCounts[1] * 1f)
			);
		}
		protected Vector2 CalcGroupLengthFromFixedElementLengthAndPadding(
			Vector2 elementLength,
			Vector2 padding
		){
			int[] gridCounts = GetGridCounts();
			Vector2 totalElementLength = new Vector2(
				gridCounts[0] * elementLength.x,
				gridCounts[1] * elementLength.y
			);
			Vector2 totalPadding = new Vector2(
				(gridCounts[0] + 1) * padding.x,
				(gridCounts[1] + 1) * padding.y
			);
			return totalElementLength + totalPadding;
		}
	}
}
