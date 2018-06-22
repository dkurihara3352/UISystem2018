using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
namespace UISystem{
	public interface IMBAdaptor{
		Transform GetTransform();
		Vector2 GetLocalPosition();
		void SetLocalPosition(Vector2 localPos);
		Vector2 GetWorldPosition();
		void SetWorldPosition(Vector2 worldPos);
		Vector2 GetPositionInThisSpace(Vector2 worldPos);
		IUIAdaptor GetParentUIA();
		void SetParentUIA(IUIAdaptor parent, bool worldPositionStays);
	}
	public interface IUIAdaptor: IMBAdaptor{
		void GetReadyForActivation(IUIAActivationData passedData);
		IUIElement GetUIElement();
		IUIElement GetParentUIE();
		void SetParentUIE(IUIElement newParentUIE, bool worldPositionStays);
		List<IUIAdaptor> GetAllOffspringUIAs();
		List<IUIElement> GetChildUIEs();
	}
	public abstract class AbsUIAdaptor<T>: MonoBehaviour, IUIAdaptor, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler, IDragHandler where T: IUIElement{
		/*  Activation and init */
			public void Awake(){

			}
			public virtual void GetReadyForActivation(IUIAActivationData passedData){
				thisDomainActivationData = CheckAndCreateDomainActivationData(passedData);
				thisUIElement = CreateUIElement(thisDomainActivationData.uieFactory);
				thisInputStateEngine = new UIAdaptorStateEngine(this, thisDomainActivationData.processFactory);
				GetAllChildUIAsReadyForActivation(this.GetChildUIAdaptors(), thisDomainActivationData);
			}
			protected IUIAActivationData thisDomainActivationData;
			IUIAActivationData CheckAndCreateDomainActivationData(IUIAActivationData passedData){
				IUIAActivationData result = null;
				if(this is IPickUpContextUIAdaptor){
					result = ((IPickUpContextUIAdaptor)this).CreateDomainActivationData(passedData);
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
			public IUIAdaptor GetParentUIA(){
				return FindClosestParentUIAdaptor();
			}
			public void SetParentUIA(IUIAdaptor parentUIA, bool worldPositionStays){
				this.transform.SetParent(parentUIA.GetTransform(), worldPositionStays);
			}
		/*  Hierarchy stuff */
			protected abstract T CreateUIElement(IUIElementFactory factory);
			T thisUIElement;
			public IUIElement GetUIElement(){
				return thisUIElement;
			}
			public IUIElement GetParentUIE(){
				IUIAdaptor closestParentUIAdaptor = FindClosestParentUIAdaptor();
				if(closestParentUIAdaptor != null)
					return closestParentUIAdaptor.GetUIElement();
				else
					return null;/* this should be the top one */
			}
			public void SetParentUIE(IUIElement newParentUIE, bool worldPositionStays){
				IUIAdaptor newParUIA = newParentUIE.GetUIAdaptor();
				this.SetParentUIA(newParUIA, worldPositionStays);
			}
			IUIAdaptor FindClosestParentUIAdaptor(){/* any change in this must be reflected in corresponding pseudo transform test */
				Transform parentToExamine = transform.parent;
				while(true){
					if(parentToExamine != null){
						IUIAdaptor parentUIAdaptor = parentToExamine.GetComponent(typeof(IUIAdaptor)) as IUIAdaptor;
						if(parentUIAdaptor != null){
							return parentUIAdaptor;
						}else{
							parentToExamine = parentToExamine.parent;
						}
					}else{
						return null;/* top of the hierarchy */
					}
				}
			}
			public List<IUIElement> GetChildUIEs(){
				List<IUIElement> result = new List<IUIElement>();
				List<IUIAdaptor> childUIAs = this.GetChildUIAdaptors();
				foreach(IUIAdaptor uia in childUIAs)
					result.Add(uia.GetUIElement());
				return result;
			}
			List<IUIAdaptor> GetChildUIAdaptors(){
				return FindAllClosestChildUIAdaptors(this.transform);
			}
			public List<IUIAdaptor> GetAllOffspringUIAs(){
				List<IUIAdaptor> result = new List<IUIAdaptor>();
				foreach(IUIAdaptor childUIA in this.GetChildUIAdaptors()){
					result.AddRange(childUIA.GetAllOffspringUIAs());
				}
				return result;
			}
			protected List<U> FindAllNextOffspringsWithComponent<U>(Transform transToExamine) where U: class{
				/* 
					find and return all most proximate child Components
					in all children branches
					i.e. if any children does not have the component,
					then the child transform must try search down its offsprings for hits
					and return them

					Make sure the component inherits both from mono behaviour and from T
				*/
				List<U> result = new List<U>();
				for(int i = 0; i < transToExamine.childCount; i ++){
					Transform child = transToExamine.GetChild(i);
					U childComp = child.GetComponent(typeof(U)) as U;
					if(childComp != null){
						result.Add(childComp);
					}else{
						List<U> allChildCompsOfThisChild = FindAllNextOffspringsWithComponent<U>(child);
						if(allChildCompsOfThisChild.Count != 0)
							result.AddRange(allChildCompsOfThisChild);
					}
				}
				return result;
			}
			List<IUIAdaptor> FindAllClosestChildUIAdaptors (Transform transToExamine){
				return FindAllNextOffspringsWithComponent<IUIAdaptor>(transToExamine);
			}
		/* Event System Imple */
			IUIAdaptorStateEngine thisInputStateEngine;
			public void OnPointerEnter(PointerEventData eventData){
				if(this.GetUIElement() is IPickUpReceiver){
					IPickUpReceiver receiver = (IPickUpReceiver)this.GetUIElement();
					receiver.CheckForHover();
				}
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
