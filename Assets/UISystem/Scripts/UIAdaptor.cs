using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IUIAdaptor{
		IUIElement GetUIElement();
		IUIElement GetParentUIE();
		List<IUIElement> GetChildUIEs();
		void GetReadyForActivation(IUIManager uim);
	}
	public abstract class AbsUIAdaptorMB: MonoBehaviour, IUIAdaptor{
		/* 
			Isolate MB-free part of implementation to some other class and 
			hold ref to it for the sake of easy testing
		 */
		protected IUIElement uiElement;
		public IUIElement GetUIElement(){return uiElement;}
		public IUIElement GetParentUIE(){
			IUIAdaptor closestParentUIAdaptor = FindClosestParentUIAdaptor();
			if(closestParentUIAdaptor != null)
				return closestParentUIAdaptor.GetUIElement();
			else
				return null;/* this should be the top one */
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
		List<IUIAdaptor> FindAllClosestChildUIAdaptors(Transform transToExamine){
			/* 
				find and return all most proximate child UIAdaptors
				in all children branches
				i.e. if any children does not have IUIAdaptor component,
				then the child transform must try search down its offsprings for hits
				and return them
			 */
			List<IUIAdaptor> result = new List<IUIAdaptor>();
			for(int i = 0; i < transToExamine.childCount; i ++){
				Transform child = transToExamine.GetChild(i);
				IUIAdaptor childUIA = child.GetComponent(typeof(IUIAdaptor)) as IUIAdaptor;
				if(childUIA != null){
					result.Add(childUIA);
				}else{
					List<IUIAdaptor> allUIAsOfThisChild = FindAllClosestChildUIAdaptors(child);
					if(allUIAsOfThisChild.Count != 0)
						result.AddRange(allUIAsOfThisChild);
				}
			}
			return result;
		}
		public void GetReadyForActivation(IUIManager uim){
			/* 
				Perform below tasks, 
				and call GetReadyForActivation(uim) for each child UIAdaptors

				1.Instantiate and store ref to UIElement
				2.uie.SetUIM(uim)
			 */
			this.CreateAndSetUIE(uim);
			foreach(IUIAdaptor childUIA in this.GetChildUIAdaptors()){
				childUIA.GetReadyForActivation(uim);
			}
		}
		protected abstract void CreateAndSetUIE(IUIManager uim);
			/* Instantiate corresponding UIE with this, uim, and some other required args
			 */
	}
}
