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
			thisElementLength = arg.elementLength;
			thisPadding = arg.padding;
		}
		protected List<T> thisElements;/* explicitly and externally set */
		public IUIElement GetUIElement(int index){
			return thisElements[index];
		}
		public int GetSize(){return thisElements.Count;}
		readonly int thisColumnCountConstraint = 0;
		readonly int thisRowCountConstraint = 0;
		bool thisIsConstrainedByColumnCount{get{return thisRowCountConstraint == 0 && thisColumnCountConstraint != 0;}}
		bool thisIsConstrainedByRowCount{get{return thisColumnCountConstraint == 0 && thisRowCountConstraint != 0;}}
		bool thisIsConstrainedByBothAxis{get{return thisColumnCountConstraint != 0 && thisRowCountConstraint != 0;}}
		readonly bool thisTopToBottom;
		readonly bool thisLeftToRight;
		readonly bool thisRowToColumn;
		readonly Vector2 thisElementLength;
		readonly Vector2 thisPadding;
		int thisMaxElementCount = 0;/* used only when both axis are constrained */
		protected void SetUpElements(List<T> elements){
			CheckIfElementsCountIsValid(elements.Count);
			thisElements = elements;
			thisElementsArray = CreateElements2DArray();
			this.ResizeToFitElements();
			PlaceElements();
		}
		void CheckIfElementsCountIsValid(int count){
			if(thisIsConstrainedByBothAxis)
				if(count > thisMaxElementCount)
					throw new System.InvalidOperationException("elements count exceeds maximum allowed count. try either decrease the elements count or release one of the array constraints");
		}
		protected T[,] thisElementsArray;
		protected T[ , ] CreateElements2DArray(){
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
		protected int CalcNumberOfColumnsToCreate(){
			if(thisColumnCountConstraint != 0)
				return thisColumnCountConstraint;
			else{
				int quotient = thisElements.Count / thisRowCountConstraint;
				int modulo = thisElements.Count % thisRowCountConstraint;
				return modulo > 0? quotient + 1 : quotient;
			}
		}
		protected int CalcNumberOfRowsToCreate(){
			if(thisRowCountConstraint != 0)
				return thisRowCountConstraint;
			else{
				int quotient = thisElements.Count / thisColumnCountConstraint;
				int modulo = thisElements.Count % thisColumnCountConstraint;
				return modulo > 0? quotient + 1 : quotient;
			}
		}
		protected int CalcColumnIndex(int n, int numOfColumns, int numOfRows){
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
		protected int CalcRowIndex(int n, int numOfColumns, int numOfRows){
			int valueA = n / numOfColumns;
			int valueB = n % numOfRows;
			if(thisTopToBottom)
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
		protected void ResizeToFitElements(){
			int columnCount = thisElementsArray.GetLength(0);
			int rowCount = thisElementsArray.GetLength(1);
			float targetWidth = columnCount * (thisElementLength.x + thisPadding.x) + thisPadding.x;
			float targetHeight = rowCount * (thisElementLength.y + thisPadding.y) + thisPadding.y;
			IUIAdaptor uia = GetUIAdaptor();
			uia.SetRectLength(targetWidth, targetHeight);
		}
		protected void GetElementArrayIndex(IUIElement element ,out int columnIndex, out int rowIndex){
			for(int i = 0; i < thisElementsArray.GetLength(0); i ++){
				for(int j = 0; j < thisElementsArray.GetLength(1); j ++){
					T elementAtIndex = thisElementsArray[i, j];
					if(elementAtIndex != null)
						if(elementAtIndex == element){
							columnIndex = i;
							rowIndex = j;
							return;
						}
				}
			}
			columnIndex = -1;
			rowIndex = -1;
		}
		protected void PlaceElements(){
			foreach(T element in thisElements){
				int columnIndex;
				int rowIndex;
				GetElementArrayIndex(element, out columnIndex, out rowIndex);
				float localPosX = (columnIndex * (thisElementLength.x + thisPadding.x)) + thisPadding.x;
				float localPosY = (rowIndex * (thisElementLength.y + thisPadding.y)) + thisPadding.y;
				Vector2 newLocalPos = new Vector2(localPosX, localPosY);
				element.SetLocalPosition(newLocalPos);
			}
		}
	}



	public interface IUIElementGroupConstArg: IUIElementConstArg{
		int columnCountConstraint{get;}
		int rowCountConstraint{get;}
		bool topToBottom{get;}
		bool leftToRight{get;}
		bool rowToColumn{get;}
		Vector2 elementLength{get;}
		Vector2 padding{get;}
	}
	public class UIElementGroupConstArg: UIElementConstArg ,IUIElementGroupConstArg{
		public UIElementGroupConstArg(int columnCountConstraint, int rowCountConstraint, bool topToBottom, bool leftToRight, bool rowToColumn, Vector2 elementLength, Vector2 padding, IUIManager uim, IUISystemProcessFactory processFactory, IUIElementFactory uieFactory, IUIElementGroupAdaptor uia, IUIImage image): base(uim, processFactory, uieFactory, uia, image){
			thisColumnCountConstraint = columnCountConstraint;
			thisRowCountConstraint = rowCountConstraint;
			MakeSureConstraintIsProperlySet();
			thisTopToBottom = topToBottom;
			thisLeftToRight = leftToRight;
			thisRowToColumn = OverrideRowToColumnAccordingToConstraint(rowToColumn);
			thisElementLength = elementLength;
			thisPadding = padding;
		}
		void MakeSureConstraintIsProperlySet(){
			if(thisRowCountConstraint == 0 && thisColumnCountConstraint == 0)
				throw new System.InvalidOperationException("either rowCount or columnCount must be defined");
		}
		bool OverrideRowToColumnAccordingToConstraint(bool rowToColumn){
			bool result = rowToColumn;
			if(thisColumnCountConstraint == 0)
				result = false;
			else if(thisRowCountConstraint == 0)
				result = true;
			return result;
		}
		readonly int thisColumnCountConstraint;
		public int columnCountConstraint{get{return thisColumnCountConstraint;}}
		readonly int thisRowCountConstraint;
		public int rowCountConstraint{get{return thisRowCountConstraint;}}
		readonly bool thisTopToBottom;
		public bool topToBottom{get{return thisTopToBottom;}}
		readonly bool thisLeftToRight;
		public bool leftToRight{get{return thisLeftToRight;}}
		readonly bool thisRowToColumn;
		public bool rowToColumn{get{return thisRowToColumn;}}
		readonly Vector2 thisElementLength;
		public Vector2 elementLength{get{return thisElementLength;}}
		readonly Vector2 thisPadding;
		public Vector2 padding{get{return thisPadding;}}
	}
	public interface IUIElementGroupAdaptor: IUIAdaptor{}
}
