using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DKUtility;

namespace UISystem{
	public interface IUIManager{
		IUIElement GetReserveTransformUIE();
		void SetReserveTransformUIE(IUIElement reserveTransformUIE);
		void SetDragWorldPosition(Vector2 dragPos);
		Vector2 GetDragWorldPosition();
	}
	public class UIManager: IUIManager {
		IUIElement thisReserveTransformUIE;
		public void SetReserveTransformUIE(IUIElement reserveTransformUIE){
			thisReserveTransformUIE = reserveTransformUIE;
		}
		public IUIElement GetReserveTransformUIE(){
			return thisReserveTransformUIE;
		}
		Vector2 thisDragWorldPosition;
		public void SetDragWorldPosition(Vector2 dragPos){
			thisDragWorldPosition = dragPos;
		}
		public Vector2 GetDragWorldPosition(){return thisDragWorldPosition;}
	}
	public class UIManagerAdaptor: MonoBehaviour{
		IUIManager uiManager;
		public IProcessManager processManager;/* assigned in the inspector */
		public IUIAdaptor rootUIAdaptor;/* assigned in inspector*/
		public IUIElement reserveTransformUIE;
		public void Awake(){
			uiManager = new UIManager();
			uiManager.SetReserveTransformUIE(reserveTransformUIE);
			IUISystemProcessFactory processFactory = new UISystemProcessFactory(processManager, uiManager);
			IUIElementFactory uiElementFactory = new UIElementFactory(uiManager);
			IUIAActivationData rootUIAActivationArg = new RootUIAActivationData(uiManager, processFactory, uiElementFactory);
			rootUIAdaptor.GetReadyForActivation(rootUIAActivationArg);
		}
	}
}
