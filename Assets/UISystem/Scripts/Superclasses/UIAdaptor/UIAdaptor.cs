using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
namespace UISystem{
	public interface IMBAdaptor{
		Transform GetTransform();
		Rect GetRect();
		Vector2 GetLocalPosition();
		void SetLocalPosition(Vector2 localPos);
		Vector2 GetWorldPosition();
		void SetWorldPosition(Vector2 worldPos);
		Vector2 GetPositionInThisSpace(Vector2 worldPos);
	}
	public interface IUIAdaptor: IMBAdaptor{
		void GetReadyForActivation(IUIAActivationData passedData);
		IUIElement GetUIElement();
		IUIElement GetParentUIE();
		void SetParentUIE(IUIElement newParentUIE, bool worldPositionStays);
		List<IUIElement> GetAllOffspringUIEs();
		List<IUIElement> GetChildUIEs();
		IUIAActivationData GetDomainActivationData();
	}
	public abstract class AbsUIAdaptor<T>: MonoBehaviour, IUIAdaptor, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler, IDragHandler where T: IUIElement{
		/*  Activation and init */
			public virtual void GetReadyForActivation(IUIAActivationData passedData){
				thisDomainActivationData = CheckAndCreateDomainActivationData(passedData);
				thisUIElement = CreateUIElement();
				thisInputStateEngine = new UIAdaptorStateEngine(passedData.uim, this, thisDomainActivationData.processFactory);
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
		/* MB adaptor */
			public Transform GetTransform(){
				return this.transform;
			}
			public Rect GetRect(){
				return ((RectTransform)this.GetComponent<RectTransform>()).rect;
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
			protected abstract T CreateUIElement();
			T thisUIElement;
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
			IUIAdaptorStateEngine thisInputStateEngine;
			public virtual void OnPointerEnter(PointerEventData eventData){
				ICustomEventData customEventData = new CustomEventData(eventData);
				thisInputStateEngine.OnPointerEnter(customEventData);
			}
			public void OnPointerDown(PointerEventData eventData){
				ICustomEventData customEventData = new CustomEventData(eventData);
				thisInputStateEngine.OnPointerDown(customEventData);
			}
			public void OnPointerUp(PointerEventData eventData){
				ICustomEventData customEventData = new CustomEventData(eventData);
				thisInputStateEngine.OnPointerUp(customEventData);
			}
			public void OnDrag(PointerEventData eventData){
				ICustomEventData customEventData = new CustomEventData(eventData);
				thisInputStateEngine.OnDrag(customEventData);
			}
		/*  */
	}
	public interface ICustomEventData{
		Vector2 deltaP{get;}
		Vector2 position{get;}
	}
	public struct CustomEventData: ICustomEventData{
		public CustomEventData(PointerEventData sourceData){
			/* do some conversion here */
			thisDeltaP = sourceData.delta;
			thisPosition = sourceData.position;
		}
		public Vector2 deltaP{get{return thisDeltaP;}}
		Vector2 thisDeltaP;
		public Vector2 position{get{return thisPosition;}}
		Vector2 thisPosition;
	}
}
