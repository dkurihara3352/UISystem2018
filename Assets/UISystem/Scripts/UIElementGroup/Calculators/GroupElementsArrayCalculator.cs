using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IGroupElementsArrayCalculator{
		IUIElement[] GetGroupElementsWithinIndexRange(int minColumnIndex, int minRowIndex, int maxColumnIndex, int maxRowIndex);
		int[] GetGroupElementArrayIndex(IUIElement groupElement);
	}
	public class GroupElementsArrayCalculator: IGroupElementsArrayCalculator{
		public GroupElementsArrayCalculator(IUIElement[,] elementsArray){
			thisElementsArray = elementsArray;
		}
		readonly IUIElement[,] thisElementsArray;
		public IUIElement[] GetGroupElementsWithinIndexRange(int minColumnIndex, int minRowIndex, int maxColumnIndex, int maxRowIndex){
			List<IUIElement> result = new List<IUIElement>();
			for(int j = 0; j < thisElementsArray.GetLength(1); j ++){
				if(j >= minRowIndex && j <= maxRowIndex){
					for(int i = 0; i < thisElementsArray.GetLength(0); i ++){
						if(i >= minColumnIndex && i <= maxColumnIndex){
							IUIElement elementAtIndex = thisElementsArray[i, j];
							result.Add(elementAtIndex);
						}
					}
				}
			}
			return result.ToArray();
		}
		public int[] GetGroupElementArrayIndex(IUIElement groupElement){
			int[] result = new int[2]{-1, -1};
			for(int i = 0; i < thisElementsArray.GetLength(0); i ++){
				for(int j = 0; j < thisElementsArray.GetLength(1); j ++){
					IUIElement elementAtIndex = thisElementsArray[i, j];
					if(elementAtIndex != null)
						if(elementAtIndex == groupElement){
							result[0] = i;
							result[1] = j;
							return result;
						}
				}
			}
			return result;
		}
	}
}
