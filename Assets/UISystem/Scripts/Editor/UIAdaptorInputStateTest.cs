using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using NSubstitute;
using UISystem;

public class UIAdaptorInputStateTest{

	[Test][ExpectedException(typeof(System.InvalidOperationException))]
	public void PointerUpInputState_OnPointerUp_WhenCalled_ThrowsException(){
		IUIAdaptorStateEngine engine = Substitute.For<IUIAdaptorStateEngine>();
		TestPointerUpInputState state = new TestPointerUpInputState(engine);
		ICustomEventData eventData = Substitute.For<ICustomEventData>();

		state.OnPointerUp(eventData);
	}
	[Test][ExpectedException(typeof(System.InvalidOperationException))]
	public void PointerDownInputState_OnPointerDown_WhenCalled_ThrowsException(){
		IUIAdaptorStateEngine engine = Substitute.For<IUIAdaptorStateEngine>();
		TestPointerDownInputState state = new TestPointerDownInputState(engine);
		ICustomEventData eventData = Substitute.For<ICustomEventData>();

		state.OnPointerDown(eventData);
	}
	[Test]
	public void TestUIAStateEngine_Construction_WhenCalled_StatesAreAllSet(){
		UIAStateEngineConstArg arg;
		TestUIAStateEngine engine = CreateTestUIAStateEngine(out arg, .2f, .5f);

		Assert.That(engine.StatesAreAllSet(), Is.True);
	}
	[Test]
	public void TestUIAStateEngine_Contruction_WhenCalled_SetToWFFirstTouchState(){
		UIAStateEngineConstArg arg;
		TestUIAStateEngine engine = CreateTestUIAStateEngine(out arg, .2f, .5f);

		Assert.That(engine.IsWaitingForFirstTouch(), Is.True);

	}
	/* Test Support Classes */
		class TestPointerUpInputState: PointerUpInputState{
			public TestPointerUpInputState(IUIAdaptorStateEngine engine) :base(engine){}
			public override void OnEnter(){}
			public override void OnExit(){}
			public override void OnPointerDown(ICustomEventData eventData){}
		}
		class TestPointerDownInputState: PointerDownInputState{
			public TestPointerDownInputState(IUIAdaptorStateEngine engine) :base(engine){}
			public override void OnEnter(){}
			public override void OnExit(){}
			public override void OnPointerUp(ICustomEventData eventData){}
		}
		class UIAStateEngineConstArg{
			public UIAStateEngineConstArg(IUIAdaptor uia, IUIElement uie, IWaitAndExpireProcess wfTapProcess, IWaitAndExpireProcess wfNextTouchProcess){
				this.uia = uia;
				this.uie = uie;
				this.wfTapProcess = wfTapProcess;
				this.wfNextTouchProcess = wfNextTouchProcess;
			}
			public IUIAdaptor uia;
			public IUIElement uie;
			IWaitAndExpireProcess wfTapProcess;
			IWaitAndExpireProcess wfNextTouchProcess;

		}
		class TestUIAStateEngine: UIAdaptorStateEngine{
			public TestUIAStateEngine(IUIAdaptor uia, IProcessFactory procFac): base(uia, procFac){}
			public IUIAdaptorInputState GetCurState(){
				return this.curState;
			}
			public bool IsWaitingForFirstTouch(){
				return this.curState is WaitingForFirstTouchState;
			}
			public bool IsWaitingForTap(){
				return this.curState is WaitingForTapState;
			}
			public bool IsWaitingForRelease(){
				return this.curState is WaitingForReleaseState;
			}
			public bool IsWaitingForNextTouch(){
				return this.curState is WaitingForNextTouchState;
			}
			public bool StatesAreAllSet(){
				return 
					this.waitingForFirstTouchState != null &&
					this.waitingForTapState != null &&
					this.waitingForReleaseState != null &&
					this.waitingForNextTouchState != null;
			}
		}
		TestUIAStateEngine CreateTestUIAStateEngine(out UIAStateEngineConstArg arg, float tapExpT, float ntExpT){
			/*  ** Note **
				tapExpT and ntExpT MUST be different value in order this to work properly
			*/
			IUIAdaptor mockUIA = Substitute.For<IUIAdaptor>();
				IUIElement mockUIE = Substitute.For<IUIElement>();
				mockUIA.GetUIElement().Returns(mockUIE);
			IProcessFactory mockProcFac = Substitute.For<IProcessFactory>();
				IWaitAndExpireProcess mockWFTapProcess = Substitute.For<IWaitAndExpireProcess>();
				IWaitAndExpireProcess mockWFNextTouchProcess = Substitute.For<IWaitAndExpireProcess>();
				mockProcFac.CreateWaitAndExpireProcess(Arg.Any<IWaitAndExpireProcessState>(), tapExpT).Returns(mockWFTapProcess);
				mockProcFac.CreateWaitAndExpireProcess(Arg.Any<IWaitAndExpireProcessState>(), ntExpT).Returns(mockWFNextTouchProcess);
			arg = new UIAStateEngineConstArg(mockUIA, mockUIE, mockWFTapProcess, mockWFNextTouchProcess);
			TestUIAStateEngine engine = new TestUIAStateEngine(mockUIA, mockProcFac);
			return engine;
		}
	/*  */
}
