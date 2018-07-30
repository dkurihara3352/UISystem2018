using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DKUtility;
using DKUtility.CurveUtility;

namespace UISystem{
	public interface IScrollerElementMotorProcess: IProcess{}
	public abstract class AbsScrollerElementMotorProcess: AbsProcess, IScrollerElementMotorProcess {
		public AbsScrollerElementMotorProcess(IScroller scroller, IUIElement scrollerElement, int dimension, IProcessManager processManager): base(processManager){
			thisScroller = scroller;
			thisDimension = dimension;
			thisScrollerElement = scrollerElement;
		}
		protected readonly IScroller thisScroller;
		protected readonly IUIElement thisScrollerElement;
		protected readonly int thisDimension;
		public override void Run(){
			base.Run();
			thisScroller.SwitchRunningElementMotorProcess(this, thisDimension);
		}
		public override void Stop(){
			base.Stop();
			thisScroller.ClearScrollerElementMotorProcess(this, thisDimension);
		}
		protected void SetScrollerElementLocalPosOnAxis(float newLocalPosOnAxis){
			Vector2 newElementLocalPos = thisScrollerElement.GetLocalPosition();
			newElementLocalPos[thisDimension] = newLocalPosOnAxis;
			thisScrollerElement.SetLocalPosition(newElementLocalPos);
		}
	}
}
