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
			thisUIM = uim;
			thisEqpTool = eqpTool;
			thisEqpIITAM = eqpIITAM;
		}
		readonly IUIManager thisUIM;
		readonly IEquipTool thisEqpTool;
		readonly IEquippableIITAManager thisEqpIITAM;
		public IEquipToolUIE CreateEquipToolUIE(IEquipToolUIAdaptor uia){
			IUIImage image = CreateEquipToolUIImage();
			IUIElementConstArg arg = new UIElementConstArg(thisUIM, uia, image);
			EquipToolUIE uie = new EquipToolUIE(arg);
			return uie;
		}
		IUIImage CreateEquipToolUIImage(){
			return null;
		}
		public IEquippableItemIcon CreateEquippableItemIcon(IEquippableItemIconUIA uia, IEquippableUIItem item){
			UIImage image = CreateEquippableItemIconUIImage(item);
			ItemIconPickUpImplementor iiPickUpImplementor = new ItemIconPickUpImplementor(thisEqpIITAM);
			EqpIITransactionStateEngine eqpIITAStateEngine = new EqpIITransactionStateEngine(thisEqpIITAM, thisEqpTool);
			ItemIconEmptinessStateEngine emptinessStateEngine = new ItemIconEmptinessStateEngine();
			IEquippableItemIconConstArg arg = new EquippableItemIconConstArg(thisUIM, uia, image, thisEqpIITAM, item, eqpIITAStateEngine, iiPickUpImplementor, emptinessStateEngine, thisEqpTool);
			EquippableItemIcon eqpII = new EquippableItemIcon(arg);
			return eqpII;
		}
		UIImage CreateEquippableItemIconUIImage(IEquippableUIItem item){
			return null;
		}
	}
}

