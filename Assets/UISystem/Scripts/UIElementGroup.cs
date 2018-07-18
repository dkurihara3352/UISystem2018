using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DKUtility;

namespace UISystem{
	public interface IResizableUIElement: IUIElement{
		IInterpolator GetRectSizeInterpolator(Vector2 targetRectDim);
	}
	public interface IUIElementGroup: IUIElement{
		IInterpolator GetSlotSizeInterpolator(int rowNumber, int columnNumber, Vector2 targetRectDim);
		int GetSize();
		int GetElementsArraySize(int dimension);
	}
	public abstract class AbsUIElementGroup<T> : AbsUIElement, IUIElementGroup where T: class, IUIElement{
		List<T> thisElements;
		int rowsConstraint = 0;
		int columnsConstraint = 0;
		bool TopToBottom;
		bool LeftToRight;
		bool rowToColumn;
		Vector2 elementDimension;
		Vector2 padding;
		int maxElementCount = 0;
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
			if(rowsConstraint == 0 && columnsConstraint == 0)
				throw new System.InvalidOperationException("either rowCount or columnCount must be defined");
		}
		int CalcNumberOfRowsToCreate(){
			if(rowsConstraint != 0)
				return rowsConstraint;
			else{
				int quotient = thisElements.Count / columnsConstraint;
				int modulo = thisElements.Count % columnsConstraint;
				return modulo > 0? quotient + 1 : quotient;
			}
		}
		int CalcNumberOfColumnsToCreate(){
			if(columnsConstraint != 0)
				return columnsConstraint;
			else{
				int quotient = thisElements.Count / rowsConstraint;
				int modulo = thisElements.Count % rowsConstraint;
				return modulo > 0? quotient + 1 : quotient;
			}
		}
		int CalcColumnIndex(int n, int numOfColumns, int numOfRows){
			int valueA = n % numOfColumns;
			int valueB = n / numOfRows;
			if(LeftToRight)
				if(rowToColumn)
					return valueA;
				else
					return valueB;
			else
				if(rowToColumn)
					return numOfColumns - valueA - 1;
				else
					return numOfColumns - valueB - 1;
		}
		int CalcRowIndex(int n, int numOfColumns, int numOfRows){
			int valueA = n / numOfColumns;
			int valueB = n % numOfRows;
			if(LeftToRight)
				if(rowToColumn)
					return valueA;
				else
					return valueB;
			else
				if(rowToColumn)
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
			float targetWidth = columnCount * (elementDimension.x + padding.x) + padding.x;
			float targetHeight = rowCount * (elementDimension.y + padding.y) + padding.y;
			IUIAdaptor uia = GetUIAdaptor();
			Vector2 targetRectDim = new Vector2(targetWidth, targetHeight);
			uia.SetRectDimension(targetRectDim);
		}
		void GetElementArrayIndex(T element ,out int columnIndex, out int rowIndex){
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
				float localPosX = (columnIndex * (elementDimension.x + padding.x)) + padding.x;
				float localPosY = (rowIndex * (elementDimension.y + padding.y)) + padding.y;
				Vector2 newLocalPos = new Vector2(localPosX, localPosY);
				element.SetLocalPosition(newLocalPos);
			}
		}
	}
}
