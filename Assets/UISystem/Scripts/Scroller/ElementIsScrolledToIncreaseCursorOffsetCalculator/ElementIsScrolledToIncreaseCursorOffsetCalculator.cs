using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IElementIsScrolledToIncreaseCursorOffsetCalculator{
		bool Calculate(float deltaPosOnAxis, float scrollerElementLocalPosOnAxis, int dimension);
	}
	public class ElementIsScrolledToIncreaseCursorOffsetCalculator : IElementIsScrolledToIncreaseCursorOffsetCalculator {
		public ElementIsScrolledToIncreaseCursorOffsetCalculator(IScroller scroller){
			thisScroller = scroller;
		}
		readonly IScroller thisScroller;
		public bool Calculate(float deltaPosOnAxis, float scrollerElementLocalPosOnAxis, int dimension){
		float cursorOffsetInPixel = thisScroller.GetElementCursorOffsetInPixel(scrollerElementLocalPosOnAxis, dimension);
			if(deltaPosOnAxis == 0f){
				return false;
			}else{
				if(cursorOffsetInPixel == 0f)
					return false;
				else{
					if(cursorOffsetInPixel < 0f)//too right
						return deltaPosOnAxis > 0f;
					else//displacement > 0f: too left
						return deltaPosOnAxis < 0f;
				}
			}
		}
	}
}
