using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IElementCursorOffsetInPixelCalculator{
		float Calculate(float scrollerElementLocalPosiOnAxis, int dimension);
	}
	public class ElementCursorOffsetInPixelCalculator : IElementCursorOffsetInPixelCalculator {
		public ElementCursorOffsetInPixelCalculator(
			IScroller scroller,
			Vector2 cursorLength,
			Vector2 cursorLocalPosition,
			Vector2 scrollerElementLength
		){
			thisScroller = scroller;
			thisCursorLength = cursorLength;
			thisCursorLocalPosition = cursorLocalPosition;
			thisScrollerElementLength = scrollerElementLength;
		}

		readonly IScroller thisScroller;
		readonly Vector2 thisCursorLength;
		readonly Vector2 thisCursorLocalPosition;
		readonly Vector2 thisScrollerElementLength;
		public float Calculate(float scrollerElementLocalPosOnAxis, int dimension){
			if(thisScroller.ScrollerElementIsUndersizedTo(thisCursorLength, dimension)){
				return thisCursorLocalPosition[dimension] - scrollerElementLocalPosOnAxis;
			}
			else{
				float elementNormalizedCursoredPos = thisScroller.GetNormalizedCursoredPositionOnAxis(scrollerElementLocalPosOnAxis, dimension);
				float normalizedOffset = elementNormalizedCursoredPos;
				if(elementNormalizedCursoredPos <= 1f && elementNormalizedCursoredPos >= 0f)
					normalizedOffset = 0f;
				else if(elementNormalizedCursoredPos > 1f)
					normalizedOffset = elementNormalizedCursoredPos - 1f;
				return normalizedOffset * (thisScrollerElementLength[dimension]- thisCursorLength[dimension]);
			}
		}
	}
}
