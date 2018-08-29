using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IFixedRectConstraintValueData{
		Vector2 GetValue();
	}
	public struct ConstantFixedRectConstraintValueData: IFixedRectConstraintValueData{
		public ConstantFixedRectConstraintValueData(Vector2 constantValue){
			thisConstantValue = constantValue;
		}
		readonly Vector2 thisConstantValue;
		public Vector2 GetValue(){return thisConstantValue;}
	}
	public struct ReferenceFixedRectConstraintValueData: IFixedRectConstraintValueData{
		public ReferenceFixedRectConstraintValueData(
			Vector2 referenceRectLength, Vector2 relativeLength
		){
			thisReferenceRectLength = referenceRectLength;
			thisRelativeLength = relativeLength;
		}
		readonly Vector2 thisReferenceRectLength;
		readonly Vector2 thisRelativeLength;
		public Vector2 GetValue(){
			return new Vector2(
				thisReferenceRectLength.x * thisRelativeLength.x,
				thisReferenceRectLength.y * thisRelativeLength.y
			);
		}
	}
}
