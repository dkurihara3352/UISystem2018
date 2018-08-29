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
	}
}
