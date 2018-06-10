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
				this.processFacotory = new ProcessFactory(processManager, this);
			else
				throw new System.InvalidOperationException("ProcessManager is not assigned to UIManager in the inspector");
		}
		public IProcessFactory GetProcessFactory(){
			return processFacotory;
		}
		IProcessFactory processFacotory;
		public IProcessManager processManager;/* assigned in the inspector */
	}
	public class UIManagerAdaptor: MonoBehaviour{
		IUIManager uiManager;
		public IPickUpContextUIAdaptor rootUIAdaptor;/* assigned in inspector*/
		public void Awake(){
			uiManager = new UIManager();
			IUIAActivationData rootUIAActivationArg = new RootUIAActivationData(uiManager, factory:null);
			rootUIAdaptor.GetReadyForActivation(rootUIAActivationArg);
		}
	}
}
