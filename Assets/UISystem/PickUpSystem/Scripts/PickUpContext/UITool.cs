using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem.PickUpUISystem{
	public interface IUITool{
	}
	public abstract class AbsUITool: IUITool{
		public AbsUITool(IUIManager uim, IItemIconTransactionManager iiTAM){
			thisUIM = uim;
			thisIITAM = iiTAM;
		}
		protected IUIManager thisUIM;
		protected IItemIconTransactionManager thisIITAM;
	}
	public interface IPlayerCharacterConfigurationTool: IUITool{
	}
}

