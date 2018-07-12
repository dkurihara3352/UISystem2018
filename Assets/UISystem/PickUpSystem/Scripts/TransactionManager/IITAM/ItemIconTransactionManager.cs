using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem.PickUpUISystem{
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
		void ClearTAFields();
		void ExecuteTransaction();
	}
	public abstract class AbsItemIconTransactionManager: AbsPickUpManager, IItemIconTransactionManager{
		public AbsItemIconTransactionManager(IItemIconTAManagerStateEngine stateEngine){
			stateEngine.SetIITAM(this);
			thisStateEngine = stateEngine;
		}
		/* tam state switch */
			protected readonly IItemIconTAManagerStateEngine thisStateEngine;
			public void Activate(){
				SetToDefaultState();
			}
			public void SetToPickedState(IItemIcon pickedII){
				thisStateEngine.SetToPickedState(pickedII);
			}
			public void SetToDefaultState(){
				thisStateEngine.SetToDefaultState();
			}
			public bool IsInPickedUpState(){
				return thisStateEngine.IsInPickedUpState();
			}
			public bool IsInDefaultState(){
				return thisStateEngine.IsInDefaultState();
			}
		/*  */
			public void SetPickedII(IItemIcon pickedII){
				thisPickedUIE = pickedII;
			}
			public IItemIcon GetPickedII(){
				IPickableUIE pickedUIE = this.GetPickedUIE();
				if(pickedUIE != null){
					return (IItemIcon)pickedUIE;
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
			public abstract void ExecuteTransaction();
		/*  */
	}
}

