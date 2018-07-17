using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DKUtility;

namespace UISystem{
	public interface IScrollerAxis: ISwitchableState{
		void DragImple(Vector2 position, Vector2 deltaP);
	}
	public abstract class AbsScrollerAxis: IScrollerAxis{
	}
	public interface IScrollerAxisHorizontal: IScrollerAxis{}
	public class ScrollerAxisHorizontal: AbsScrollerAxis, IScrollerAxisHorizontal{}
}
