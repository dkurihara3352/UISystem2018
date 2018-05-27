using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IUIManager{
		IProcessFactory GetProcessFactory();
	}
	public class UIManager: IUIManager {
		public IProcessFactory GetProcessFactory(){
			return ThisProcessFacotory();
		}
		IProcessFactory ThisProcessFacotory(){
			return _processFacotory;
		}
		IProcessFactory _processFacotory;
	}
}
