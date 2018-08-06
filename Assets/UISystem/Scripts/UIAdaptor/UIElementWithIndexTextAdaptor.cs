using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UISystem{
	public class UIElementWithIndexTextAdaptor : UIAdaptor, IInstatiableUIAdaptor {
		public void SetInitializationFields(IUIAInitializationData data){
			IUIElementWithIndexTextAdaptorInitializationData typedData = data as IUIElementWithIndexTextAdaptorInitializationData;
			thisIndexForText = typedData.index;
			thisFont = typedData.font;
			thisFontSize = typedData.fontSize;
			thisImageColor = typedData.imageColor;
			thisImageDefaultDarkness = typedData.imageDefaultDarkness;
			thisImageDarkenedDarkness = typedData.imageDarkenedDarkness;
		}
		protected override IUIElement CreateUIElement(IUIImage image){
			IUIElementConstArg arg = new UIElementConstArg(thisDomainActivationData.uim, thisDomainActivationData.processFactory, thisDomainActivationData.uiElementFactory, this, image);
			GenericUIElement uie = new GenericUIElement(arg);
			return uie;
		}
		int thisIndexForText;
		Color thisImageColor;
		Font thisFont;
		int thisFontSize;
		protected override IUIImage CreateUIImage(){
			Image image;
			RectTransform imageRT = CreateChildWithImageComponent(out image);
			Text text;
			RectTransform textRT = CreateChildWithTextComponent(out text);
			IUIImage uiImage = new UIImage(image, imageRT, thisImageDefaultDarkness, thisImageDarkenedDarkness);
			return uiImage;
		}
		RectTransform CreateChildWithImageComponent(out Image image){
			GameObject imageGO = new GameObject("imageGO");
			RectTransform imageRT = imageGO.AddComponent<RectTransform>();
			imageRT.SetParent(this.transform);
			imageRT.SetAsLastSibling();
			imageRT.pivot = new Vector2(0f, 0f);
			imageRT.anchorMin = Vector2.zero;
			imageRT.anchorMax = Vector2.zero;
			imageRT.anchoredPosition = new Vector2(0f, 0f);
			imageRT.sizeDelta = this.GetRect().size;
			Image thisImage = CreateImage(imageGO);
			image = thisImage;
			return imageRT;
		}
		Image CreateImage(GameObject gameObject){
			Image image = gameObject.AddComponent<Image>();
			image.color = thisImageColor;
			return image;
		}
		RectTransform CreateChildWithTextComponent(out Text text){
			GameObject textGO = new GameObject("textGO");
			RectTransform textRT = textGO.AddComponent<RectTransform>();
			textRT.SetParent(this.transform);
			textRT.SetAsLastSibling();
			textRT.pivot = new Vector2(0f, 0f);
			textRT.anchorMin = Vector2.zero;
			textRT.anchorMax = Vector2.zero;
			textRT.anchoredPosition = new Vector2(0f, 0f);
			textRT.sizeDelta = this.GetRect().size;
			Text thisText = CreateIndexText(thisIndexForText, textGO);
			text = thisText;
			return textRT;
		}
		Text CreateIndexText(int index, GameObject gameObject){
			Text text = gameObject.AddComponent<Text>();
			text.text = index.ToString();
			text.alignment = TextAnchor.MiddleCenter;
			text.fontSize = thisFontSize;
			text.font = thisFont;
			text.color = Color.black;
			return text;
		}
	}
	public interface IUIElementWithIndexTextAdaptorInitializationData: IUIAInitializationData{
		int index{get;}
		Font font{get;}
		int fontSize{get;}
		Color imageColor{get;}
		float imageDefaultDarkness{get;}
		float imageDarkenedDarkness{get;}
	}
	public struct UIElementWithIndexTextAdaptorInitializationData: IUIElementWithIndexTextAdaptorInitializationData{
		public UIElementWithIndexTextAdaptorInitializationData(int index, Font font, int fontSize, Color imageColor){
			thisIndex = index;
			thisFont = font;
			thisImageColor = imageColor;
			thisFontSize = fontSize;
		}
		readonly int thisIndex;
		public int index{get{return thisIndex;}}
		readonly Font thisFont;
		public Font font{get{return thisFont;}}
		readonly int thisFontSize;
		public int fontSize{get{return thisFontSize;}}
		readonly Color thisImageColor;
		public Color imageColor{get{return thisImageColor;}}
		public float imageDefaultDarkness{get{return .8f;}}
		public float imageDarkenedDarkness{get{return .5f;}}
	}
	public interface IUIElementWithIndexTextAdaptorInstantiationData: IInstantiableUIAdaptorInstantiationData{
	}
	public struct UIElementWithIndexTextAdaptorInstantiationData: IUIElementWithIndexTextAdaptorInstantiationData{
		public UIElementWithIndexTextAdaptorInstantiationData(Vector2 sizeDelta, IUIElementWithIndexTextAdaptorInitializationData initData){
			thisSizeDelta = sizeDelta;
			thisInitData = initData;
		}
		readonly Vector2 thisSizeDelta;
		public Vector2 sizeDelta{get{return thisSizeDelta;}}
		readonly IUIElementWithIndexTextAdaptorInitializationData thisInitData;
		public IUIAInitializationData initializationData{get{return thisInitData;}}
	}
}
