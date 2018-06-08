using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IPickUpContextUIAdaptor: IUIAdaptor{
		IUIAActivationArg CreateDomainActivationArg(IUIManager uim);
	}
	public abstract class AbsPickUpContextUIAdaptor<T>: AbsUIAdaptor<T>, IPickUpContextUIAdaptor where T: IPickUpContextUIE{
		/*  uia for tools and widgets that handles pickup
		*/
		protected abstract T GetPickUpContextUIE();
		protected List<IUIAdaptor> GetPickUpDomainUIAs(){
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
		protected List<IUIAdaptor> FindAllOffspringUIAsDownToPUCUIAs(List<IUIAdaptor> allOffspringUIAs, List<IPickUpContextUIAdaptor> allNextOffspringPUCUIAs){
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
		public abstract IUIAActivationArg CreateDomainActivationArg(IUIManager uim);
	}
}
