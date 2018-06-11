using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IEquipToolElementUIE: IUIElement{
	}
	// public abstract class AbsEquipToolElementUIE: AbsUIElement, IEquipToolElementUIE{
	// 	public AbsEquipToolElementUIE(IUIManager uim, IEquipToolElementUIA uia, IUIImage image, IEquipToolUIAActivationData activationData): base(uim, uia, image){
	// 		this.equipTool = activationData.eqpTool;
	// 		this.equipIITAM = activationData.eqpIITAM;
	// 	}
	// 	readonly protected IEquipTool equipTool;
	// 	readonly protected IEquippableIITAManager equipIITAM;
	// }
}
