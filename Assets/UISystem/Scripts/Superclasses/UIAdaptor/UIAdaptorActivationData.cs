﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IUIAActivationData{
		IUIManager uim{get;}
		IUIElementFactory factory{get;}
	}
	public abstract class AbsUIAActivationData: IUIAActivationData{
		public AbsUIAActivationData(IUIManager uim, IUIElementFactory factory){
			this._uim = uim;
			this._factory = factory;
		}
		readonly IUIManager _uim;
		public IUIManager uim{get{return _uim;}}
		readonly IUIElementFactory _factory;
		public IUIElementFactory factory{get{return _factory;}}
	}
	public class RootUIAActivationData: AbsUIAActivationData{
		public RootUIAActivationData(IUIManager uim, IUIElementFactory factory): base(uim, factory){}
	}
	public interface IEquipToolActivationData: IUIAActivationData{
		IEquippableIITAManager eqpIITAM{get;}
		IEquipTool eqpTool{get;}
		IEquipToolUIEFactory eqpToolUIEFactory{get;}
	}
	public class EquipToolUIAActivationData: AbsUIAActivationData, IEquipToolActivationData{
		public EquipToolUIAActivationData(IUIManager uim, IEquipToolUIEFactory factory, IEquippableIITAManager eqpIITAM, IEquipTool eqpTool) :base(uim, factory){
			this._eqpIITAM = eqpIITAM;
			this._eqpTool = eqpTool;
		}
		readonly IEquippableIITAManager _eqpIITAM;
		public IEquippableIITAManager eqpIITAM{get{return _eqpIITAM;}}
		readonly IEquipTool _eqpTool;
		public IEquipTool eqpTool{get{return _eqpTool;}}
		public IEquipToolUIEFactory eqpToolUIEFactory{get{return factory as IEquipToolUIEFactory;}}
	}
}
