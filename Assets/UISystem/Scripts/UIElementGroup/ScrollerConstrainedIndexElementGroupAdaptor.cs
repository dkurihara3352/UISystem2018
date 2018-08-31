using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IScrollerConstrainedIndexElementGroupAdaptor: IUIElementGroupAdaptor{}
	public class ScrollerConstrainedIndexElementGroupAdaptor : AbsScrollerConstrainedGroupAdaptor, IScrollerConstrainedIndexElementGroupAdaptor {
		TestUIElementFactory testUIElementFactory;

		public int groupElementCount;
		public Font font;
		public int fontSize;
		public Color imageColor;
		public float imageDefaultDarkness = .8f;
		public float imageDarkenedDarkness = .5f;
		
		protected override List<IUIElement> GetGroupElements(){
			testUIElementFactory = new TestUIElementFactory(
				thisUIManager, 
				font, 
				fontSize, 
				imageColor, 
				imageDefaultDarkness, 
				imageDarkenedDarkness
			);
			return CreateUIEs();
		}
		List<IUIElement> CreateUIEs(){
			/* to SetUpUIEReference */
			List<IUIElement> result = new List<IUIElement>();
			for(int i = 0; i < groupElementCount; i ++){
				result.Add(
					testUIElementFactory.CreateUIElementWithIndexText(
						i, 
						GetIndexElementLength(), 
						thisDomainInitializationData.processFactory
					
					)
				);
			}
			return result;
		}
		Vector2 GetIndexElementLength(){
			return new Vector2(100f, 100f);
			// bool found = false;
			// Vector2 result = new Vector2();
			// if(ConstraintIsFixedType(firstConstraintType)){
			// 	if(firstConstraintType == RectConstraintType.FixedElementLength){
			// 		found = true;
			// 		result = firstConstraintValue;
			// 	}
			// }else if(ConstraintIsFixedType(secondConstraintType)){
			// 	if(secondConstraintType == RectConstraintType.FixedElementLength){
			// 		found = true;
			// 		result = secondConstraintValue;
			// 	}
			// }
			// if(found)
			// 	return result;
			// else
			// 	throw new System.InvalidOperationException(
			// 		"IndexElementGroup requires at lest one of its constraints be set FixedElementLength"
			// 	);

		}
		protected override IUIElement CreateUIElement(IUIImage image){
			IUIElementGroupConstArg arg = new UIElementGroupConstArg(
				columnCountConstraint,
				rowCountConstraint,
				topToBottom,
				leftToRight,
				rowToColumn,

				thisDomainInitializationData.uim,
				thisDomainInitializationData.processFactory,
				thisDomainInitializationData.uiElementFactory,
				this,
				image,
				activationMode
			);
			return new GenericUIElementGroup(arg);
		}
	}
}
