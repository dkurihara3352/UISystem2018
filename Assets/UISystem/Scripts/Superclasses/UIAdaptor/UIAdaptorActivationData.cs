using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IUIAActivationData{
		IUIManager uim{get;}
		IUIElementFactory uieFactory{get;}
		IProcessFactory processFactory{get;}
		IPickUpManager pickUpManager{get;}
	}
	public abstract class AbsUIAActivationData: IUIAActivationData{
		public AbsUIAActivationData(IUIManager uim, IUIElementFactory uieFactory, IPickUpManager pum){
			thisUIM = uim;
			thisUIEFactory = uieFactory;
			thisPUM = pum;
		}
		readonly IUIManager thisUIM;
		public IUIManager uim{get{return thisUIM;}}
		readonly IUIElementFactory thisUIEFactory;
		public IUIElementFactory uieFactory{get{return thisUIEFactory;}}
		public IProcessFactory processFactory{get{return thisUIM.GetProcessFactory();}}
		readonly IPickUpManager thisPUM;
		public IPickUpManager pickUpManager{get{return thisPUM;}}
	}
	public class RootUIAActivationData: AbsUIAActivationData{
		public RootUIAActivationData(IUIManager uim, IUIElementFactory uieFactory, IPickUpManager pum): base(uim, uieFactory, pum){}
	}
	public interface IEquipToolActivationData: IUIAActivationData{
		IEquippableIITAManager eqpIITAM{get;}
		IEquipTool eqpTool{get;}
		IEquipToolUIEFactory eqpToolUIEFactory{get;}
	}
	public class EquipToolUIAActivationData: AbsUIAActivationData, IEquipToolActivationData{
		public EquipToolUIAActivationData(IUIManager uim, IEquipToolUIEFactory eqpToolUIEFactory, IEquippableIITAManager eqpIITAM, IEquipTool eqpTool) :base(uim, eqpToolUIEFactory, eqpIITAM){
			thisEqpIITAM = eqpIITAM;
			thisEqpTool = eqpTool;
		}
		readonly IEquippableIITAManager thisEqpIITAM;
		public IEquippableIITAManager eqpIITAM{get{return thisEqpIITAM;}}
		readonly IEquipTool thisEqpTool;
		public IEquipTool eqpTool{get{return thisEqpTool;}}
		public IEquipToolUIEFactory eqpToolUIEFactory{get{return (IEquipToolUIEFactory)uieFactory;}}
	}
}
