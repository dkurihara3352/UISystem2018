using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IRectCalculationData{
		IRectConstraint[] constraints{get;}
		void SetColumnAndRowCount(
			int numOfColumns, 
			int numOfRows
		);
	}
	public struct RectCalculationData: IRectCalculationData{
		public RectCalculationData(
			IRectConstraint[] constraints
		){
			thisConstraints = constraints;
		}
		readonly IRectConstraint[] thisConstraints;
		public IRectConstraint[] constraints{get{return thisConstraints;}}
		public void SetColumnAndRowCount(
			int numOfColumns,
			int numOfRows
		){
			foreach(IRectConstraint rectConstraint in thisConstraints)
				rectConstraint.SetColumnAndRowCount(
					numOfColumns, 
					numOfRows
				);
		}
	}
	public enum FixedRectConstraintType{
		None,
		FixedGroupRectLength,
		FixedGroupElementLength,
		FixedPadding
	}
	public enum FixedRectValueType{
		ConstantValue,
		ReferenceRect
	}
	public enum RatioRectConstraintType{
		None,
		GroupToElement,
		ElementToPadding,
		GroupToPadding
	}	
}
