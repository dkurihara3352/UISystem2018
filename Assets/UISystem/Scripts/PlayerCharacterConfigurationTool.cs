using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IPlayerCharacterConfigurationTool{

	}
	public interface IEquipTool: IPlayerCharacterConfigurationTool{
		void ResetMode();
		void TrySwitchItemMode(IItemTemplate itemTemp);
		void TrySwitchItemFilter(IItemTemplate itemTemp);
	}
	public interface IPickUpContextUIAdaptor: IUIAdaptor{
		IPickUpContextUIE GetPickUpContextUIE();
	}
	public abstract class AbsPickUpContextUIAdaptor: AbsUIAdaptor, IPickUpContextUIAdaptor{
		public IPickUpContextUIE GetPickUpContextUIE(){
			return this.uiElement as IPickUpContextUIE;
		}
		public IPickUpManager pum;/* assingned in the inspector */
		public override void GetReadyForActivation(IUIManager uim){
			/*  Find all iiTA elements =>
					IPickableUIEs and IPickUpReceivers
					Those that resides in the hierarchy betweeen this and the closest child pickUpContextUIA
					foreach iiTAElement
						iiTAElement.SetPUM(this.pum)
			*/
			List<IUIAdaptor> uias = GetPickUpDomainUIAs();
			foreach(IUIAdaptor uia in uias){
				IUIElement uie = uia.GetUIElement();
				if(uie is IPickUpTransactionElement){
					((IPickUpTransactionElement)uie).SetPickUpManager(pum);
				}
			}
			base.GetReadyForActivation(uim);
		}
		List<IUIAdaptor> GetPickUpDomainUIAs(){
			/* Get all the children, from self down to next offspring with IPickUpContextUIA, excluding both */
			List<IUIAdaptor> result = new List<IUIAdaptor>();
			List<IPickUpContextUIAdaptor> allNextOffspringPUCUIAs = FindAllNextOffspringsWithComponent<IPickUpContextUIAdaptor>(this.transform);
			List<IUIAdaptor> allOffspringUIAs = GetAllOffspringUIAs();
			if(allNextOffspringPUCUIAs.Count == 0)
				result = allOffspringUIAs;
			else{
				result = FindAllOffspringUIAsDownToPUCUIAs(allOffspringUIAs, allNextOffspringPUCUIAs);
			}
			return result;
		}
		List<IUIAdaptor> FindAllOffspringUIAsDownToPUCUIAs(List<IUIAdaptor> allOffspringUIAs, List<IPickUpContextUIAdaptor> allNextOffspringPUCUIAs){
			List<IUIAdaptor> result = new List<IUIAdaptor>();
			foreach(IUIAdaptor uia in allOffspringUIAs){
				foreach(IPickUpContextUIAdaptor pucUIA in allNextOffspringPUCUIAs){
					if(uia == pucUIA || pucUIA.GetAllOffspringUIAs().Contains(uia))
						continue;
					else
						result.Add(uia);
				}
			}
			return result;
		}
	}
}

