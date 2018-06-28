using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IQuantityRoller: IUIElement{
		void Roll(float targetValue);
	}
	public class QuantityRoller: AbsUIElement, IQuantityRoller{
		IUIElementFactory thisUIEFactory{
			get{return thisTool.GetUIElementFactory();}
		}
		public QuantityRoller(IQuantityRollerConstArg arg): base(arg){
			thisDigitPanelSets = CreateDigitPanelSets(arg.maxQuantity);
			CalcAndSetRollerRectDimension(arg.panelDim, arg.rollerNormalizedPos, arg.padding);
			CalcAndSetDigitPanelSetRectDimension(arg.panelDim, arg.padding);
		}
		readonly List<IDigitPanelSet> thisDigitPanelSets;
		List<IDigitPanelSet> CreateDigitPanelSets(int maxQuantity){
			int digitsCount = GetDigitsCountForPositiveInt(maxQuantity);
			List<IDigitPanelSet> result = new List<IDigitPanelSet>();
			for(int i = 0; i < digitsCount; i++){
				IDigitPanelSet digitPanelSet = thisUIEFactory.CreateDigitPanelSet(i, this);
				thisDigitPanelSets.Add(digitPanelSet);
			}
			return result;
		}
		void CalcAndSetRollerRectDimension(Vector2 panelDim, Vector2 rollerNormPos, Vector2 padding){
			int digitsCount = thisDigitPanelSets.Count;
			IUIAdaptor parentUIA = this.GetParentUIE().GetUIAdaptor();
			float parentHeight = parentUIA.GetRect().height;
			float parentWidth = parentUIA.GetRect().width;

			float rollerHeight = panelDim.y + (padding.y * 2);
			float rollerWidth = (panelDim.x * digitsCount) + (padding.x * (digitsCount + 1));
			float localX = ;
			float localY = ;
			((IQuantityRollerAdaptor)thisUIA).SetRectDimension(rollerHeight, rollerWidth, localX, localY);
		}
		void CalcAndSetDigitPanelSetRectDimension(Vector2 panelDim, Vector2 padding){
			int digitsCount = thisDigitPanelSets.Count;
			float rollerHeight = thisUIA.GetRect().height;
			float rollerWidth = thisUIA.GetRect().width;
			foreach(IDigitPanelSet dps in thisDigitPanelSets){
				float height = (panelDim.y * 2) + (padding.y * 3);
				float width = panelDim.x;
				int digitPlace = thisDigitPanelSets.IndexOf(dps);
				float localX = rollerWidth - ((width + padding.x) * (digitPlace +1));
				float localY = 0f;
				((IDigitPanelSetAdaptor)dps.GetUIAdaptor()).SetRectDimension(height, width, localX, localY);
			}
		}
		int GetDigitsCountForPositiveInt(int sourceNumber){
			if(sourceNumber >= 0)
				return Mathf.FloorToInt(Mathf.Log10(sourceNumber)) + 1;
			else
				throw new System.ArgumentOutOfRangeException("sourceNumber must be at least zero");
		}
		public void Roll(float targetValue){
			foreach(IDigitPanelSet digitPanelSet in thisDigitPanelSets){
				digitPanelSet.Roll(targetValue);
			}
		}
	}
	public interface IQuantityRollerConstArg: IUIElementConstArg{
		int maxQuantity{get;}
		IUIElementFactory uieFactory{get;}
	}
	public interface IDigitPanelSet: IUIElement{
		void Roll(float targetValue);
	}
	public class DigitPanelSet: AbsUIElement, IDigitPanelSet{
		public DigitPanelSet(IUIElementConstArg arg): base(arg){
			/*  Create and set digit panels here
			*/
		}
		readonly IDigitPanel upperPanel;
		readonly IDigitPanel lowerPanel;
		readonly int digitPlace;
		public void Roll(float targetValue){
			float thisDigitTargetValue = GetThisDigitTargetValue(targetValue);
			int floor;
			int ceiling;
			CalcFlankingIntegers(thisDigitTargetValue, out floor, out ceiling);
			upperPanel.SetNumber(floor);
			lowerPanel.SetNumber(ceiling);
			if(ceiling == 0)
				ceiling = 10;
			float normalizedVerticalPosition = thisDigitTargetValue - floor;
			SetPositionInRoller(normalizedVerticalPosition);
		}
		void SetPositionInRoller(float normalizedVerticalPos){

		}
	}
	public interface IDigiPanelSetConstArg: IUIElementConstArg{
		int digitPlace{get;}
	}
	public class DigitPanelSetConstArg: UIElementConstArg{
		public DigitPanelSetConstArg(IUIManager uim, IUIAdaptor uia, IUIImage image, IUITool tool, int digitPlace): base(uim, uia, image, tool){
			thisDigitPlace = digitPlace;
		}
		readonly int thisDigitPlace;
		public int digitPlace{get{return thisDigitPlace;}}
	}
	public interface IDigitPanelSetAdaptor: IUIAdaptor{
		void SetDigitPlace(int digitPlace);
		void SetRectDimension(float height, float width, float localPosX, float localPosY);
	}
	public class DigitPanelSetAdaptor: AbsUIAdaptor<DigitPanelSet>, IDigitPanelSetAdaptor{
		public void SetDigitPlace(int digitPlace){
			thisDigitPlace = digitPlace;
		}
		public int thisDigitPlace;
		protected override DigitPanelSet CreateUIElement(){
			DigitPanelSetConstArg arg = new DigitPanelSetConstArg(thisDomainActivationData.uim, this, null, thisDomainActivationData.tool, thisDigitPlace);
			DigitPanelSet digitPanelSet = new DigitPanelSet(arg);
			return digitPanelSet;
		}
	}
	public interface IDigitPanel: IUIElement{
		void SetNumber(int number);
	}
}
