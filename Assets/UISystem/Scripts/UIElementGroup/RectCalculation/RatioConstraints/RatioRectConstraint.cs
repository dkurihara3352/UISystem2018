using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UISystem{
	public interface IRatioRectConstraint: IRectConstraint{}
	public abstract class AbsRatioRectConstraint: AbsRectConstraint, IRatioRectConstraint{
		public AbsRatioRectConstraint(
			Vector2 value
		){
			thisConstantValue = MakeSureValueIsValid(value);
		}
		readonly Vector2 thisConstantValue;
		protected override Vector2 thisValue{get{return thisConstantValue;}}
		Vector2 MakeSureValueIsValid(Vector2 source){
			for(int i = 0; i < 2; i ++)
				if(source[i] == 0f)
					throw new System.InvalidOperationException(
						"source value's either vector component is wrongly set to 0"
					);
			return source;
		}
	}
}
