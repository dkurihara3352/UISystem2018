using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IUIElementFactory{
	}
	public interface IEquipToolUIEFactory: IUIElementFactory{
		IEquipToolUIE CreateEquipToolUIE(IEquipToolUIAdaptor uia);
		IEquippableItemIcon CreateEquippableItemIcon(IEquippableItemIconUIA uia ,IEquippableUIItem eqpItem);
	}
	public class EquipToolUIEFactory: IEquipToolUIEFactory{
		public EquipToolUIEFactory(IUIManager uim, IEquipTool eqpTool, IEquippableIITAManager eqpIITAM){
			this.eqpTool = eqpTool;
			this.eqpIITAM = eqpIITAM;
		}
		readonly IUIManager uim;
		readonly IEquipTool eqpTool;
		readonly IEquippableIITAManager eqpIITAM;
		public IEquipToolUIE CreateEquipToolUIE(IEquipToolUIAdaptor uia){
			IUIImage image = CreateEquipToolUIImage();
			EquipToolUIE uie = new EquipToolUIE(uim, uia, image);
			return uie;
		}
		IUIImage CreateEquipToolUIImage(){
		}
		public IEquippableItemIcon CreateEquippableItemIcon(IEquippableItemIconUIA uia, IEquippableUIItem item){
			IUIImage image = CreateEquippableItemIconUIImage(item);
			EquippableItemIcon eqpII = new EquippableItemIcon(uim, uia, image, item, eqpIITAM, eqpTool);
			return eqpII;
		}
		IUIImage CreateEquippableItemIconUIImage(IEquippableUIItem item){

		}
	}
}

