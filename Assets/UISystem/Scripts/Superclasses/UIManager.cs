using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IUIManager{
		IProcessFactory GetProcessFactory();
	}
	public class UIManager: IUIManager {
		public UIManager(){
			if(processManager != null)
				thisProcessFactory = new ProcessFactory(processManager, this);
			else
				throw new System.InvalidOperationException("ProcessManager is not assigned to UIManager in the inspector");
		}
		public IProcessFactory GetProcessFactory(){
			return thisProcessFactory;
		}
		IProcessFactory thisProcessFactory;
		public IProcessManager processManager;/* assigned in the inspector */
	}
	public class UIManagerAdaptor: MonoBehaviour{
		IUIManager uiManager;
		public IPickUpContextUIAdaptor rootUIAdaptor;/* assigned in inspector*/
		public void Awake(){
			uiManager = new UIManager();
			IUIAActivationData rootUIAActivationArg = new RootUIAActivationData(uiManager, uieFactory:null, pum:null);
			rootUIAdaptor.GetReadyForActivation(rootUIAActivationArg);
		}
	}
}
