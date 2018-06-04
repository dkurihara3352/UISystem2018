using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IIconPanel: IPickUpReceiver{
		IIconGroup GetRelevantIG();
	}
	public abstract class AbsIconPanel: AbsUIElement, IIconPanel{
		public AbsIconPanel(IUIManager uim, IUIAdaptor uia) :base(uim, uia){}
		public void CheckForHover(){}
		public void WaitForPickUp(){}
		public abstract IIconGroup GetRelevantIG();
		/* panel transaction state handling */
		readonly IPanelTransactionStateEngine panTAStateEngine;
		public void BecomeHoverable(){
			panTAStateEngine.BecomeHoverable();
		}
		public void BecomeUnhoverable(){
			panTAStateEngine.BecomeUnhoverable();
		}
	}
	public interface IPanelTransactionStateEngine: IHoverabilityStateHandler{}
	public class EquippedItemsPanel: AbsIconPanel{
		public EquippedItemsPanel(IUIManager uim, IUIAdaptor uia): base(uim, uia){}
		public override IUIImage CreateUIImage(){
			return null;/* imple later */
		}
		public override IIconGroup GetRelevantIG(){
			/* impled later when building Scrollers */
			return null;
		}
	}
	public class PoolItemsPanel: AbsIconPanel{
		public PoolItemsPanel(IUIManager uim, IUIAdaptor uia): base(uim, uia){}
		public override IUIImage CreateUIImage(){
			return null;/* imple later */
		}
		public override IIconGroup GetRelevantIG(){
			/* impled later when building Scrollers */
			return null;
		}
	}
}
