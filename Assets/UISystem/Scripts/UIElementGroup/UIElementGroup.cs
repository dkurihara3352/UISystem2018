using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DKUtility;

namespace UISystem{
	public interface IUIElementGroup: IUIElement{
		int GetSize();
		List<IUIElement> GetGroupElements();
		int GetGroupElementsArraySize(int dimension);
		int[] GetArraySize();
		IUIElement GetGroupElement(int index);
		IUIElement GetGroupElement(int columnIndex, int rowIndex);
		int[] GetGroupElementArrayIndex(IUIElement groupElement);
		IUIElement[] GetGroupElementsWithinIndexRange(int minColumnIndex, int minRowIndex, int maxColumnIndex, int maxRowIndex);
		IUIElement GetGroupElementAtPositionInGroupSpace(Vector2 positionInElementGroupSpace);
		void SetUpElements(List<IUIElement> elements);
	}
	public abstract class AbsUIElementGroup<T> : AbsUIElement, IUIElementGroup where T: class, IUIElement{
		public AbsUIElementGroup(IUIElementGroupConstArg arg) :base(arg){
			thisRowCountConstraint = arg.rowCountConstraint;
			thisColumnCountConstraint = arg.columnCountConstraint;
			MakeSureConstraintIsProperlySet();
			CheckAndSetMaxElementsCount();
			thisTopToBottom = arg.topToBottom;
			thisLeftToRight = arg.leftToRight;
			thisRowToColumn = OverrideRowToColumnAccordingToConstraint(arg.rowToColumn);
			thisElementLength = arg.elementLength;
			thisPadding = arg.padding;
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
		protected List<T> thisElements;/* explicitly and externally set */
		public IUIElement GetGroupElement(int index){
			return thisElements[index];
		}
		public List<IUIElement> GetGroupElements(){
			List<IUIElement> result = new List<IUIElement>();
			foreach(T element in thisElements){
				result.Add(element);
			}
			return result;
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
		void CheckAndSetMaxElementsCount(){
			if(thisColumnCountConstraint != 0 && thisRowCountConstraint != 0)
				thisMaxElementCount = thisColumnCountConstraint * thisRowCountConstraint;
		}
		public void SetUpElements(List<IUIElement> elements){
			CheckIfElementsCountIsValid(elements.Count);
			thisElements = CreateTypedList(elements);
			ChildrenAllElements(elements);
			thisElementsArray = CreateElements2DArray();
			this.ResizeToFitElements();
			PlaceElements();
		}
		List<T> CreateTypedList(List<IUIElement> source){
			List<T> result = new List<T>();
			foreach(IUIElement uie in source){
				result.Add(uie as T);
			}
			return result;
		}
		void ChildrenAllElements(List<IUIElement> elements){
			foreach(IUIElement uie in elements)
				uie.SetParentUIE(this, true);
		}
		void CheckIfElementsCountIsValid(int count){
			if(thisIsConstrainedByBothAxis)
				if(count > thisMaxElementCount)
					throw new System.InvalidOperationException("elements count exceeds maximum allowed count. try either decrease the elements count or release one of the array constraints");
		}
		protected T[,] thisElementsArray;
		public IUIElement GetGroupElement(int columnIndex, int rowIndex){
			return thisElementsArray[columnIndex, rowIndex];
		}
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
		public int GetGroupElementsArraySize(int dimension){
			return thisElementsArray.GetLength(dimension);
		}
		public int[] GetArraySize(){
			int[] result = new int[2];
			result[0] = thisElementsArray.GetLength(0);
			result[1] = thisElementsArray.GetLength(1);
			return result;
		}
		protected void ResizeToFitElements(){
			int columnCount = thisElementsArray.GetLength(0);
			int rowCount = thisElementsArray.GetLength(1);
			float targetWidth = columnCount * (thisElementLength.x + thisPadding.x) + thisPadding.x;
			float targetHeight = rowCount * (thisElementLength.y + thisPadding.y) + thisPadding.y;
			IUIAdaptor uia = GetUIAdaptor();
			uia.SetRectLength(targetWidth, targetHeight);
		}
		public int[] GetGroupElementArrayIndex(IUIElement element){
			int[] result = new int[2]{-1, -1};
			for(int i = 0; i < thisElementsArray.GetLength(0); i ++){
				for(int j = 0; j < thisElementsArray.GetLength(1); j ++){
					T elementAtIndex = thisElementsArray[i, j];
					if(elementAtIndex != null)
						if(elementAtIndex == element){
							result[0] = i;
							result[1] = j;
							return result;
						}
				}
			}
			return result;
		}
		protected void PlaceElements(){
			foreach(T element in thisElements){
				int[] index = GetGroupElementArrayIndex(element);
				float localPosX = (index[0] * (thisElementLength.x + thisPadding.x)) + thisPadding.x;
				float localPosY = (index[1] * (thisElementLength.y + thisPadding.y)) + thisPadding.y;
				Vector2 newLocalPos = new Vector2(localPosX, localPosY);
				element.SetLocalPosition(newLocalPos);
			}
		}
		public IUIElement[] GetGroupElementsWithinIndexRange(int minColumnIndex, int minRowIndex, int maxColumnIndex, int maxRowIndex){
			int[] arraySize = GetArraySize();
			List<IUIElement> result = new List<IUIElement>();
			for(int i = 0; i < arraySize[0]; i ++){
				if(i >= minColumnIndex && i <= maxColumnIndex){
					for(int j = 0; j < arraySize[1]; j ++){
						if(j >= minRowIndex && j <= maxRowIndex){
							IUIElement elementAtIndex = GetGroupElement(i, j);
							result.Add(elementAtIndex);
						}
					}
				}
			}
			return result.ToArray();
		}
		public IUIElement GetGroupElementAtPositionInGroupSpace(Vector2 positionInElementGroupSpace){
			//return null if the point is at padding space
			if(PositionIsOutOfThisRectBouds(positionInElementGroupSpace))
				return null;
			else{
				int[] arrayIndex = new int[2];
				for(int i = 0; i < 2; i ++){
					float elementLengthPlusPadding = thisElementLength[i] + thisPadding[i];
					float modulo = positionInElementGroupSpace[i] % elementLengthPlusPadding;
					if(modulo == 0f){
						if(positionInElementGroupSpace[i] > thisPadding[i]){
							int quotient = Mathf.FloorToInt(positionInElementGroupSpace[i]/ elementLengthPlusPadding) -1;
							arrayIndex[i] = quotient;
						}else
							return null;
					}else{
						if(modulo >= thisPadding[i]){
							int quotient = Mathf.FloorToInt(positionInElementGroupSpace[i] / elementLengthPlusPadding);
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
				if(position[i] < thisPadding[i] || position[i] > thisUIA.GetRect().size[i] - thisPadding[i])
					return true;
			}
			return false;
		}
		/*  */

		public override void OnScrollerFocus(){
			IUIElement parentUIE = GetParentUIE();
			if(parentUIE != null && parentUIE is IScroller)
				thisIsFocusedInScroller = true;
			else
				base.OnScrollerFocus();
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
			thisTopToBottom = topToBottom;
			thisLeftToRight = leftToRight;
			thisRowToColumn = rowToColumn;
			thisElementLength = elementLength;
			thisPadding = padding;
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
