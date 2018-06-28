using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IUIManager{
		IProcessFactory GetProcessFactory();
		IReserveTransformUIE GetReserveTransformUIE();
		void SetReserveTransformUIE(IReserveTransformUIE reserveTransformUIE);
	}
	public class UIManager: IUIManager {
		public UIManager(){
			if(thisProcessManager != null)
				thisProcessFactory = new ProcessFactory(thisProcessManager, this);
			else
				throw new System.InvalidOperationException("ProcessManager is not assigned to UIManager in the inspector");
		}

		public IProcessFactory GetProcessFactory(){
			return thisProcessFactory;
		}
		readonly IProcessFactory thisProcessFactory;
		public IProcessManager thisProcessManager;/* assigned in the inspector */
		IReserveTransformUIE thisReserveTransformUIE;
		public void SetReserveTransformUIE(IReserveTransformUIE reserveTransformUIE){
			thisReserveTransformUIE = reserveTransformUIE;
		}
		public IReserveTransformUIE GetReserveTransformUIE(){
			return thisReserveTransformUIE;
		}
	}
	public class UIManagerAdaptor: MonoBehaviour{
		IUIManager uiManager;
		public IRootUIAdaptor rootUIAdaptor;/* assigned in inspector*/
		public void Awake(){
			uiManager = new UIManager();
			uiManager.SetReserveTransformUIE(rootUIAdaptor.GetReserveTransformUIE());
			IUIAActivationData rootUIAActivationArg = new RootUIAActivationData(uiManager, uieFactory:null, pum:null);
			rootUIAdaptor.GetReadyForActivation(rootUIAActivationArg);
		}
	}
	public interface IRootUIAdaptor: IPickUpContextUIAdaptor{
		IReserveTransformUIE GetReserveTransformUIE();
	}
	public interface IReserveTransformUIE: IUIElement{
		Vector2 GetReservePosition();
	}
}
