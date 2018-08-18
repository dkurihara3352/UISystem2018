using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IQuantityRoller: IUIElement{
		void SetRollerValue(float targetValue);
		float GetRollerValue();

	}
	public class QuantityRoller: UIElement, IQuantityRoller{
		public QuantityRoller(IQuantityRollerConstArg arg): base(arg){
			thisAllDigitPanelSets = CreateDigitPanelSets(arg.maxQuantity, arg.panelDim, arg.padding);
			CalcAndSetRectDimension(arg.panelDim, arg.rollerNormalizedPos, arg.padding);
		}
		readonly List<IDigitPanelSet> thisAllDigitPanelSets;
		List<IDigitPanelSet> CreateDigitPanelSets(int maxQuantity, Vector2 panelDim, Vector2 padding){
			int digitsCount = GetDigitsCountForPositiveInt(maxQuantity);
			List<IDigitPanelSet> result = new List<IDigitPanelSet>();
			for(int i = 0; i < digitsCount; i++){
				IDigitPanelSet digitPanelSet = thisUIElementFactory.CreateDigitPanelSet(i, this, panelDim, padding);
				thisAllDigitPanelSets.Add(digitPanelSet);
			}
			return result;
		}
		void CalcAndSetRectDimension(Vector2 panelDim, Vector2 rollerNormPos, Vector2 padding){
			int digitsCount = thisAllDigitPanelSets.Count;
			IUIAdaptor parentUIA = this.GetParentUIE().GetUIAdaptor();
			float parentHeight = parentUIA.GetRect().height;
			float parentWidth = parentUIA.GetRect().width;

			float rollerHeight = panelDim.y + (padding.y * 2);
			float rollerWidth = (panelDim.x * digitsCount) + (padding.x * (digitsCount + 1));
			float localX = rollerNormPos.x * (parentWidth - rollerWidth);
			float localY = rollerNormPos.y * (parentHeight - rollerHeight);
			((IQuantityRollerAdaptor)thisUIA).SetRectDimension(rollerHeight, rollerWidth, localX, localY);
		}
		int GetDigitsCountForPositiveInt(int sourceNumber){
			if(sourceNumber >= 0)
				return Mathf.FloorToInt(Mathf.Log10(sourceNumber)) + 1;
			else
				throw new System.ArgumentOutOfRangeException("sourceNumber must be at least zero");
		}
		public void SetRollerValue(float targetValue){
			thisCurrentRollerValue = targetValue;
			int targetValueInt = Mathf.FloorToInt(targetValue);
			float normalizedTransitionValue = targetValue - targetValueInt;
			int[] digitNumbers = ConvertIntToDigitNumbers(targetValueInt);
			foreach(IDigitPanelSet dps in thisAllDigitPanelSets){
				int thisIndex = thisAllDigitPanelSets.IndexOf(dps);
				if(thisIndex == digitNumbers.Length){
					if(PrevDPS(thisIndex).GetDigitTargetValue() > 9f){
						dps.PerformNumberTransition(-1, 1, normalizedTransitionValue);
					}else{
						dps.Blank();
					}
				}else{
					if(thisIndex > digitNumbers.Length){
						dps.Blank();
					}else{
						if(thisIndex == 0){
							PerformDPSNumberTransition(dps, digitNumbers, thisIndex, normalizedTransitionValue);
						}else{
							if(PrevDPS(thisIndex).GetDigitTargetValue() > 9f){
								PerformDPSNumberTransition(dps, digitNumbers, thisIndex, normalizedTransitionValue);
							}else{
								dps.UpdateNumberOnPanel(digitNumbers[thisIndex]);
							}
						}
					}
				}
			}
		}
		float thisCurrentRollerValue;
		public float GetRollerValue(){return thisCurrentRollerValue;}
		int[] ConvertIntToDigitNumbers(int sourceNumber){
			List<int> result = new List<int>();
			while(sourceNumber > 0){
				int lastDigitNumber = sourceNumber % 10;
				sourceNumber -= lastDigitNumber;
				if(sourceNumber != 0)
					sourceNumber /= 10;
			}
			result.Reverse();
			return result.ToArray();
		}
		void PerformDPSNumberTransition(IDigitPanelSet dps, int[] digitNumbers, int thisIndex, float normalizedTransitionValue){
			int num = digitNumbers[thisIndex];
			int nextNum = num == 9? 0: num + 1; 
			dps.PerformNumberTransition(num, nextNum, normalizedTransitionValue);
		}
		IDigitPanelSet PrevDPS(int id){
			return thisAllDigitPanelSets[id -1];
		}
	}
	public interface IQuantityRollerConstArg: IUIElementConstArg{
		int maxQuantity{get;}
		IUIElementFactory uieFactory{get;}
		Vector2 panelDim{get;}
		Vector2 padding{get;}
		Vector2 rollerNormalizedPos{get;}
	}
	public class QuantityRollerConstArg: UIElementConstArg, IQuantityRollerConstArg{
		public QuantityRollerConstArg(
			IUIManager uim, 
			IUISystemProcessFactory processFactory, 
			IUIElementFactory uiElementFactory, 
			IQuantityRollerAdaptor quaRolAdaptor, 
			IUIImage image,

			int maxQuantity, 			
			Vector2 panelDim, 
			Vector2 padding, 
			Vector2 rollerNormalizedPos
		): base(
			uim, 
			processFactory, 
			uiElementFactory, 
			quaRolAdaptor, 
			image,
			ActivationMode.None
		){
			thisMaxQuantity = maxQuantity;
			thisUIEFactory = uieFactory;
			thisPanelDim = panelDim;
			thisPadding = padding;
			thisRollerNormalizedPos = rollerNormalizedPos;
		}
		readonly int thisMaxQuantity;
		public int maxQuantity{get{return thisMaxQuantity;}}
		readonly IUIElementFactory thisUIEFactory;
		public IUIElementFactory uieFactory{get{return thisUIEFactory;}}
		readonly Vector2 thisPanelDim;
		public Vector2 panelDim{get{return thisPanelDim;}}
		readonly Vector2 thisPadding;
		public Vector2 padding{get{return thisPadding;}}
		readonly Vector2 thisRollerNormalizedPos;
		public Vector2 rollerNormalizedPos{get{return thisRollerNormalizedPos;}}

	}
}
