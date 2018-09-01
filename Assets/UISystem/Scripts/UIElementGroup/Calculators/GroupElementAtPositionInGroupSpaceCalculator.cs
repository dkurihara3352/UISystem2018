using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IGroupElementAtPositionInGroupSpaceCalculator{
		IUIElement Calculate(Vector2 positionInGroupSpace);
	}
	public class GroupElementAtPositionInGroupSpaceCalculator : IGroupElementAtPositionInGroupSpaceCalculator {
		public GroupElementAtPositionInGroupSpaceCalculator(
			IUIElement[,] elementsArray, 
			Vector2 elementLength, 
			Vector2 padding, 
			Vector2 groupRectLength,
			string name
		){
			thisElementsArray = elementsArray;
			thisElementLength = elementLength;
			thisPadding = padding;
			thisGroupRectLength = groupRectLength;
			thisName = name;
		}
		readonly IUIElement[,] thisElementsArray;
		readonly Vector2 thisElementLength;
		readonly Vector2 thisPadding;
		readonly Vector2 thisGroupRectLength;
		readonly string thisName;
		float marginOfError = .01f;
		int[] thisGridCounts{
			get{
				return new int[]{
					thisElementsArray.GetLength(0),
					thisElementsArray.GetLength(1)
				};
			}
		}
		public IUIElement Calculate(Vector2 positionInGroupSpace){
			if(PositionIsOutOfThisRectBouds(positionInGroupSpace)){
				return null;
			}
			else{
				int[] arrayIndex = new int[2];
				for(int i = 0; i < 2; i ++){
					float elementLengthPlusPadding = thisElementLength[i] + thisPadding[i];
					float modulo = positionInGroupSpace[i] % elementLengthPlusPadding;
					if(modulo == 0f){
						int quotient = Mathf.FloorToInt(positionInGroupSpace[i]/ elementLengthPlusPadding);
						if(quotient == 0){
							if(thisPadding[1] != 0f){//at the leastside edge
								return null;
							}
						}else{
							quotient -= 1;
						}
						arrayIndex[i] = quotient;
					}else{
						if(modulo >= thisPadding[i] - marginOfError){
							int quotient = Mathf.FloorToInt(positionInGroupSpace[i] / elementLengthPlusPadding);
							arrayIndex[i] = quotient;
						}else{
							return null;
						}
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
