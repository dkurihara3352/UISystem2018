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
			IUIElementConstArg arg = new UIElementConstArg(uim, uia, image);
			EquipToolUIE uie = new EquipToolUIE(arg);
			return uie;
		}
		IUIImage CreateEquipToolUIImage(){
			return null;
		}
		public IEquippableItemIcon CreateEquippableItemIcon(IEquippableItemIconUIA uia, IEquippableUIItem item){
			UIImage image = CreateEquippableItemIconUIImage(item);
			ItemIconPickUpImplementor iiPickUpImplementor = new ItemIconPickUpImplementor(eqpIITAM);
			EqpIITransactionStateEngine eqpIITAStateEngine = new EqpIITransactionStateEngine(eqpIITAM, eqpTool);
			ItemIconEmptinessStateEngine emptinessStateEngine = new ItemIconEmptinessStateEngine();
			IEquippableItemIconConstArg arg = new EquippableItemIconConstArg(uim, uia, image, eqpIITAM, item, eqpIITAStateEngine, iiPickUpImplementor, emptinessStateEngine, eqpTool);
			EquippableItemIcon eqpII = new EquippableItemIcon(arg);
			return eqpII;
		}
		UIImage CreateEquippableItemIconUIImage(IEquippableUIItem item){
			return null;
		}
	}
}

