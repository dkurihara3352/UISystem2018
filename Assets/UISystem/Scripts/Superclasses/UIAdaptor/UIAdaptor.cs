﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
namespace UISystem{
	public interface IMBAdaptor{}
	public interface IUIAdaptor: IMBAdaptor{
		IUIElement GetUIElement();
		IUIElement GetParentUIE();
		void SetUIEParent(IUIElement newParentUIE);
		List<IUIAdaptor> GetAllOffspringUIAs();
		List<IUIElement> GetChildUIEs();
		void GetReadyForActivation(IUIAActivationData passedArg);
		/*  IPickUpContextUIA is fed with null factory
				it needs to create its own domain factory and pass it to every domain elements
		*/
		Vector2 GetLocalPosition(Vector2 worldPos);
		void SetLocalPosition(Vector2 localPos);
		Transform GetParentTrans();
	}
	public abstract class AbsUIAdaptor<T>: MonoBehaviour, IUIAdaptor, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler, IDragHandler where T: IUIElement{
		/*  Activation and init */
			protected IUIAActivationData activationData;
			public virtual void GetReadyForActivation(IUIAActivationData passedData){
				/* Perform all initialization here */
				IUIAActivationData domainData = null;
					/*  pick up context activation data
							factory
							tool ?
							transaction manager
					*/
				if(this is IPickUpContextUIAdaptor){
					domainData = ((IPickUpContextUIAdaptor)this).CreateDomainActivationData(passedData.uim);
				}else{
					domainData = passedData;
				}
				IUIElementFactory factory = domainData.factory;
				this.uiElement = this.CreateUIElement(factory);
				foreach(IUIAdaptor childUIA in this.GetChildUIAdaptors()){
					childUIA.GetReadyForActivation(domainData);
				}
				IUIManager uim = domainData.uim;
				IUIAdaptorStateEngine engine = new UIAdaptorStateEngine(this, uim.GetProcessFactory());
				SetInputStateEngine(engine);
				this.activationData = domainData;
			}
			void SetInputStateEngine(IUIAdaptorStateEngine engine){
				this.inputStateEngine = engine;
			}
			IUIAdaptorStateEngine inputStateEngine;
		/*  Hierarchy stuff */
			protected abstract T CreateUIElement(IUIElementFactory factory);
			T uiElement;
			public IUIElement GetUIElement(){
				return uiElement;
			}
			public IUIElement GetParentUIE(){
				IUIAdaptor closestParentUIAdaptor = FindClosestParentUIAdaptor();
				if(closestParentUIAdaptor != null)
					return closestParentUIAdaptor.GetUIElement();
				else
					return null;/* this should be the top one */
			}
			public Transform GetParentTrans(){
				return transform.parent;
			}
			public void SetUIEParent(IUIElement newParentUIE){
				IUIAdaptor newParUIA = newParentUIE.GetUIAdaptor();
				Transform newParTrans = newParUIA.GetParentTrans();
				this.transform.SetParent(newParTrans, worldPositionStays:true);
			}
			IUIAdaptor FindClosestParentUIAdaptor(){
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
			CustomEventData CreateCustomEventData(PointerEventData sourceData){
				CustomEventData result = new CustomEventData(sourceData);
				return result;
			}
			Vector2 CalcPointerPos(PointerEventData eventData){
				return Vector2.zero;
			}
			Vector2 CalcPointerDelta(PointerEventData eventData){
				return Vector2.zero;
			}
			public void OnPointerEnter(PointerEventData eventData){
				if(this.GetUIElement() is IPickUpReceiver){
					IPickUpReceiver receiver = (IPickUpReceiver)this.GetUIElement();
					receiver.CheckForHover();
				}
			}
			public void OnPointerDown(PointerEventData eventData){
				CustomEventData customEventData = CreateCustomEventData(eventData);
				inputStateEngine.OnPointerDown(customEventData);
			}
			public void OnPointerUp(PointerEventData eventData){
				CustomEventData customEventData = CreateCustomEventData(eventData);
				inputStateEngine.OnPointerUp(customEventData);
			}
			public void OnDrag(PointerEventData eventData){
				Vector2 dragPos = this.CalcPointerPos(eventData);
				Vector2 dragDeltaP = this.CalcPointerDelta(eventData);
				inputStateEngine.OnDrag(dragPos, dragDeltaP);
			}
		/*  */
		public Vector2 GetLocalPosition(Vector2 worldPos){
			Vector3 localPosV3 = transform.InverseTransformPoint(new Vector3(worldPos.x, worldPos.y, 0f));
			return new Vector3(localPosV3.x, localPosV3.y);
		}
		public void SetLocalPosition(Vector2 localPos){
			transform.localPosition = new Vector3(localPos.x, localPos.y, 0f);
		}
	}
	public interface ICustomEventData{
		Vector2 deltaP{get;}
	}
	public class CustomEventData: ICustomEventData{
		public CustomEventData(PointerEventData sourceData){
			/* do some conversion here */
		}
		public Vector2 deltaP{
			get{return Vector2.zero;}
		}
	}
	public interface IPreactivationArg{
		IUIManager uim{get;}
	}
}
