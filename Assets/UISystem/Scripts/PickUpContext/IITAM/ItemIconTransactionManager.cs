using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IIITAMStateHandler{
		void SetToPickedState(IItemIcon pickedII);
		void SetToDefaultState();
	}
	public interface IItemIconTransactionManager: IPickUpManager ,IIITAMStateHandler{
		void SetPickedII(IItemIcon pickedII);
		IItemIcon GetPickedII();
		void Activate();
		void CheckAndActivateHoverPads();
		void DeactivateHoverPads();
		void EvaluateHoverability();
		void ClearHoverability();
		void HoverInitialPickUpReceiver();
		void EvaluatePickability();
		List<IIconGroup> GetAllRelevantIGs();
		IItemIcon CreateItemIcon(IUIItem item);
	}
	public abstract class AbsItemIconTransactionManager: AbsPickUpManager, IItemIconTransactionManager{
		public AbsItemIconTransactionManager(){
			this.stateEngine = new ItemIconTAManagerStateEngine();
		}
		readonly IItemIcomTAManagerStateEngine stateEngine;
		public void Activate(){
			SetToDefaultState();
		}
		public void SetToPickedState(IItemIcon pickedII){
			this.stateEngine.SetToPickedState(pickedII);
		}
		public void SetToDefaultState(){
			this.stateEngine.SetToDefaultState();
		}
		public void SetPickedII(IItemIcon pickedII){
			this.pickedUIE = pickedII;
		}
		public IItemIcon GetPickedII(){
			IPickableUIE pickedUIE = this.GetPickedUIE();
			if(pickedUIE != null){
				if(pickedUIE is IItemIcon)
					return pickedUIE as IItemIcon;
				else
					throw new System.InvalidCastException("pickedUIE is not of type IItemIcon");
			}
			return null;
		}
		IIconPanel hoveredPanel;
		IItemIcon hoveredII;
		public void CheckAndActivateHoverPads(){
			foreach(IIconGroup ig in GetAllRelevantIGs()){
				ig.ActivateHoverPads();
			}
		}
		public void DeactivateHoverPads(){
			foreach(IIconGroup ig in this.GetAllRelevantIGs())
				ig.DeactivateHoverPads();
		}
		public abstract void EvaluateHoverability();
		// public virtual void ClearHoverability(){}
		// public void HoverInitialPickUpReceiver(){}
		public override void ClearTAFields(){
			base.ClearTAFields();
			this.hoveredPanel = null;
			this.hoveredII = null;
		}
		public void EvaluatePickability(){
			foreach(IIconGroup ig in GetAllRelevantIGs()){
				ig.EvaluateAllIIsPickability();
			}
		}
		public abstract List<IIconGroup> GetAllRelevantIGs();
		public abstract IItemIcon CreateItemIcon(IUIItem item);
	}
}

