using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IEmptinessStateHandler{
		bool IsEmpty();
		void DisemptifyInstantly();
	}
	public interface IEmptinessStateEngine: IEmptinessStateHandler{}
	public interface IEquipStateHandler{
		void Equip();
		void Unequip();
		bool IsEquipped();
	}
}
