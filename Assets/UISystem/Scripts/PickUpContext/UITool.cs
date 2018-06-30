using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IUITool{
		IUIElementFactory GetUIElementFactory();
	}
	public abstract class AbsUITool: IUITool{
		public AbsUITool(IUIManager uim, IItemIconTransactionManager iiTAM){
			thisUIM = uim;
			thisIITAM = iiTAM;
		}
		protected IUIManager thisUIM;
		protected IItemIconTransactionManager thisIITAM;
		public abstract IUIElementFactory GetUIElementFactory();
	}
	public interface IPlayerCharacterConfigurationTool: IUITool{
	}
}

