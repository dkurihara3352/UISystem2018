using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IElementGroupOffsetCalculator{
		float Calculate(int dimension);
	}
	public class ElementGroupOffsetCalculator : IElementGroupOffsetCalculator {
		public ElementGroupOffsetCalculator(IUIElementGroup uieGroup, Vector2 groupElementLength, Vector2 padding, Vector2 cursorLocalPosition){
			thisUIElementGroup = uieGroup;
			thisGroupElementLength = groupElementLength;
			thisPadding = padding;
			thisCursorLocalPosition = cursorLocalPosition;
		}
		readonly IUIElementGroup thisUIElementGroup;
		readonly Vector2 thisGroupElementLength;
		readonly Vector2 thisPadding;
		readonly Vector2 thisCursorLocalPosition;

		public float Calculate(int dimension){
			float sectionLength = thisGroupElementLength[dimension] + thisPadding[dimension];
			Vector2 uieGroupCursoredPosition = thisCursorLocalPosition - thisUIElementGroup.GetLocalPosition();
			float modulo = uieGroupCursoredPosition[dimension] % sectionLength;
			if(modulo == 0f)
				return 0f;
			else{
				float cursoredPosNormalziedToSectionLength = modulo/ sectionLength;
				if(cursoredPosNormalziedToSectionLength < 0f)
					cursoredPosNormalziedToSectionLength = 1f + cursoredPosNormalziedToSectionLength;
				return cursoredPosNormalziedToSectionLength;
			}
		}

	}
}
