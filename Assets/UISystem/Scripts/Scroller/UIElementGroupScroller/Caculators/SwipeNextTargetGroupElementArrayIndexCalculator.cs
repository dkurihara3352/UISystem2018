using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface ISwipeNextTargetGroupElementArrayIndexCalculator{
		int[] Calculate(Vector2 swipeDeltaPos, int[] currentGroupElementAtCursorArrayIndex);
	}
	public class SwipeNextTargetGroupElementArrayIndexCalculator : ISwipeNextTargetGroupElementArrayIndexCalculator {
		public SwipeNextTargetGroupElementArrayIndexCalculator(IUIElementGroup uieGroup, int[] cursorSize, ScrollerAxis scrollerAxis){
			thisUIElementGroup = uieGroup;
			thisCursorSize = cursorSize;
			thisScrollerAxis = scrollerAxis;
		}
		readonly IUIElementGroup thisUIElementGroup;
		readonly int[] thisCursorSize;
		readonly ScrollerAxis thisScrollerAxis;
		public int[] Calculate(Vector2 velocity, int[] currentGroupElementAtCursorArrayIndex){
			int[] result = new int[2];

			int dominantAxis = -1;
			if(thisScrollerAxis == ScrollerAxis.Both){
				dominantAxis = GetDominantAxis(velocity);
			}

			for(int i = 0; i < 2; i ++){
				if(dominantAxis == -1 || dominantAxis == i){
					if(velocity[i] != 0f){
						if(velocity[i] < 0f)
							result[i] = currentGroupElementAtCursorArrayIndex[i] + 1;
						else
							result[i] = currentGroupElementAtCursorArrayIndex[i] - 1;
					}else
						result[i] = currentGroupElementAtCursorArrayIndex[i];
				}else{
					result[i] = currentGroupElementAtCursorArrayIndex[i];
				}
				result[i] = MakeTargetGroupElementArrayIndexWithinRange(result[i], i);
			}
			return result;
		}
		int GetDominantAxis(Vector2 delta){
			if(Mathf.Abs(delta.x) >= Mathf.Abs(delta.y))
				return 0;
			else
				return 1;
		}
		int MakeTargetGroupElementArrayIndexWithinRange(int source, int dimension){
			if(source < 0)
				return 0;
			else{
				int allowedMaxArrayIndex = thisUIElementGroup.GetArraySize(dimension) - thisCursorSize[dimension];
				if(source > allowedMaxArrayIndex)
					return allowedMaxArrayIndex;
			}
			return source;
		}
	}
}
