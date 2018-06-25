using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IIconPanel: IPickUpReceiver, IUIElement{
	}
	public abstract class AbsIconPanel: AbsUIElement, IIconPanel{
		public AbsIconPanel(IUIElementConstArg arg) :base(arg){
		}
		protected override void ActivateImple(){
			base.ActivateImple();
			WaitForPickUp();
		}
		public abstract void CheckForHover();
		protected IPanelTransactionStateEngine panelTransactionStateEngine;
		public void EvaluateHoverability(IPickableUIE pickedUIE){
			IItemIcon pickedII = (IItemIcon)pickedUIE;
			if(this.IsEligibleForHover(pickedII))
				BecomeHoverable();
			else
				BecomeUnhoverable();
		}
		protected abstract bool IsEligibleForHover(IItemIcon pickedII);
		public void WaitForPickUp(){
			panelTransactionStateEngine.WaitForPickUp();
		}
		public void BecomeHoverable(){
			panelTransactionStateEngine.BecomeHoverable();
		}
		public void BecomeUnhoverable(){
			panelTransactionStateEngine.BecomeUnhoverable();
		}
		public void BecomeHovered(){
			panelTransactionStateEngine.BecomeHovered();
		}
		public bool IsHoverable(){
			return panelTransactionStateEngine.IsHoverable();
		}
		public bool IsHovered(){
			return panelTransactionStateEngine.IsHovered();
		}
	}
}
