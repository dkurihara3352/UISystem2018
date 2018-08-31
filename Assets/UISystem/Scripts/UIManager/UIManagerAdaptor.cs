using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DKUtility;

namespace UISystem{
	public class UIManagerAdaptor: MonoBehaviour{
		IUIManager thisUIManager;
		public IUIManager GetUIManager(){
			return thisUIManager;
		}
		public ProcessManager processManager;
		public UIAdaptor rootUIAdaptor;/* assigned in inspector*/
		public RectTransform uieReserveTrans;
		public bool showsInputability;
		public float uiImageDarkenedBrightness = .4f;
		public float uiImageDefaultBrightness = .8f;
		public float swipeVelocityThreshold = 400f;

		public void Awake(){
			thisUIManager = new UIManager(
				processManager,
				rootUIAdaptor,
				uieReserveTrans,
				showsInputability,
				uiImageDarkenedBrightness,
				uiImageDefaultBrightness,
				swipeVelocityThreshold
			);
		}
	}	
}
