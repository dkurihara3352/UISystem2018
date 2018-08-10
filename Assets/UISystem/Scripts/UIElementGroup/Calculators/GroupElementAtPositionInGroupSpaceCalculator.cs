﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IGroupElementAtPositionInGroupSpaceCalculator{
		IUIElement Calculate(Vector2 positionInGroupSpace);
	}
	public class GroupElementAtPositionInGroupSpaceCalculator : IGroupElementAtPositionInGroupSpaceCalculator {
		public GroupElementAtPositionInGroupSpaceCalculator(IUIElement[,] elementsArray, Vector2 elementLength, Vector2 padding, Vector2 groupRectLength){
			thisElementsArray = elementsArray;
			thisElementLength = elementLength;
			thisPadding = padding;
			thisGroupRectLength = groupRectLength;
		}
		readonly IUIElement[,] thisElementsArray;
		readonly Vector2 thisElementLength;
		readonly Vector2 thisPadding;
		readonly Vector2 thisGroupRectLength;
		public IUIElement Calculate(Vector2 positionInGroupSpace){
			if(PositionIsOutOfThisRectBouds(positionInGroupSpace))
				return null;
			else{
				int[] arrayIndex = new int[2];
				for(int i = 0; i < 2; i ++){
					float elementLengthPlusPadding = thisElementLength[i] + thisPadding[i];
					float modulo = positionInGroupSpace[i] % elementLengthPlusPadding;
					if(modulo == 0f){
						if(positionInGroupSpace[i] > thisPadding[i]){
							int quotient = Mathf.FloorToInt(positionInGroupSpace[i]/ elementLengthPlusPadding) -1;
							arrayIndex[i] = quotient;
						}else
							return null;
					}else{
						if(modulo >= thisPadding[i]){
							int quotient = Mathf.FloorToInt(positionInGroupSpace[i] / elementLengthPlusPadding);
							arrayIndex[i] = quotient;
						}else
							return null;
					}
				}
				IUIElement elementAtIndex = thisElementsArray[arrayIndex[0], arrayIndex[1]];
				return elementAtIndex;
			}
		}
		bool PositionIsOutOfThisRectBouds(Vector2 position){
			for(int i = 0; i < 2; i ++){
				if(position[i] < thisPadding[i] || position[i] > thisGroupRectLength[i] - thisPadding[i])
					return true;
			}
			return false;
		}
	}
}