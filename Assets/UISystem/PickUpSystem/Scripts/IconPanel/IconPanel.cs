using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem.PickUpUISystem{
	public interface IIconPanel: IPickUpReceiver, IUIElement{
	}
	public abstract class AbsIconPanel: UIElement, IIconPanel{
		public AbsIconPanel(IUIElementConstArg arg) :base(arg){
		}
		protected override void OnUIActivate(){
			WaitForPickUp();
		}
		public abstract void CheckForHover();
		protected IPanelTransactionStateEngine thisPanelTransactionStateEngine;
		public void EvaluateHoverability(IPickableUIE pickedUIE){
			IItemIcon pickedII = (IItemIcon)pickedUIE;
			if(this.IsEligibleForHover(pickedII))
				BecomeHoverable();
			else
				BecomeUnhoverable();
		}
		protected abstract bool IsEligibleForHover(IItemIcon pickedII);
		public void WaitForPickUp(){
			thisPanelTransactionStateEngine.WaitForPickUp();
		}
		public void BecomeHoverable(){
			thisPanelTransactionStateEngine.BecomeHoverable();
		}
		public void BecomeUnhoverable(){
			thisPanelTransactionStateEngine.BecomeUnhoverable();
		}
		public void BecomeHovered(){
			thisPanelTransactionStateEngine.BecomeHovered();
		}
		public bool IsHoverable(){
			return thisPanelTransactionStateEngine.IsHoverable();
		}
		public bool IsHovered(){
			return thisPanelTransactionStateEngine.IsHovered();
		}
	}
}
