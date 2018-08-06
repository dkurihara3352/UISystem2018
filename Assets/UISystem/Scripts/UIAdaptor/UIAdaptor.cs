using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace UISystem{
	[RequireComponent(typeof(RectTransform))]
	public class UIAdaptor: UIBehaviour, IUIAdaptor, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler{
		protected override void Awake(){
			RectTransform rectTrans = transform.GetComponent<RectTransform>();
			rectTrans.pivot = new Vector2(0f, 0f);
			rectTrans.anchorMin = new Vector2(0f, 0f);
			rectTrans.anchorMax = new Vector2(0f, 0f);


			Rect guiRect = CreateGUIRect(new Vector2(1f, 1f), new Vector2(.2f, .3f));
			rect1 = GetSubRect(guiRect, 0, 4);
			rect2 = GetSubRect(guiRect, 1, 4);
			rect3 = GetSubRect(guiRect, 2, 4);
			rect4 = GetSubRect(guiRect, 3, 4);
		}
		protected Rect CreateGUIRect(Vector2 normalizedPosition, Vector2 normalizedSize){
			MakeSureValuesAreInRange(normalizedPosition);
			MakeSureValuesAreInRange(normalizedSize);
			Vector2 rectLength = new Vector2(Screen.width * normalizedSize.x, Screen.height * normalizedSize.y);
			float rectX = (Screen.width - rectLength.x) * normalizedPosition.x;
			float rectY = (Screen.height - rectLength.y) * normalizedPosition.y;
			Vector2 rectPosition = new Vector2(rectX, rectY);
			return new Rect(rectPosition, rectLength);
		}
		protected Rect GetSubRect(Rect sourceRect, int index, int rowCount){
			if(index >= rowCount)
				throw new System.InvalidOperationException("index out of range");
			float rectHeight = sourceRect.height/ rowCount;
			float rectPosY = index * rectHeight + sourceRect.y;
			return new Rect(new Vector2(sourceRect.x, rectPosY), new Vector2(sourceRect.width, rectHeight));
		}
		void MakeSureValuesAreInRange(Vector2 value){
			for(int i = 0; i < 2; i ++)
				if(value[i] < 0f || value[i] > 1f)
					throw new System.InvalidOperationException("value is not in range");
		}
		Rect rect1;
		Rect rect2;
		Rect rect3;
		Rect rect4;
		public bool thisGUIIsEnabled;
		public virtual void OnGUI(){
			if(thisGUIIsEnabled){
				GUI.color = Color.white;
				GUI.Label(rect1, "event: " + thisEventName);
				GUI.Label(rect2, "pos: " + thisPointerPos.ToString());
				GUI.Label(rect3, "del: " + thisPointerDelta.ToString());
				GUI.Label(rect4, "vel: " + thisPointerVel.ToString());
			}
		}
		Vector2 thisPointerPos;
		Vector2 thisPointerDelta;
		Vector2 thisPointerVel;
		string thisEventName = "no event";
		void UpdatePointerGUIData(ICustomEventData data, string eventName){
			thisPointerPos = data.position;
			thisPointerDelta = data.deltaPos;
			thisPointerVel = data.velocity;
			thisEventName = eventName;
		}
		/*  Activation and init */
			public virtual void GetReadyForActivation(IUIAActivationData passedData){
				thisDomainActivationData = CheckAndCreateDomainActivationData(passedData);
				thisUIM = thisDomainActivationData.uim;
				IUIImage uiImage = CreateUIImage();
				thisUIElement = CreateUIElement(uiImage);
				thisInputStateEngine = new UIAdaptorInputStateEngine(passedData.uim, this, thisDomainActivationData.processFactory);
				GetAllChildUIAsReadyForActivation(this.GetAllChildUIAs(), thisDomainActivationData);
			}
			protected IUIAActivationData thisDomainActivationData;
			protected IUIManager thisUIM;
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
				IUIImage uiImage = new UIImage(image, childWithImage, thisImageDefaultDarkness, thisImageDarkenedDarkness);
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
			public float thisImageDefaultDarkness;
			public float thisImageDarkenedDarkness;
			public void ActivateUIElement(){
				thisUIElement.ActivateRecursively();
			}
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
						UpdatePointerGUIData(customEventData, "OnPointerEnter");
					}
				}
			}
			public void OnPointerExit(PointerEventData eventData){
				if(thisUIM.TouchIDIsRegistered()){
					if(PointerIDMatchesTheRegistered(eventData.pointerId)){
						ICustomEventData customEventData = new CustomEventData(eventData, Time.deltaTime);
						thisInputStateEngine.OnPointerExit(customEventData);
						UpdatePointerGUIData(customEventData, "OnPointerExit");
					}
				}
			}
			public void OnPointerDown(PointerEventData eventData){
				if(!thisUIM.TouchIDIsRegistered()){
					thisUIM.RegisterTouchID(eventData.pointerId);
					ICustomEventData customEventData = new CustomEventData(eventData, Time.deltaTime);
					thisInputStateEngine.OnPointerDown(customEventData);
					UpdatePointerGUIData(customEventData, "OnPointerDown");
				}
			}
			public void OnPointerUp(PointerEventData eventData){
				if(thisUIM.TouchIDIsRegistered()){
					if(PointerIDMatchesTheRegistered(eventData.pointerId)){
						ICustomEventData customEventData = new CustomEventData(eventData, Time.deltaTime);
						thisInputStateEngine.OnPointerUp(customEventData);
						thisUIM.UnregisterTouchID();
						UpdatePointerGUIData(customEventData, "OnPointerUp");
					}
				}
			}
			/* OnEndDrag is needed too? */
			public void OnBeginDrag(PointerEventData eventData){
				if(thisUIM.TouchIDIsRegistered()){
					if(PointerIDMatchesTheRegistered(eventData.pointerId)){
						ICustomEventData customEventData = new CustomEventData(eventData, Time.deltaTime);
						thisInputStateEngine.OnBeginDrag(customEventData);
						UpdatePointerGUIData(customEventData, "OnBeginDrag");
					}
				}
			}
			public void OnDrag(PointerEventData eventData){
				if(thisUIM.TouchIDIsRegistered()){
					if(PointerIDMatchesTheRegistered(eventData.pointerId)){
						ICustomEventData customEventData = new CustomEventData(eventData, Time.deltaTime);
						thisInputStateEngine.OnDrag(customEventData);
						UpdatePointerGUIData(customEventData, "OnDrag");
					}
				}
			}
		/*  */
	}
}
