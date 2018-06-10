using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IIconPanel: IPickUpReceiver{
		IIconGroup GetRelevantIG();
	}
	public abstract class AbsIconPanel: AbsUIElement, IIconPanel{
		public AbsIconPanel(IUIManager uim, IUIAdaptor uia, IUIImage image) :base(uim, uia, image){}
		protected override void ActivateImple(){
			base.ActivateImple();
			WaitForPickUp();
		}
		public void CheckForHover(){}
		public abstract IIconGroup GetRelevantIG();
		/* panel transaction state handling */
		readonly IPanelTransactionStateEngine panTAStateEngine;
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
	}
	public class EquippedItemsPanel: AbsIconPanel{
		public EquippedItemsPanel(IUIManager uim, IUIAdaptor uia, IUIImage image) :base(uim, uia, image){}
		public override IIconGroup GetRelevantIG(){
			/* impled later when building Scrollers */
			return null;
		}
	}
	public class PoolItemsPanel: AbsIconPanel{
		public PoolItemsPanel(IUIManager uim, IUIAdaptor uia, IUIImage image) :base(uim, uia, image){}
		public override IIconGroup GetRelevantIG(){
			/* impled later when building Scrollers */
			return null;
		}
	}
	public interface IPanelTransactionStateEngine: IHoverabilityStateHandler{}
}
