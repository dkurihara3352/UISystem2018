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
		/*  uia for tools and widgets that handles pickup
		*/
		public abstract IPickUpContextUIE GetPickUpContextUIE();
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
	}
	public interface IEqpToolUIAdaptor: IPickUpContextUIAdaptor{
	}
	public class EqpToolUIAdaptor: AbsPickUpContextUIAdaptor, IEqpToolUIAdaptor{
		/* assigned in the insp */
			public IIconPanel eqpItemsPanel;
			public IIconPanel poolItemsPanel;
		/*  */
		readonly IEqpToolUIE eqpToolUIE;
		public override IUIElement GetUIElement(){
			return eqpToolUIE;
		}
		public override IPickUpContextUIE GetPickUpContextUIE(){
			return eqpToolUIE;
		}
		readonly IEquipTool equipTool;
		IEquippableIITAManager _equipIITAM;
		IEquippableIITAManager equipIITAM{
			get{return _equipIITAM;}
		}
		IEquippableIITAManager CreateEquipIITAManager(){
			return new EquippableIITAManager(eqpItemsPanel, poolItemsPanel);
		}
		// IEquippableIITAManager
		protected override void CreateAndSetUIE(IUIManager uim){
			IUIImage image = CreateUIImage();
			IEqpToolUIE uie = new EqpToolUIE(uim, this, image);
		}
		protected override IUIImage CreateUIImage(){}
		public override void GetReadyForActivation(IUIManager uim){
			this._equipIITAM = CreateEquipIITAManager();
			List<IEquipToolElementUIA> allEqpToolEleUIAsInThisDomain = GetAllEquipToolElementUIAsInThisDomain();
			foreach(IEquipToolElementUIA eqpEleUIA in allEqpToolEleUIAsInThisDomain){
				eqpEleUIA.SetEquipIITAM(this.equipIITAM);
				eqpEleUIA.SetEquipTool(this.equipTool);
			}
			base.GetReadyForActivation(uim);
		}
		List<IEquipToolElementUIA> GetAllEquipToolElementUIAsInThisDomain(){
			List<IEquipToolElementUIA> result = new List<IEquipToolElementUIA>();
			List<IUIAdaptor> allUIAsInThisDomain = GetPickUpDomainUIAs();
			foreach(IUIAdaptor uia in allUIAsInThisDomain){
				if(uia is IEquipToolElementUIA)
					result.Add((IEquipToolElementUIA)uia);
			}
			return result;
		}
	}
	public interface IEqpToolUIE: IPickUpContextUIE{}
	public class EqpToolUIE: AbsUIElement, IEqpToolUIE{
		public EqpToolUIE(IUIManager uim, IEqpToolUIAdaptor uia, IUIImage image): base(uim, uia, image){}
		public Vector2 GetPickUpReservePosInWorldSpace(){
		}
	}
	public interface IEquipToolElementUIE: IUIElement{
		void SetEqpTool(IEquipTool tool);
	}
}

