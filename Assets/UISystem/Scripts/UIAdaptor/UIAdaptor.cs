using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace UISystem{
	[RequireComponent(typeof(RectTransform))]
	public class UIAdaptor: /* MonoBehaviour */Selectable, IUIAdaptor, /* ISelectHandler, IDeselectHandler,  */IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, ICancelHandler{
		protected override void Awake(){
			RectTransform rectTrans = transform.GetComponent<RectTransform>();
			rectTrans.pivot = new Vector2(0f, 0f);
			rectTrans.anchorMin = new Vector2(0f, 0f);
			rectTrans.anchorMax = new Vector2(0f, 0f);
		}
		/*  Activation and init */
			public virtual void GetReadyForActivation(IUIAActivationData passedData){
				thisDomainActivationData = CheckAndCreateDomainActivationData(passedData);
				IUIImage uiImage = CreateUIImage();
				thisUIElement = CreateUIElement(uiImage);
				thisInputStateEngine = new UIAdaptorInputStateEngine(passedData.uim, this, thisDomainActivationData.processFactory);
				GetAllChildUIAsReadyForActivation(this.GetAllChildUIAs(), thisDomainActivationData);
			}
			protected IUIAActivationData thisDomainActivationData;
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
				imageRectTrans.sizeDelta = this.GetRect().size;
				imageRectTrans.anchoredPosition = Vector3.zero;
				IUIImage uiImage = new UIImage(image, childWithImage, imageDefaultDarkness, imageDarkenedDarkness);
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
			public float imageDefaultDarkness;
			public float imageDarkenedDarkness;
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
				rectTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
				rectTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
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
			public override void OnPointerEnter(PointerEventData eventData){
				ICustomEventData customEventData = new CustomEventData(eventData, Time.deltaTime);
				thisInputStateEngine.OnPointerEnter(customEventData);
				base.OnPointerEnter(eventData);
			}
			public override void OnPointerExit(PointerEventData eventData){
				ICustomEventData customEventData = new CustomEventData(eventData, Time.deltaTime);
				thisInputStateEngine.OnPointerExit(customEventData);
				base.OnPointerExit(eventData);
			}
			public override void OnPointerDown(PointerEventData eventData){
				ICustomEventData customEventData = new CustomEventData(eventData, Time.deltaTime);
				thisInputStateEngine.OnPointerDown(customEventData);
				base.OnPointerDown(eventData);
			}
			public override void OnPointerUp(PointerEventData eventData){
				ICustomEventData customEventData = new CustomEventData(eventData, Time.deltaTime);
				thisInputStateEngine.OnPointerUp(customEventData);
				base.OnPointerUp(eventData);
			}
			public void OnBeginDrag(PointerEventData eventData){
				ICustomEventData customEventData = new CustomEventData(eventData, Time.deltaTime);
				thisInputStateEngine.OnBeginDrag(customEventData);
			}
			public void OnDrag(PointerEventData eventData){
				ICustomEventData customEventData = new CustomEventData(eventData, Time.deltaTime);
				thisInputStateEngine.OnDrag(customEventData);
			}
			public void OnCancel(BaseEventData eventData){
				thisInputStateEngine.OnCancel();
			}
		/*  */
	}
}
