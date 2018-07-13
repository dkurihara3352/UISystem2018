using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem.PickUpUISystem{
	public interface IEquipTool: IPlayerCharacterConfigurationTool{
		void ResetMode();
		void TrySwitchItemMode(IItemTemplate itemTemp);
		void TrySwitchItemFilter(IItemTemplate itemTemp);
	}
	public class EquipTool: AbsUITool, IEquipTool{
		public EquipTool(IUIManager uim, IEquippableIITAManager eqpIITAM): base(uim, eqpIITAM){
			thisModeEngine = new EquipToolItemModeEngine();
			thisAllItemMode = new EquipToolAllItemMode();
			thisBowMode = new EquipToolBowMode();
			thisWearMode = new EquipToolWearMode();
			thisCGMode = new EquipToolCarriedGearMode();

			thisFilterEngine = new EquipToolItemFilterEngine();
			thisNoneFilter = new EquipToolNoneFilter();
			thisBowFilter = new EquipToolBowFilter();
			thisWearFilter = new EquipToolWearFilter();
			thisCGFilter = new EquipToolCarriedGearFilter();

			eqpIITAM.SetEqpTool(this);
		}
		public void ResetMode(){}
		/* mode switch */
			public void TrySwitchItemMode(IItemTemplate itemTemp){
				IEquipToolItemMode modeToSwitch = GetModeToSwitch(itemTemp);
				thisModeEngine.TrySwitchState(modeToSwitch);
			}
			IEquipToolItemMode GetModeToSwitch(IItemTemplate itemTemp){
				if(itemTemp is IBowTemplate)
					return thisBowMode;
				else if( itemTemp is IWearTemplate)
					return thisWearMode;
				else/* cg */
					return thisCGMode;
			}
			readonly IEquipToolItemModeEngine thisModeEngine;
			readonly IEquipToolAllItemMode thisAllItemMode;
			readonly IEquipToolBowMode thisBowMode;
			readonly IEquipToolWearMode thisWearMode;
			readonly IEquipToolCarriedGearMode thisCGMode;
		/* filter switch */
			public void TrySwitchItemFilter(IItemTemplate itemTemp){
				IEquipToolItemFilter filterToSwitch = GetFilterToSwitch(itemTemp);
				thisFilterEngine.TrySwitchState(filterToSwitch);
			}
			readonly IEquipToolItemFilterEngine thisFilterEngine;
			readonly IEquipToolNoneFilter thisNoneFilter;
			readonly IEquipToolBowFilter thisBowFilter;
			readonly IEquipToolWearFilter thisWearFilter;
			readonly IEquipToolCarriedGearFilter thisCGFilter;
			IEquipToolItemFilter GetFilterToSwitch(IItemTemplate itemTemp){
				if(itemTemp is IBowTemplate)
					return this.thisBowFilter;
				else if(itemTemp is IWearTemplate)
					return this.thisWearFilter;
				else/* cg */
					return this.thisCGFilter;
			}
	}
}
