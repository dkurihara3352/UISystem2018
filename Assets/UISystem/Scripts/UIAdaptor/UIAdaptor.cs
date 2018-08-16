using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace UISystem{
	[RequireComponent(typeof(RectTransform))]
	public class UIAdaptor: UIBehaviour, IUIAdaptor, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler{
		public bool resizeRelativeToScreenSize = false;
		public Vector2 sizeRelativeToReferenceRect = Vector2.one;
		protected override void Awake(){
			RectTransform rectTrans = transform.GetComponent<RectTransform>();
			rectTrans.pivot = new Vector2(0f, 0f);
			rectTrans.anchorMin = new Vector2(0f, 0f);
			rectTrans.anchorMax = new Vector2(0f, 0f);
			if(resizeRelativeToScreenSize){
				Vector2 newSize = new Vector2();
				Vector2 screenRect = new Vector2(Screen.width, Screen.height);
				for(int i = 0; i < 2; i ++){
					float newLength = screenRect[i] * sizeRelativeToReferenceRect[i];
					newSize[i] = newLength;
				}
				rectTrans.sizeDelta = newSize;
			}
			this.enabled = false;
		}
		/*  Activation and init */
			public virtual void GetReadyForActivation(IUIAActivationData passedData){
				thisDomainActivationData = CheckAndCreateDomainActivationData(passedData);
				IUIImage uiImage = CreateUIImage();
				thisUIElement = CreateUIElement(uiImage);
				thisInputStateEngine = new UIAdaptorInputStateEngine(passedData.uim, this, thisDomainActivationData.processFactory);
				this.enabled = true;
				GetAllChildUIAsReadyForActivation(this.GetAllChildUIAs(), thisDomainActivationData);
			}
			protected IUIAActivationData thisDomainActivationData;
			protected IUIManager thisUIM{get{return thisDomainActivationData.uim;}}
			public IUIAActivationData GetDomainActivationData(){
				return thisDomainActivationData;
			}
			IUIAActivationData CheckAndCreateDomainActivationData(IUIAActivationData passedData){
				IUIAActivationData result = null;
				if(this is IUIDomainManager){
					result = ((IUIDomainManager)this).CreateDomainActivationData(passedData);
				}else{
					result = passedData;
				}
				return result;
			}
			void GetAllChildUIAsReadyForActivation(List<IUIAdaptor> childUIAs, IUIAActivationData passedData){
				foreach(IUIAdaptor childUIA in childUIAs)
					childUIA.GetReadyForActivation(passedData);
			}
			protected virtual IUIImage CreateUIImage(){
				Image image;
				Transform childWithImage = GetChildWithImage(out image);
				RectTransform imageRectTrans = childWithImage.GetComponent<RectTransform>();
				if(imageRectTrans == null)
					throw new System.InvalidOperationException("image transform must have RectTransform component");
				imageRectTrans.pivot = new Vector2(0f, 0f);
				imageRectTrans.anchorMin = new Vector2(0f, 0f);
				imageRectTrans.anchorMax = new Vector2(0f, 0f);
				imageRectTrans.sizeDelta = this.GetRect().size;
				imageRectTrans.anchoredPosition = Vector3.zero;
				IUIImage uiImage = new UIImage(image, childWithImage, thisImageDefaultDarkness, thisImageDarkenedDarkness, thisDomainActivationData.processFactory);
				return uiImage;
			}
			protected Transform GetChildWithImage(out Image image){
				for(int i = 0; i < transform.childCount; i ++){
					Transform child = transform.GetChild(i);
					Image thisImage = child.GetComponent<Image>();
					if(thisImage != null){
						image = thisImage;
						return child;
					}
				}
				throw new System.InvalidOperationException("there's no child transform with Image component asigned");
			}
			public float thisImageDefaultDarkness = .8f;
			public float thisImageDarkenedDarkness = .5f;
		/* MB adaptor */
			public Transform GetTransform(){
				return this.transform;
			}
			public Rect GetRect(){
				return ((RectTransform)this.GetComponent<RectTransform>()).rect;
			}
			public void SetRectLength(float width, float height){
				RectTransform rectTrans = (RectTransform)this.GetComponent<RectTransform>();
				Vector2 newSize = new Vector2(width, height);
				rectTrans.sizeDelta = newSize;
				RectTransform[] childGraphicRTArray = GetChildRectTransformsWithGraphicComponent();
				foreach(RectTransform graphicRT in childGraphicRTArray)
				if(graphicRT != null){
					graphicRT.pivot = Vector2.zero;
					graphicRT.anchorMin = Vector2.zero;
					graphicRT.anchorMax = Vector2.zero;
					graphicRT.anchoredPosition = Vector2.zero;
					graphicRT.sizeDelta = newSize;
				}
			}
			protected RectTransform[] GetChildRectTransformsWithGraphicComponent(){
				List<RectTransform> result = new List<RectTransform>();
				for(int i = 0; i < this.transform.childCount; i ++){
					Transform child = this.transform.GetChild(i);
					Graphic graphicComp = child.GetComponent<Graphic>();
					if(graphicComp != null)
						result.Add(child.GetComponent<RectTransform>());
				}
				return result.ToArray();
			}

			public Vector2 GetWorldPosition(){
				return new Vector2(this.transform.position.x, this.transform.position.y);
			}
			public void SetWorldPosition(Vector2 worldPos){
				this.transform.position = new Vector3(worldPos.x, worldPos.y, 0f);
			}
			public Vector2 GetLocalPosition(){
				return new Vector2(this.transform.localPosition.x, this.transform.localPosition.y);
			}
			public void SetLocalPosition(Vector2 pos){
				this.transform.localPosition = new Vector3(pos.x, pos.y, 0f);
			}
			public Vector2 GetPositionInThisSpace(Vector2 worldPos){
				Vector3 resultV3 = this.transform.InverseTransformPoint(new Vector3(worldPos.x, worldPos.y, 0f));
				return new Vector2(resultV3.x, resultV3.y);
			}
			public void SetParentUIA(IUIAdaptor parentUIA, bool worldPositionStays){
				this.transform.SetParent(parentUIA.GetTransform(), worldPositionStays);
			}
			public string GetName(){
				return this.transform.gameObject.name;
			}
		/*  Hierarchy stuff */
			protected virtual IUIElement CreateUIElement(IUIImage image){return null;}
			IUIElement thisUIElement;
			public IUIElement GetUIElement(){
				return thisUIElement;
			}
			public IUIElement GetParentUIE(){
				Transform parentTrans = transform.parent;
				IUIAdaptor parentUIA = parentTrans.GetComponent(typeof(IUIAdaptor)) as IUIAdaptor;
				if(parentUIA != null)
					return parentUIA.GetUIElement();
				else
					return null;//must be top
			}
			public void SetParentUIE(IUIElement newParentUIE, bool worldPositionStays){
				IUIAdaptor newParentUIA = newParentUIE.GetUIAdaptor();
				Transform parentTransform = newParentUIA.GetTransform();
				this.transform.SetParent(parentTransform, worldPositionStays);
			}
			public List<IUIElement> GetChildUIEs(){
				List<IUIElement> result = new List<IUIElement>();
				foreach(IUIAdaptor childUIA in this.GetAllChildUIAs()){
					result.Add(childUIA.GetUIElement());
				}
				return result;
			}
			List<IUIAdaptor> GetAllChildUIAs(){
				List<IUIAdaptor> result = new List<IUIAdaptor>();
				for(int i = 0; i < transform.childCount; i ++){
					Transform child = transform.GetChild(i);
					IUIAdaptor childUIA = child.GetComponent(typeof(IUIAdaptor)) as IUIAdaptor;
					if(childUIA != null)
						result.Add(childUIA);
				}
				return result;
			}
			public List<IUIElement> GetAllOffspringUIEs(){
				List<IUIElement> result = new List<IUIElement>();
				foreach(IUIAdaptor childUIA in this.GetAllChildUIAs()){
					result.AddRange(childUIA.GetAllOffspringUIEs());
				}
				return result;
			}
		/* Event System Imple */
			IUIAdaptorInputStateEngine thisInputStateEngine;
			bool PointerIDMatchesTheRegistered(int pointerId){
				return thisUIM.registeredID == pointerId;
			}
			public virtual void OnPointerEnter(PointerEventData eventData){
				if(thisUIM.TouchIDIsRegistered()){
					if(PointerIDMatchesTheRegistered(eventData.pointerId)){
						ICustomEventData customEventData = new CustomEventData(eventData, Time.deltaTime);
						thisInputStateEngine.OnPointerEnter(customEventData);
					}
				}
			}
			public void OnPointerExit(PointerEventData eventData){
				if(thisUIM.TouchIDIsRegistered()){
					if(PointerIDMatchesTheRegistered(eventData.pointerId)){
						ICustomEventData customEventData = new CustomEventData(eventData, Time.deltaTime);
						thisInputStateEngine.OnPointerExit(customEventData);
					}
				}
			}
			public void OnPointerDown(PointerEventData eventData){
				if(!thisUIM.TouchIDIsRegistered()){
					thisUIM.RegisterTouchID(eventData.pointerId);
					ICustomEventData customEventData = new CustomEventData(eventData, Time.deltaTime);
					thisInputStateEngine.OnPointerDown(customEventData);
				}
			}
			public void OnPointerUp(PointerEventData eventData){
				if(thisUIM.TouchIDIsRegistered()){
					if(PointerIDMatchesTheRegistered(eventData.pointerId)){
						ICustomEventData customEventData = new CustomEventData(eventData, Time.deltaTime);
						thisInputStateEngine.OnPointerUp(customEventData);
						thisUIM.UnregisterTouchID();
					}
				}
			}
			/* OnEndDrag is needed too? */
			public void OnBeginDrag(PointerEventData eventData){
				if(thisUIM.TouchIDIsRegistered()){
					if(PointerIDMatchesTheRegistered(eventData.pointerId)){
						ICustomEventData customEventData = new CustomEventData(eventData, Time.deltaTime);
						thisInputStateEngine.OnBeginDrag(customEventData);
					}
				}
			}
			public void OnDrag(PointerEventData eventData){
				if(thisUIM.TouchIDIsRegistered()){
					if(PointerIDMatchesTheRegistered(eventData.pointerId)){
						ICustomEventData customEventData = new CustomEventData(eventData, Time.deltaTime);
						thisInputStateEngine.OnDrag(customEventData);
					}
				}
			}
		/*  */
	}
}
