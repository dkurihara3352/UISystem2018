using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IEquipTool: IPlayerCharacterConfigurationTool{
		void ResetMode();
		void TrySwitchItemMode(IItemTemplate itemTemp);
		void TrySwitchItemFilter(IItemTemplate itemTemp);
	}
	public class EquipTool: AbsUITool, IEquipTool{
		public EquipTool(IUIManager uim, IEquippableIITAManager eqpIITAM): base(uim, eqpIITAM){
			this.modeEngine = new EquipToolItemModeEngine();
			this.allItemMode = new EquipToolAllItemMode();
			this.bowMode = new EquipToolBowMode();
			this.wearMode = new EquipToolWearMode();
			this.cgMode = new EquipToolCarriedGearMode();

			this.filterEngine = new EquipToolItemFilterEngine();
			this.noneFilter = new EquipToolNoneFilter();
			this.bowFilter = new EquipToolBowFilter();
			this.wearFilter = new EquipToolWearFilter();
			this.cgFilter = new EquipToolCarriedGearFilter();

			thisEqpToolUIEFactory = new EquipToolUIEFactory(thisUIM, this, (IEquippableIITAManager)thisIITAM);
			eqpIITAM.SetEqpTool(this);
		}
		public void ResetMode(){}
		/* mode switch */
			public void TrySwitchItemMode(IItemTemplate itemTemp){
				IEquipToolItemMode modeToSwitch = GetModeToSwitch(itemTemp);
				this.modeEngine.TrySwitchState(modeToSwitch);
			}
			IEquipToolItemMode GetModeToSwitch(IItemTemplate itemTemp){
				if(itemTemp is IBowTemplate)
					return bowMode;
				else if( itemTemp is IWearTemplate)
					return wearMode;
				else/* cg */
					return cgMode;
			}
			readonly IEquipToolItemModeEngine modeEngine;
			readonly IEquipToolAllItemMode allItemMode;
			readonly IEquipToolBowMode bowMode;
			readonly IEquipToolWearMode wearMode;
			readonly IEquipToolCarriedGearMode cgMode;
		/* filter switch */
			public void TrySwitchItemFilter(IItemTemplate itemTemp){
				IEquipToolItemFilter filterToSwitch = GetFilterToSwitch(itemTemp);
				this.filterEngine.TrySwitchState(filterToSwitch);
			}
			readonly IEquipToolItemFilterEngine filterEngine;
			readonly IEquipToolNoneFilter noneFilter;
			readonly IEquipToolBowFilter bowFilter;
			readonly IEquipToolWearFilter wearFilter;
			readonly IEquipToolCarriedGearFilter cgFilter;
			IEquipToolItemFilter GetFilterToSwitch(IItemTemplate itemTemp){
				if(itemTemp is IBowTemplate)
					return this.bowFilter;
				else if(itemTemp is IWearTemplate)
					return this.wearFilter;
				else/* cg */
					return this.cgFilter;
			}
		/*  */
		readonly IEquipToolUIEFactory thisEqpToolUIEFactory;
		public override IUIElementFactory GetUIElementFactory(){
			return thisEqpToolUIEFactory;
		}
	}
}
