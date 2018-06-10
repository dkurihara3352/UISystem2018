using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IEmptinessStateHandler{
		bool IsEmpty();
		void DisemptifyInstantly(IUIItem item);
		void EmptifyInstantly();
		void Disemptify(IUIItem item);
		void Emptify();
	}
	public interface IEmptinessStateEngine: IEmptinessStateHandler{}
}
