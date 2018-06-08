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
	public interface IEquipToolUIAdaptor: IPickUpContextUIAdaptor{
	}
	public class EquipToolUIAdaptor: AbsPickUpContextUIAdaptor<IEquipToolUIE>, IEquipToolUIAdaptor{
		/* assigned in the insp */
			public IIconPanel eqpItemsPanel;
			public IIconPanel poolItemsPanel;
		/*  */
		IEquipTool eqpTool;
		IEquipToolUIE eqpToolUIE;
		protected override IEquipToolUIE GetPickUpContextUIE(){
			return eqpToolUIE;
		}
		IEquippableIITAManager equipIITAM;
		public override IUIAActivationArg CreateDomainActivationArg(IUIManager uim){
			/*  Instantiate and set up
					IITAM
					Tool
					Factory
			*/
		}
		protected override IEquipToolUIE CreateUIElement(IUIElementFactory factory){
			if(factory is IEquipToolUIEFactory){
				IEquipToolUIEFactory eqpToolFactory = factory as IEquipToolUIEFactory;
				return eqpToolFactory.CreateEquipToolUIE(this);
			}
			else
				throw new System.ArgumentException("factory must be of type IEquipToolUIEFactory");
		}

	}
	public interface IEquipToolUIE: IPickUpContextUIE{}
	public class EquipToolUIE: AbsUIElement, IEquipToolUIE{
		public EquipToolUIE(IUIManager uim, IEquipToolUIAdaptor uia, IUIImage image): base(uim, uia, image){}
		public Vector2 GetPickUpReservePosInWorldSpace(){
		}
	}
	public interface IEquipToolElementUIE: IUIElement{
		void SetEqpTool(IEquipTool tool);
	}
}

