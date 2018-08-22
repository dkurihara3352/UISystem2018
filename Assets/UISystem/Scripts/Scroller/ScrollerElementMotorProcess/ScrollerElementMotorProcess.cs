using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DKUtility;
using DKUtility.CurveUtility;

namespace UISystem{
	public interface IScrollerElementMotorProcess: IProcess{
		int GetDimension();
	}
	public abstract class AbsScrollerElementMotorProcess: AbsProcess, IScrollerElementMotorProcess {
		public AbsScrollerElementMotorProcess(IScrollerElementMotorProcessConstArg arg): base(arg){
			thisScroller = arg.scroller;
			thisScrollerElement = arg.scrollerElement;
			thisDimension = arg.dimension;
		}
		protected readonly IScroller thisScroller;
		protected readonly IUIElement thisScrollerElement;
		protected readonly int thisDimension;
		public int GetDimension(){return thisDimension;}
		protected override void RunImple(){
			thisScroller.SwitchRunningElementMotorProcess(this, thisDimension);
		}
		protected override void ExpireImple(){
			thisScroller.SwitchRunningElementMotorProcess(null, thisDimension);
		}
	}
	public interface IScrollerElementMotorProcessConstArg: IProcessConstArg{
		IScroller scroller{get;}
		IUIElement scrollerElement{get;}
		int dimension{get;}
	}
	public class ScrollerElementMotorProcessConstArg: ProcessConstArg, IScrollerElementMotorProcessConstArg{
		public ScrollerElementMotorProcessConstArg(
			IProcessManager processManager,
			IScroller scroller,
			IUIElement scrollerElement,
			int dimension
		): base(
			processManager
		){
			thisScroller = scroller;
			thisDimension = dimension;
			thisScrollerElement = scrollerElement;
		}
		readonly IScroller thisScroller;
		public IScroller scroller{get{return thisScroller;}}
		readonly IUIElement thisScrollerElement;
		public IUIElement scrollerElement{get{return thisScrollerElement;}}
		readonly int thisDimension;
		public int dimension{get{return thisDimension;}}
	}
}
