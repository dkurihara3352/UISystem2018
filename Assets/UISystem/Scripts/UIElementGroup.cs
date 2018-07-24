using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DKUtility;

namespace UISystem{
	public interface IUIElementGroup: IUIElement{
		int GetSize();
		int GetElementsArraySize(int dimension);
		IUIElement GetUIElement(int index);
	}
	public abstract class AbsUIElementGroup<T> : AbsUIElement, IUIElementGroup where T: class, IUIElement{
		public AbsUIElementGroup(IUIElementGroupConstArg arg) :base(arg){
			thisRowCountConstraint = arg.rowCountConstraint;
			thisColumnCountConstraint = arg.columnCountConstraint;
			thisTopToBottom = arg.topToBottom;
			thisLeftToRight = arg.leftToRight;
			thisRowToColumn = arg.rowToColumn;
			thisElementDimension = arg.elementDimension;
			thisPadding = arg.padding;
		}
		List<T> thisElements;/* explicitly and externally set */
		public IUIElement GetUIElement(int index){
			return thisElements[index];
		}
		public int GetSize(){return thisElements.Count;}
		readonly int thisRowCountConstraint = 0;
		readonly int thisColumnCountConstraint = 0;
		readonly bool thisTopToBottom;
		readonly bool thisLeftToRight;
		readonly bool thisRowToColumn;
		readonly Vector2 thisElementDimension;
		readonly Vector2 thisPadding;
		int thismaxElementCount = 0;/* used only when both axis are constrained */
		T[,] thisElementsArray;
		T[ , ] CreateElements2DArray(){
			MakeSureConstraintIsProperlySet();
			int numOfRowsToCreate = CalcNumberOfRowsToCreate();
			int numOfColumnsToCreate = CalcNumberOfColumnsToCreate();
			T[ , ] array = new T[ numOfColumnsToCreate, numOfRowsToCreate];
			foreach(T element in thisElements){
				int elementIndex = thisElements.IndexOf(element);
				int columnIndex = CalcColumnIndex(elementIndex, numOfColumnsToCreate, numOfRowsToCreate);
				int rowIndex = CalcRowIndex(elementIndex, numOfColumnsToCreate, numOfRowsToCreate);
				array[columnIndex, rowIndex] = element;
			}
			return array;
		}
		void MakeSureConstraintIsProperlySet(){
			if(thisRowCountConstraint == 0 && thisColumnCountConstraint == 0)
				throw new System.InvalidOperationException("either rowCount or columnCount must be defined");
		}
		int CalcNumberOfRowsToCreate(){
			if(thisRowCountConstraint != 0)
				return thisRowCountConstraint;
			else{
				int quotient = thisElements.Count / thisColumnCountConstraint;
				int modulo = thisElements.Count % thisColumnCountConstraint;
				return modulo > 0? quotient + 1 : quotient;
			}
		}
		int CalcNumberOfColumnsToCreate(){
			if(thisColumnCountConstraint != 0)
				return thisColumnCountConstraint;
			else{
				int quotient = thisElements.Count / thisRowCountConstraint;
				int modulo = thisElements.Count % thisRowCountConstraint;
				return modulo > 0? quotient + 1 : quotient;
			}
		}
		int CalcColumnIndex(int n, int numOfColumns, int numOfRows){
			int valueA = n % numOfColumns;
			int valueB = n / numOfRows;
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
		int CalcRowIndex(int n, int numOfColumns, int numOfRows){
			int valueA = n / numOfColumns;
			int valueB = n % numOfRows;
			if(thisLeftToRight)
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
		public int GetElementsArraySize(int dimension){
			return thisElementsArray.GetLength(dimension);
		}
		void ResizeToFitElements(){
			int columnCount = thisElementsArray.GetLength(0);
			int rowCount = thisElementsArray.GetLength(1);
			float targetWidth = columnCount * (thisElementDimension.x + thisPadding.x) + thisPadding.x;
			float targetHeight = rowCount * (thisElementDimension.y + thisPadding.y) + thisPadding.y;
			IUIAdaptor uia = GetUIAdaptor();
			uia.SetRectDimension(targetWidth, targetHeight);
		}
		void GetElementArrayIndex(IUIElement element ,out int columnIndex, out int rowIndex){
			for(int i = 0; i < thisElementsArray.GetLength(0); i ++){
				for(int j = 0; j < thisElementsArray.GetLength(1); j ++){
					T elementAtIndex = thisElementsArray[i, j];
					if(elementAtIndex != null)
						if(elementAtIndex == element){
							columnIndex = i;
							rowIndex = j;
						}
				}
			}
			columnIndex = -1;
			rowIndex = -1;
		}
		void PlaceElements(){
			foreach(T element in thisElements){
				int columnIndex;
				int rowIndex;
				GetElementArrayIndex(element, out columnIndex, out rowIndex);
				float localPosX = (columnIndex * (thisElementDimension.x + thisPadding.x)) + thisPadding.x;
				float localPosY = (rowIndex * (thisElementDimension.y + thisPadding.y)) + thisPadding.y;
				Vector2 newLocalPos = new Vector2(localPosX, localPosY);
				element.SetLocalPosition(newLocalPos);
			}
		}
	}
	public interface IUIElementGroupConstArg: IUIElementConstArg{
		int rowCountConstraint{get;}
		int columnCountConstraint{get;}
		bool topToBottom{get;}
		bool leftToRight{get;}
		bool rowToColumn{get;}
		Vector2 elementDimension{get;}
		Vector2 padding{get;}
	}
}
