using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IUIAdaptorBaseInitializationData{
		IUIManager uim{get;}
		IUISystemProcessFactory processFactory{get;}
		IUIElementFactory uiElementFactory{get;}
	}
	public abstract class AbsUIAActivationData: IUIAdaptorBaseInitializationData{
		public AbsUIAActivationData(IUIManager uim, IUISystemProcessFactory processFactory, IUIElementFactory uiElementFactory){
			thisUIM = uim;
			thisProcessFactory = processFactory;
			thisUIElementFactory = uiElementFactory;
		}
		readonly IUIManager thisUIM;
		public IUIManager uim{get{return thisUIM;}}
		readonly IUISystemProcessFactory thisProcessFactory;
		public IUISystemProcessFactory processFactory{get{return thisProcessFactory;}}
		readonly IUIElementFactory thisUIElementFactory;
		public IUIElementFactory uiElementFactory{get{return thisUIElementFactory;}}
	}
	public class RootUIAActivationData: AbsUIAActivationData{
		public RootUIAActivationData(IUIManager uim, IUISystemProcessFactory processFactory, IUIElementFactory uiElementFactory): base(uim, processFactory, uiElementFactory){}
	}
}
