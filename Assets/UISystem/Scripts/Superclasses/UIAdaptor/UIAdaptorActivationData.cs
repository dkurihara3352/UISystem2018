using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IUIAActivationData{
		IUIManager uim{get;}
		IUIElementFactory uieFactory{get;}
		IProcessFactory processFactory{get;}
		IPickUpManager pickUpManager{get;}
		IUITool tool{get;}
	}
	public abstract class AbsUIAActivationData: IUIAActivationData{
		public AbsUIAActivationData(IUIManager uim, IPickUpManager pum, IUITool tool){
			thisUIM = uim;
			thisPUM = pum;
			thisTool = tool;
		}
		readonly IUIManager thisUIM;
		public IUIManager uim{get{return thisUIM;}}
		public IUIElementFactory uieFactory{get{return tool.GetUIElementFactory();}}
		public IProcessFactory processFactory{get{return thisUIM.GetProcessFactory();}}
		readonly IPickUpManager thisPUM;
		public IPickUpManager pickUpManager{get{return thisPUM;}}
		readonly IUITool thisTool;
		public IUITool tool{get{return thisTool;}}
	}
	public class RootUIAActivationData: AbsUIAActivationData{
		public RootUIAActivationData(IUIManager uim, IPickUpManager pum, IUITool tool): base(uim, pum, tool){}
	}
	public interface IEquipToolActivationData: IUIAActivationData{
		IEquippableIITAManager eqpIITAM{get;}
		IEquipTool eqpTool{get;}
		IEquipToolUIEFactory eqpToolUIEFactory{get;}
	}
	public class EquipToolUIAActivationData: AbsUIAActivationData, IEquipToolActivationData{
		public EquipToolUIAActivationData(IUIManager uim, IEquippableIITAManager eqpIITAM, IEquipTool eqpTool) :base(uim, eqpIITAM, eqpTool){
			thisEqpIITAM = eqpIITAM;
		}
		readonly IEquippableIITAManager thisEqpIITAM;
		public IEquippableIITAManager eqpIITAM{get{return thisEqpIITAM;}}
		public IEquipTool eqpTool{get{return (IEquipTool)tool;}}
		public IEquipToolUIEFactory eqpToolUIEFactory{get{return (IEquipToolUIEFactory)eqpTool.GetUIElementFactory();}}
	}
}
