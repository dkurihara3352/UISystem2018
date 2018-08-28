using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem.PickUpUISystem{
	public interface IPickUpSystemUIAActivationData: IUIAdaptorBaseInitializationData{
		IPickUpManager pickUpManager{get;}
		IPickUpSystemProcessFactory pickUpSystemProcessFactory{get;}
		IPickUpSystemUIElementFactory pickUpSystemUIElementFactory{get;}
	}
	public abstract class AbsPickUpSystemUIAActivationData: AbsUIAActivationData, IPickUpSystemUIAActivationData{
		public AbsPickUpSystemUIAActivationData(IUIManager uim, IPickUpSystemProcessFactory pickUpSystemProcessFactory, IPickUpSystemUIElementFactory pickUpSystemUIElementFactory, IPickUpManager pickUpManager): base(uim, pickUpSystemProcessFactory, pickUpSystemUIElementFactory){
			thisPickUpManager = pickUpManager;
		}
		readonly IPickUpManager thisPickUpManager;
		public IPickUpManager pickUpManager{get{return thisPickUpManager;}}
		public IPickUpSystemProcessFactory pickUpSystemProcessFactory{get{return (IPickUpSystemProcessFactory)processFactory;}}
		public IPickUpSystemUIElementFactory pickUpSystemUIElementFactory{get{return (IPickUpSystemUIElementFactory)uiElementFactory;}}
	}
}
