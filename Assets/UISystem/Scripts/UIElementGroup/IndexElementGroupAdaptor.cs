using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IIndexElementGroupAdaptor: IUIElementGroupAdaptor{}
	public class IndexElementGroupAdaptor : UIAdaptor, IIndexElementGroupAdaptor {
		TestUIElementFactory testUIElementFactory;

		public int columnCountConstraint;
		public int rowCountConstraint;
		public bool topToBottom = true;
		public bool leftToRight = true;
		public bool rowToColumn = true;

		public int groupElementCount;
		public Font font;
		public int fontSize;
		public Color imageColor;
		public float imageDefaultDarkness = .8f;
		public float imageDarkenedDarkness = .5f;

		public Vector2 groupElementLength;
		public Vector2 GetGroupElementLength(){return groupElementLength;}
		public Vector2 padding;
		public Vector2 GetPadding(){return padding;}
		public bool[] usesFixedPadding = new bool[2]{true, true};
		
		protected override void SetUpUIElementReference(){
			testUIElementFactory = new TestUIElementFactory(
				thisUIManager, 
				font, 
				fontSize, 
				imageColor, 
				imageDefaultDarkness, 
				imageDarkenedDarkness
			);
			List<IUIElement> groupElements = CreateUIEs();
			IUIElementGroup uieGroup = (IUIElementGroup)this.GetUIElement();
			uieGroup.SetUpElements(groupElements);
		}
		List<IUIElement> CreateUIEs(){
			/* to SetUpUIEReference */
			List<IUIElement> result = new List<IUIElement>();
			for(int i = 0; i < groupElementCount; i ++){
				result.Add(
					testUIElementFactory.CreateUIElementWithIndexText(
						i, 
						groupElementLength, 
						thisDomainInitializationData.processFactory
					
					)
				);
			}
			return result;
		}
		protected override IUIElement CreateUIElement(IUIImage image){
			IUIElementGroupConstArg arg = new UIElementGroupConstArg(
				columnCountConstraint,
				rowCountConstraint,
				topToBottom,
				leftToRight,
				rowToColumn,
				groupElementLength,
				padding,
				usesFixedPadding,

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
