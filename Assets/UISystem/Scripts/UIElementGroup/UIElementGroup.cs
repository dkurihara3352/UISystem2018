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
		int GetGroupElementIndex(IUIElement groupElement);
		IUIElement GetGroupElement(int columnIndex, int rowIndex);
		int[] GetGroupElementArrayIndex(IUIElement groupElement);
		IUIElement[] GetGroupElementsWithinIndexRange(int minColumnIndex, int minRowIndex, int maxColumnIndex, int maxRowIndex);
		IUIElement GetGroupElementAtPositionInGroupSpace(Vector2 positionInElementGroupSpace);
		void SetUpElements(List<IUIElement> elements);
	}
	public abstract class AbsUIElementGroup<T> : UIElement, IUIElementGroup where T: class, IUIElement{
		public AbsUIElementGroup(IUIElementGroupConstArg arg) :base(arg){
			thisRowCountConstraint = arg.rowCountConstraint;
			thisColumnCountConstraint = arg.columnCountConstraint;
			MakeSureConstraintIsProperlySet();
			CheckAndSetMaxElementsCount();
			thisTopToBottom = arg.topToBottom;
			thisLeftToRight = arg.leftToRight;
			thisRowToColumn = OverrideRowToColumnAccordingToConstraint(arg.rowToColumn);
			thisArrayIndexCalculator = new UIElementGroupArrayIndexCalculator(thisTopToBottom, thisLeftToRight, thisRowToColumn);
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
		protected List<T> thisGroupElements;/* explicitly and externally set */
		public IUIElement GetGroupElement(int index){
			return thisGroupElements[index];
		}
		public List<IUIElement> GetGroupElements(){
			List<IUIElement> result = new List<IUIElement>();
			foreach(T element in thisGroupElements){
				result.Add(element);
			}
			return result;
		}
		public int GetGroupElementIndex(IUIElement groupElement){
			if(groupElement != null){
				foreach(T uie in thisGroupElements){
					if( uie == groupElement)
						return thisGroupElements.IndexOf(uie);
				}
				throw new System.InvalidOperationException("groupElement is not found among thisGroupElements");
			}else
				throw new System.InvalidOperationException("groupElement should not be null");
		}
		public int GetSize(){return thisGroupElements.Count;}
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
			thisGroupElements = CreateTypedList(elements);
			ChildrenAllElements(elements);
			int numOfColumns = CalcNumberOfColumnsToCreate();
			int numOfRows = CalcNumberOfRowsToCreate();
			thisElementsArray = CreateElements2DArray(numOfColumns, numOfRows);
			this.ResizeToFitElements();
			thisGroupElementAtPositionInGroupSpaceCalculator = new GroupElementAtPositionInGroupSpaceCalculator(thisElementsArray, thisElementLength, thisPadding, thisUIA.GetRect().size);
			thisGroupElementsArrayCalculator = new GroupElementsArrayCalculator(thisElementsArray);
			PlaceElements();
			CheckAndSetUpScrollerElementOnParentScroller();
		}
		void CheckAndSetUpScrollerElementOnParentScroller(){
			IUIElement parentUIE = GetParentUIE();
			if(parentUIE != null)
				if(parentUIE is IScroller)
					((IScroller)parentUIE).SetUpScrollerElement();
		}
		List<T> CreateTypedList(List<IUIElement> source){
			List<T> result = new List<T>();
			foreach(IUIElement uie in source){
				result.Add((T)uie);
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
		protected T[ , ] CreateElements2DArray(int numOfColumns, int numOfRows){
			T[ , ] array = new T[ numOfColumns, numOfRows];
			foreach(T element in thisGroupElements){
				int elementIndex = thisGroupElements.IndexOf(element);
				int columnIndex = CalcColumnIndex(elementIndex, numOfColumns, numOfRows);
				int rowIndex = CalcRowIndex(elementIndex, numOfColumns, numOfRows);
				array[columnIndex, rowIndex] = element;
			}
			return array;
		}
		protected int CalcNumberOfColumnsToCreate(){
			if(thisColumnCountConstraint != 0)
				return thisColumnCountConstraint;
			else{
				int quotient = thisGroupElements.Count / thisRowCountConstraint;
				int modulo = thisGroupElements.Count % thisRowCountConstraint;
				return modulo > 0? quotient + 1 : quotient;
			}
		}
		protected int CalcNumberOfRowsToCreate(){
			if(thisRowCountConstraint != 0)
				return thisRowCountConstraint;
			else{
				int quotient = thisGroupElements.Count / thisColumnCountConstraint;
				int modulo = thisGroupElements.Count % thisColumnCountConstraint;
				return modulo > 0? quotient + 1 : quotient;
			}
		}
		UIElementGroupArrayIndexCalculator thisArrayIndexCalculator;
		protected int CalcColumnIndex(int n, int numOfColumns, int numOfRows){
			return thisArrayIndexCalculator.CalcColumnIndex(n, numOfColumns, numOfRows);
		}
		protected int CalcRowIndex(int n, int numOfColumns, int numOfRows){
			return thisArrayIndexCalculator.CalcRowIndex(n, numOfColumns, numOfRows);
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
			return thisGroupElementsArrayCalculator.GetGroupElementArrayIndex(element);
		}
		protected void PlaceElements(){
			foreach(T element in thisGroupElements){
				int[] arrayIndex = GetGroupElementArrayIndex(element);
				float localPosX = (arrayIndex[0] * (thisElementLength.x + thisPadding.x)) + thisPadding.x;
				float localPosY = (arrayIndex[1] * (thisElementLength.y + thisPadding.y)) + thisPadding.y;
				Vector2 newLocalPos = new Vector2(localPosX, localPosY);
				element.SetLocalPosition(newLocalPos);
			}
		}
		IGroupElementsArrayCalculator thisGroupElementsArrayCalculator;
		public IUIElement[] GetGroupElementsWithinIndexRange(int minColumnIndex, int minRowIndex, int maxColumnIndex, int maxRowIndex){
			return thisGroupElementsArrayCalculator.GetGroupElementsWithinIndexRange(minColumnIndex, minRowIndex, maxColumnIndex, maxRowIndex);
		}
		IGroupElementAtPositionInGroupSpaceCalculator thisGroupElementAtPositionInGroupSpaceCalculator;
		public IUIElement GetGroupElementAtPositionInGroupSpace(Vector2 positionInElementGroupSpace){
			return thisGroupElementAtPositionInGroupSpaceCalculator.Calculate(positionInElementGroupSpace);
		}
		/*  */
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
		public UIElementGroupConstArg(
			int columnCountConstraint, 
			int rowCountConstraint, 
			bool topToBottom, 
			bool leftToRight, 
			bool rowToColumn, 
			Vector2 elementLength, 
			Vector2 padding, 

			IUIManager uim, 
			IUISystemProcessFactory processFactory, 
			IUIElementFactory uieFactory, 
			IUIElementGroupAdaptor uia, 
			IUIImage image,
			ActivationMode activationMode
		): base(
			uim, 
			processFactory, 
			uieFactory, 
			uia, 
			image,
			activationMode
		){
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
	public interface IUIElementGroupAdaptor: IUIAdaptor{
		Vector2 GetGroupElementLength();
		Vector2 GetPadding();
	}
}
