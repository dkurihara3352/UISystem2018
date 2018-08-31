using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UISystem{
	public class IndexElementAdaptor : UIAdaptor, IInstatiableUIAdaptor {
		public void SetInitializationFields(IUIAInitializationData data){
			IIndexElementAdaptorInitializationData typedData = data as IIndexElementAdaptorInitializationData;
			thisIndexForText = typedData.index;
			thisFont = typedData.font;
			thisFontSize = typedData.fontSize;
			thisImageColor = typedData.imageColor;
			thisImageDefaultBrightness = typedData.imageDefaultDarkness;
			thisImageDarkenedBrightness = typedData.imageDarkenedDarkness;
		}
		protected override IUIElement CreateUIElement(IUIImage image){
			IUIElementConstArg arg = new UIElementConstArg(
				thisDomainInitializationData.uim, 
				thisDomainInitializationData.processFactory, 
				thisDomainInitializationData.uiElementFactory, 
				this, 
				image, 
				ActivationMode.None
			);
			IUIElement uie = new UIElement(arg);
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
			IUIImage uiImage = new UIImage(image, imageRT, thisImageDefaultBrightness, thisImageDarkenedBrightness, thisDomainInitializationData.processFactory);
			return uiImage;
		}
		RectTransform CreateChildWithImageComponent(out Image image){
			GameObject imageGO = new GameObject("imageGO");
			RectTransform imageRT = imageGO.AddComponent<RectTransform>();
			imageRT.SetParent(this.transform);
			imageRT.SetAsLastSibling();
			imageRT.pivot = new Vector2(0f, 0f);
			imageRT.anchorMin = Vector2.zero;
			imageRT.anchorMax = Vector2.one;
			imageRT.anchoredPosition = new Vector2(0f, 0f);
			imageRT.sizeDelta = Vector2.zero;
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
			textRT.anchorMax = Vector2.one;
			textRT.anchoredPosition = new Vector2(0f, 0f);
			textRT.sizeDelta = Vector2.zero;
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
	public interface IIndexElementAdaptorInitializationData: IUIAInitializationData{
		int index{get;}
		Font font{get;}
		int fontSize{get;}
		Color imageColor{get;}
		float imageDefaultDarkness{get;}
		float imageDarkenedDarkness{get;}
	}
	public struct IndexElementAdaptorInitializationData: IIndexElementAdaptorInitializationData{
		public IndexElementAdaptorInitializationData(
			int index, 
			Font font, 
			int fontSize, 
			Color imageColor, 
			float imageDefaultDarkness, 
			float imageDarkenedDarkness
		){
			thisIndex = index;
			thisFont = font;
			thisImageColor = imageColor;
			thisFontSize = fontSize;
			thisImageDefaultDarkness = imageDefaultDarkness;
			thisImageDarkenedDarkness = imageDarkenedDarkness;
		}
		readonly int thisIndex;
		public int index{get{return thisIndex;}}
		readonly Font thisFont;
		public Font font{get{return thisFont;}}
		readonly int thisFontSize;
		public int fontSize{get{return thisFontSize;}}
		readonly Color thisImageColor;
		public Color imageColor{get{return thisImageColor;}}
		readonly float thisImageDefaultDarkness;
		public float imageDefaultDarkness{get{return thisImageDefaultDarkness;}}
		readonly float thisImageDarkenedDarkness;
		public float imageDarkenedDarkness{get{return thisImageDarkenedDarkness;}}
	}
	public interface IIndexElementAdaptorInstantiationData: IInstantiableUIAdaptorInstantiationData{
	}
	public struct IndexElementAdaptorInstantiationData: IIndexElementAdaptorInstantiationData{
		public IndexElementAdaptorInstantiationData(
			IIndexElementAdaptorInitializationData initData
		){
			thisInitData = initData;
		}
		readonly IIndexElementAdaptorInitializationData thisInitData;
		public IUIAInitializationData initializationData{get{return thisInitData;}}
	}
}
