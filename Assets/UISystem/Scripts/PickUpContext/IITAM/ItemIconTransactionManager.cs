using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IItemIconTransactionManager: IPickUpManager ,IIITAMStateHandler{
		void SetPickedII(IItemIcon pickedII);
		IItemIcon GetPickedII();
		void Activate();
		void CheckAndActivateHoverPads();
		void DeactivateHoverPads();
		void EvaluateHoverability();
		void ResetHoverability();
		void HoverInitialPickUpReceiver();
		void EvaluatePickability();
		List<IIconGroup> GetAllRelevantIGs(IItemIcon pickedII);
		IItemIcon CreateItemIcon(IUIItem item);
		void ClearTAFields();
		void ExecuteTransaction();
	}
	public abstract class AbsItemIconTransactionManager: AbsPickUpManager, IItemIconTransactionManager{
		/* tam state */
			readonly IItemIconTAManagerStateEngine stateEngine;
			public void Activate(){
				SetToDefaultState();
			}
			public void SetToPickedState(IItemIcon pickedII){
				this.stateEngine.SetToPickedState(pickedII);
			}
			public void SetToDefaultState(){
				this.stateEngine.SetToDefaultState();
			}
			public bool IsInPickedUpState(){
				return this.stateEngine.IsInPickedUpState();
			}
			public bool IsInDefaultState(){
				return this.stateEngine.IsInDefaultState();
			}
		/*  */
			public void SetPickedII(IItemIcon pickedII){
				this.thisPickedUIE = pickedII;
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
			public void CheckAndActivateHoverPads(){
				foreach(IIconGroup ig in GetAllRelevantIGs(this.GetPickedII())){
					ig.ActivateHoverPads();
				}
			}
			public void DeactivateHoverPads(){
				foreach(IIconGroup ig in this.GetAllRelevantIGs(this.GetPickedII()))
					ig.DeactivateHoverPads();
			}
			public abstract void EvaluateHoverability();
			public abstract void ResetHoverability();
			public abstract void HoverInitialPickUpReceiver();
			public virtual void ClearTAFields(){
				this.ClearPickedUIE();
				ClearHoverFields();
			}
			protected abstract void ClearHoverFields();
			public void EvaluatePickability(){
				foreach(IIconGroup ig in GetAllRelevantIGs(null)){
					ig.EvaluateAllIIsPickability();
				}
			}
			public abstract List<IIconGroup> GetAllRelevantIGs(IItemIcon pickedII);
			public abstract IItemIcon CreateItemIcon(IUIItem item);
			public abstract void ExecuteTransaction();
		/*  */
	}
}

