using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IPopUpButton: IUIElement{
		void SetTargetPopUp(IPopUp popUp);
	}
	public class PopUpButton : UIElement {
		public PopUpButton(
			IPopUpButtonConstArg arg
		): base(
			arg
		){
			thisTargetPopUpAdaptor = arg.targetPopUpAdaptor;
		}
		readonly IPopUpAdaptor thisTargetPopUpAdaptor;
		IPopUp thisTargetPopUp = null;
		public void SetTargetPopUp(IPopUp popUp){
			/* called from uia.SetUpUIEReference */
			thisTargetPopUp = popUp;
		}
		// protected override void SetUpUIEReference(){
		// 	thisTargetPopUp = (IPopUp)thisTargetPopUpAdaptor.GetUIElement();
		// }
		// protected override void OnUIReferenceSet(){
		// 	/* To SetUpUIEReference */
		// 	base.OnUIReferenceSet();
		// 	thisTargetPopUp = (IPopUp)thisTargetPopUpAdaptor.GetUIElement();
		// }

		protected override void OnTapImple(int tapCount){
			ToggePopUp();
		}
		void ToggePopUp(){
			if(thisTargetPopUp.IsShown())
				thisTargetPopUp.Hide(false);
			else
				thisTargetPopUp.Show(false);
		}
	}



	public interface IPopUpButtonConstArg: IUIElementConstArg{
		IPopUpAdaptor targetPopUpAdaptor{get;}
	}
	public class PopUpButtonConstArg: UIElementConstArg, IPopUpButtonConstArg{
		public PopUpButtonConstArg(
			IUIManager uiManager,
			IUISystemProcessFactory processFactory,
			IUIElementFactory uiElementFactory,
			IPopUpButtonAdaptor popUpButtonAdaptor,
			IUIImage uiImage,
			ActivationMode activationMode,

			IPopUpAdaptor targetPopUpAdaptor
		): base(
			uiManager,
			processFactory,
			uiElementFactory,
			popUpButtonAdaptor,
			uiImage,
			activationMode
		){
			thisTargetPopUpAdaptor = targetPopUpAdaptor;
		}
		readonly IPopUpAdaptor thisTargetPopUpAdaptor;
		public IPopUpAdaptor targetPopUpAdaptor{get{return thisTargetPopUpAdaptor;}}
	}
}
