using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace UISystem{
	[RequireComponent(typeof(RectTransform))]
	public class UIAdaptor: UIBehaviour, IUIAdaptor, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler{
		/* take these sizes away to subclass */
		public bool resizeRelativeToScreenSize = false;
		public Vector2 sizeRelativeToScreenLength = Vector2.one;
		protected override void Awake(){
			AdjustRect();
			this.enabled = false;
		}
		void AdjustRect(){
			RectTransform rectTrans = transform.GetComponent<RectTransform>();
			AdjustPivot(rectTrans);
			AdjustAnchor(rectTrans);
			AdjustSize(rectTrans);
		}
		void AdjustPivot(RectTransform rectTransform){
			Vector2 targetPivot = Vector2.zero;
			Vector2 currentPivot = rectTransform.pivot;
			Vector2 pivotOffset = new Vector2(
				targetPivot.x - currentPivot.x,
				targetPivot.y - currentPivot.y
			);
			Vector2 localPosAdjutment = new Vector2(
				rectTransform.sizeDelta.x * pivotOffset.x,
				rectTransform.sizeDelta.y * pivotOffset.y
			);
			rectTransform.pivot = targetPivot;
			rectTransform.anchoredPosition = new Vector2(
				rectTransform.anchoredPosition.x + localPosAdjutment.x,
				rectTransform.anchoredPosition.y + localPosAdjutment.y
			);
		}
		void AdjustAnchor(RectTransform rectTrans){
			Vector2 targetAnchor = Vector2.zero;
			Vector2 currentAnchor = rectTrans.anchorMin;
			Vector2 anchorDiff = targetAnchor - currentAnchor;
			Vector2 parentRectLength = ((RectTransform)rectTrans.parent).sizeDelta;
			Vector2 anchorDisplacement = new Vector2(
				parentRectLength.x * anchorDiff.x,
				parentRectLength.y * anchorDiff.y
			);

			rectTrans.anchorMin = targetAnchor;
			rectTrans.anchorMax = targetAnchor;

			rectTrans.anchoredPosition = new Vector2(
				rectTrans.anchoredPosition.x - anchorDisplacement.x,
				rectTrans.anchoredPosition.y - anchorDisplacement.y
			);
		}	
		void AdjustSize(RectTransform rectTrans){
			if(resizeRelativeToScreenSize){
				Vector2 newSize = new Vector2();
				Vector2 screenRect = new Vector2(Screen.width, Screen.height);
				for(int i = 0; i < 2; i ++){
					float newLength = screenRect[i] * sizeRelativeToScreenLength[i];
					newSize[i] = newLength;
				}
				rectTrans.sizeDelta = newSize;
			}
		}
		/*  Activation and init */
		public void GetReadyForActivation(
			IUIAdaptorBaseInitializationData data,
			bool recursively
		){
			UpdateUIAdaptorHiearchy(recursively);
			InitializeUIAdaptor(data, recursively);
			CreateAndSetUIElement(recursively);
			SetUpUIElementReference(recursively);
			CompleteUIElementReferenceSetUp(recursively);

			thisUIElement.EvaluateScrollerFocusRecursively();
		}
		/* Setting up UIA Hierarchy */
			public void UpdateUIAdaptorHiearchy(bool recursively){
				UpdateUIAdaptorHiearchyImple();
				if(recursively)
					foreach(IUIAdaptor childUIAdaptor in thisChildUIAdaptors){
						childUIAdaptor.UpdateUIAdaptorHiearchy(true);
					}
				else{
					IUIAdaptor parentUIA = thisParentUIAdaptor;
					if(parentUIA != null)
						parentUIA.UpdateUIAdaptorHiearchy(false);
				}
			}
			void UpdateUIAdaptorHiearchyImple(){
				thisParentUIAdaptor = CalcParentUIAdaptor();
				thisChildUIAdaptors = CalcChildUIAdaptors();
			}
			protected IUIAdaptor thisParentUIAdaptor;
			IUIAdaptor CalcParentUIAdaptor(){
				Transform parent = this.transform.parent;
				if(parent != null){
					IUIAdaptor parentUIAdaptor = parent.GetComponent(typeof(IUIAdaptor)) as IUIAdaptor;
					return parentUIAdaptor;
				}
				return null;
			}
			List<IUIAdaptor> thisChildUIAdaptors;
			List<IUIAdaptor> CalcChildUIAdaptors(){
				List<IUIAdaptor> result = new List<IUIAdaptor>();
				for(int i = 0; i < transform.childCount; i ++){
					Transform child = transform.GetChild(i);
					IUIAdaptor childUIA = child.GetComponent(typeof(IUIAdaptor)) as IUIAdaptor;
					if(childUIA != null)
						result.Add(childUIA);
				}
				return result;
			}
		/* Initializing UIA */
			public void InitializeUIAdaptor(
				IUIAdaptorBaseInitializationData initData,
				bool recursively
			){
				thisDomainInitializationData = CheckAndCreateDomainInitializationData(initData);
				Initialize();
				if(recursively)
					foreach(IUIAdaptor childUIA in thisChildUIAdaptors){
						childUIA.InitializeUIAdaptor(
							thisDomainInitializationData,
							true
						);
					}
			}
			IUIAdaptorBaseInitializationData CheckAndCreateDomainInitializationData(
				IUIAdaptorBaseInitializationData passedData
			){
				IUIAdaptorBaseInitializationData result = null;
				if(this is IUIDomainManager){
					result = ((IUIDomainManager)this).CreateDomainActivationData(passedData);
				}else{
					result = passedData;
				}
				return result;
			}
			public IUIAdaptorBaseInitializationData GetDomainInitializationData(){
				return thisDomainInitializationData;
			}
			protected IUIAdaptorBaseInitializationData thisDomainInitializationData;
			protected IUIManager thisUIManager{get{return thisDomainInitializationData.uim;}}
			protected IUISystemProcessFactory thisProcessFactory{get{return thisDomainInitializationData.processFactory;}}
			void Initialize(){
				this.enabled = true;
				thisImageDarkenedBrightness = thisUIManager.GetUIImageDarknedBrightness();
				thisImageDefaultBrightness = thisUIManager.GetUIImageDefaultBrightness();
			}
		/* CreateAndSetUIElement */
			public void CreateAndSetUIElement(
				bool recursively
			){
				CreateAndSetUIElement();
				if(recursively)
					foreach(IUIAdaptor childUIA in thisChildUIAdaptors)
						childUIA.CreateAndSetUIElement(true);
			}
			void CreateAndSetUIElement(){
				IUIImage image = CreateUIImage();
				thisUIElement = CreateUIElement(image);
			}
			protected virtual IUIElement CreateUIElement(IUIImage image){
				IUIElementConstArg arg = new UIElementConstArg(
					thisDomainInitializationData.uim,
					thisDomainInitializationData.processFactory,
					thisDomainInitializationData.uiElementFactory,
					this,
					image,
					activationMode
				);
				return new UIElement(arg);
			}
			IUIElement thisUIElement;
			public IUIElement GetUIElement(){
				return thisUIElement;
			}
			public IUIElement GetParentUIE(){
				if(thisParentUIAdaptor != null)
					return thisParentUIAdaptor.GetUIElement();
				return null;
			}
			public void SetParentUIE(IUIElement newParentUIE, bool worldPositionStays){
				IUIAdaptor newParentUIA = newParentUIE.GetUIAdaptor();
				Transform parentTransform = newParentUIA.GetTransform();
				this.transform.SetParent(parentTransform, worldPositionStays);

				newParentUIA.UpdateUIAdaptorHiearchy(true);
			}
			public List<IUIElement> GetChildUIEs(){
				List<IUIElement> result = new List<IUIElement>();
				foreach(IUIAdaptor childUIA in thisChildUIAdaptors){
					result.Add(childUIA.GetUIElement());
				}
				return result;
			}
			public List<IUIElement> GetAllOffspringUIEs(){
				List<IUIElement> result = new List<IUIElement>();
				foreach(IUIAdaptor childUIA in thisChildUIAdaptors){
					result.AddRange(childUIA.GetAllOffspringUIEs());
				}
				return result;
			}
			protected virtual IUIImage CreateUIImage(){
				Image image;
				Transform childWithImage = GetChildWithImage(out image);
				RectTransform imageRectTrans = childWithImage.GetComponent<RectTransform>();
				if(imageRectTrans == null)
					throw new System.InvalidOperationException("image transform must have RectTransform component");
				imageRectTrans.pivot = new Vector2(0f, 0f);
				imageRectTrans.anchorMin = new Vector2(0f, 0f);
				imageRectTrans.anchorMax = new Vector2(1f, 1f);
				imageRectTrans.sizeDelta = Vector2.zero;
				imageRectTrans.anchoredPosition = Vector3.zero;
				IUIImage uiImage = new UIImage(image, childWithImage, thisImageDefaultBrightness, thisImageDarkenedBrightness, thisDomainInitializationData.processFactory);
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
			public float thisImageDefaultBrightness = .8f;
			public float thisImageDarkenedBrightness = .5f;
		/* Setting up UIE reference */
			public void SetUpUIElementReference(bool recursively){
				SetUpUIElementReferenceImple();
				if(recursively)
					foreach(IUIAdaptor childUIA in thisChildUIAdaptors)
						childUIA.SetUpUIElementReference(true);
			}
			protected virtual void SetUpUIElementReferenceImple(){
				IUIAdaptorInputStateEngineConstArg arg = new UIAdaptorInputStateEngineConstArg(
					thisUIManager,
					thisUIElement,
					this, 
					thisProcessFactory
				);
				thisInputStateEngine = new UIAdaptorInputStateEngine(arg);
			}
		/* Completing UIE reference */
			public void CompleteUIElementReferenceSetUp(bool recursively){
				CompleteUIElementReferenceSetUpImple();
				if(recursively)
					foreach(IUIAdaptor childUIAdaptor in thisChildUIAdaptors)
						childUIAdaptor.CompleteUIElementReferenceSetUp(true);
			}
			protected virtual void CompleteUIElementReferenceSetUpImple(){
			}
		/* Activation */
			public ActivationMode activationMode;
			public void SetUpCanvasGroupComponent(){
				if(thisCanvasGroup == null){
					CanvasGroup canvasGroup = this.gameObject.AddComponent<CanvasGroup>();
					canvasGroup.alpha = 0f;
					thisCanvasGroup = canvasGroup;
				}
			}
			CanvasGroup thisCanvasGroup = null;
			public float GetGroupAlpha(){
				return thisCanvasGroup.alpha;
			}
			public void SetGroupAlpha(float alpha){
				thisCanvasGroup.alpha = alpha;
			}
		/* MB adaptor */
			public Transform GetTransform(){
				return this.transform;
			}
			public Rect GetRect(){
				return ((RectTransform)this.GetComponent<RectTransform>()).rect;
			}
			public void SetRectLength(Vector2 length){
				RectTransform rt = (RectTransform)this.transform;
				rt.sizeDelta = length;
				thisUIElement.UpdateRect();
			}
			public Vector2 GetLocalPosition(){
				return new Vector2(this.transform.localPosition.x, this.transform.localPosition.y);
			}
			public void SetLocalPosition(Vector2 pos){
				this.transform.localPosition = new Vector3(pos.x, pos.y, 0f);
			}
			public void SetParentUIA(IUIAdaptor parentUIA, bool worldPositionStays){
				this.transform.SetParent(parentUIA.GetTransform(), worldPositionStays);
			}
			public string GetName(){
				return this.transform.gameObject.name;
			}
		/* Event System Imple */
			IUIAdaptorInputStateEngine thisInputStateEngine;
			bool PointerIDMatchesTheRegistered(int pointerId){
				return thisUIManager.registeredID == pointerId;
			}
			public virtual void OnPointerEnter(PointerEventData eventData){
				if(thisUIManager.TouchIDIsRegistered()){
					if(PointerIDMatchesTheRegistered(eventData.pointerId)){
						ICustomEventData customEventData = new CustomEventData(eventData, Time.deltaTime);
						thisInputStateEngine.OnPointerEnter(customEventData);
					}
				}
			}
			public void OnPointerExit(PointerEventData eventData){
				if(thisUIManager.TouchIDIsRegistered()){
					if(PointerIDMatchesTheRegistered(eventData.pointerId)){
						ICustomEventData customEventData = new CustomEventData(eventData, Time.deltaTime);
						thisInputStateEngine.OnPointerExit(customEventData);
					}
				}
			}
			public void OnPointerDown(PointerEventData eventData){
				if(!thisUIManager.TouchIDIsRegistered()){
					thisUIManager.RegisterTouchID(eventData.pointerId);
					ICustomEventData customEventData = new CustomEventData(eventData, Time.deltaTime);
					thisInputStateEngine.OnPointerDown(customEventData);
				}
			}
			public void OnPointerUp(PointerEventData eventData){
				if(thisUIManager.TouchIDIsRegistered()){
					if(PointerIDMatchesTheRegistered(eventData.pointerId)){
						ICustomEventData customEventData = new CustomEventData(eventData, Time.deltaTime);
						thisInputStateEngine.OnPointerUp(customEventData);
						thisUIManager.UnregisterTouchID();
					}
				}
			}
			public void OnBeginDrag(PointerEventData eventData){
				if(thisUIManager.TouchIDIsRegistered()){
					if(PointerIDMatchesTheRegistered(eventData.pointerId)){
						ICustomEventData customEventData = new CustomEventData(eventData, Time.deltaTime);
						thisInputStateEngine.OnBeginDrag(customEventData);
					}
				}
			}
			public void OnDrag(PointerEventData eventData){
				if(thisUIManager.TouchIDIsRegistered()){
					if(PointerIDMatchesTheRegistered(eventData.pointerId)){
						ICustomEventData customEventData = new CustomEventData(eventData, Time.deltaTime);
						thisInputStateEngine.OnDrag(customEventData);
					}
				}
			}
		/*  */
	}
}
