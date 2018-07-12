using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IUIAActivationData{
		IUIManager uim{get;}
		IProcessFactory processFactory{get;}
		IUIElementFactory uiElementFactory{get;}
	}
	public abstract class AbsUIAActivationData: IUIAActivationData{
		public AbsUIAActivationData(IUIManager uim, IProcessFactory processFactory, IUIElementFactory uiElementFactory){
			thisUIM = uim;
			thisProcessFactory = processFactory;
			thisUIElementFactory = uiElementFactory;
		}
		readonly IUIManager thisUIM;
		public IUIManager uim{get{return thisUIM;}}
		readonly IProcessFactory thisProcessFactory;
		public IProcessFactory processFactory{get{return thisProcessFactory;}}
		readonly IUIElementFactory thisUIElementFactory;
		public IUIElementFactory uiElementFactory{get{return thisUIElementFactory;}}
	}
	public class RootUIAActivationData: AbsUIAActivationData{
		public RootUIAActivationData(IUIManager uim, IProcessFactory processFactory, IUIElementFactory uiElementFactory): base(uim, processFactory, uiElementFactory){}
	}
}
