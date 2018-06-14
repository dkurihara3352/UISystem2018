using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IIconPanel: IPickUpReceiver, IUIElement{
		void EvaluateHoverability(IItemIcon pickedII);
	}
	public abstract class AbsIconPanel: AbsUIElement, IIconPanel{
		public AbsIconPanel(IUIElementConstArg arg) :base(arg){
		}
		protected override void ActivateImple(){
			base.ActivateImple();
			WaitForPickUp();
		}
		public abstract void CheckForHover();
		protected IPanelTransactionStateEngine<IPanelTransactionState> panTAStateEngine;
		public void EvaluateHoverability(IItemIcon pickedII){
			if(this.IsEligibleForHover(pickedII))
				BecomeHoverable();
			else
				BecomeUnhoverable();
		}
		protected abstract bool IsEligibleForHover(IItemIcon pickedII);
		public void WaitForPickUp(){
			panTAStateEngine.WaitForPickUp();
		}
		public void BecomeHoverable(){
			panTAStateEngine.BecomeHoverable();
		}
		public void BecomeUnhoverable(){
			panTAStateEngine.BecomeUnhoverable();
		}
		public void BecomeHovered(){
			panTAStateEngine.BecomeHovered();
		}
		public bool IsHoverable(){
			return panTAStateEngine.IsHoverable();
		}
		public bool IsHovered(){
			return panTAStateEngine.IsHovered();
		}
	}
}
