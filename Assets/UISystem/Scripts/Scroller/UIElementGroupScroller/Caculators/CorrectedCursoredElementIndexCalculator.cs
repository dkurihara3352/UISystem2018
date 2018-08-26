using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface ICorrectedCursoredElementIndexCalculator{
		int Calculate(int sourceIndex);
	}
	public class CorrectedCursoredElementIndexCalculator : ICorrectedCursoredElementIndexCalculator{
		public CorrectedCursoredElementIndexCalculator(IUIElementGroup uieGroup, int[] cursorSize){
			thisUIElementGroup = uieGroup;
			thisCursorSize = cursorSize;
		}
		readonly IUIElementGroup thisUIElementGroup;
		readonly int[] thisCursorSize;
		public int Calculate(int sourceIndex){
			
			IUIElement sourceElement = thisUIElementGroup.GetGroupElement(sourceIndex);

			int columnCount = thisUIElementGroup.GetArraySize(0);
			int rowCount = thisUIElementGroup.GetArraySize(1);
			int[] arraySize = new int[]{columnCount, rowCount};
			int[] arrayIndex = thisUIElementGroup.GetGroupElementArrayIndex(sourceElement);

			int[] targetArrayIndex = new int[2];
			for(int i = 0; i < 2; i ++){
				targetArrayIndex[i] = arrayIndex[i];
				if(arrayIndex[i] > arraySize[i] - thisCursorSize[i])
					targetArrayIndex[i] = arraySize[i] - thisCursorSize[i];
			}

			IUIElement targetInitElement = thisUIElementGroup.GetGroupElement(targetArrayIndex[0], targetArrayIndex[1]);
			int targetIndex = thisUIElementGroup.GetGroupElementIndex(targetInitElement);
			return targetIndex;
		}
	}
}
