using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IUIElementGroupArrayIndexCalculator{
		int CalcColumnIndex(int elementIndex, int numOfColumns, int numOfRows);
		int CalcRowIndex(int elementIndex, int numOfColumns, int rumOfRows);
	}
	public class UIElementGroupArrayIndexCalculator : IUIElementGroupArrayIndexCalculator {
		public UIElementGroupArrayIndexCalculator(bool topToBottom, bool leftToRight, bool rowToColumn){
			thisTopToBottom = topToBottom;
			thisLeftToRight = leftToRight;
			thisRowToColumn = rowToColumn;
		}
		readonly bool thisTopToBottom;
		readonly bool thisLeftToRight;
		readonly bool thisRowToColumn;
		public int CalcColumnIndex(int elementIndex, int numOfColumns, int numOfRows){
			int valueA = elementIndex % numOfColumns;
			int valueB = elementIndex / numOfRows;
			if(thisLeftToRight)
				if(thisRowToColumn)
					return valueA;
				else
					return valueB;
			else
				if(thisRowToColumn)
					return numOfColumns - valueA - 1;
				else
					return numOfColumns - valueB - 1;
		}
		public int CalcRowIndex(int n, int numOfColumns, int numOfRows){
			int valueA = n / numOfColumns;
			int valueB = n % numOfRows;
			if(!thisTopToBottom)
				if(thisRowToColumn)
					return valueA;
				else
					return valueB;
			else
				if(thisRowToColumn)
					return numOfRows - 1 - valueA;
				else
					return numOfRows - 1 - valueB;
		}
	}
}

