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
	public void PointerUpInputState_OnDrag_WhenCalled_ThrowsException(){
		IUIAdaptorStateEngine engine = Substitute.For<IUIAdaptorStateEngine>();
		TestPointerUpInputState state = new TestPointerUpInputState(engine);

		Assert.Throws(typeof(System.InvalidOperationException), () => state.OnDrag(Vector2.zero, Vector2.zero));
	}
	[Test]
	public void TestUIAStateEngine_Construction_WhenCalled_StatesAreAllSet(){
		TestUIAStateEngine engine = CreateTestUIAStateEngine();

		Assert.That(engine.StatesAreAllSet(), Is.True);
	}
	[Test]
	public void TestUIAStateEngine_Contruction_WhenCalled_SetToWFFirstTouchState(){
		TestUIAStateEngine engine = CreateTestUIAStateEngine();

		Assert.That(engine.IsWaitingForFirstTouch(), Is.True);
	}
	[Test]
	public void TestUIAStateEngine_Contruction_WhenCalled_TouchCountIsZero(){
		TestUIAStateEngine engine = CreateTestUIAStateEngine();

		Assert.That(engine.GetTouchCount(), Is.EqualTo(0));
	}
	[Test]
	public void TestUIAStateEngine_OnPointerDown_WhileWFFirstTouch_BecomesWaitForTapState(){
		TestUIAStateEngine engine = CreateTestUIAStateEngine();
		ICustomEventData eventData = Substitute.For<ICustomEventData>();
		
		engine.OnPointerDown(eventData);

		Assert.That(engine.IsWaitingForTap(), Is.True);
	}
	[Test]
	public void TestUIAStateEngine_OnPointerDown_WhileWFFirstTouch_RunsWFTapProcess(){
		UIAStateEngineConstArg arg;
		TestUIAStateEngine engine = CreateTestUIAStateEngine(out arg);
		ICustomEventData eventData = Substitute.For<ICustomEventData>();
		
		engine.OnPointerDown(eventData);

		arg.wfTapProcess.Received(1).Run();
	}
	[Test]
	public void TestUIAStateEngine_OnPointerDown_WhileWFFirstTouch_SetsTouchCountOne(){
		UIAStateEngineConstArg arg;
		TestUIAStateEngine engine = CreateTestUIAStateEngine(out arg);
		ICustomEventData eventData = Substitute.For<ICustomEventData>();
		
		engine.OnPointerDown(eventData);

		Assert.That(engine.GetTouchCount(), Is.EqualTo(1));
	}
	[Test]
	public void TestUIAStateEngine_OnPointerDown_WhileWFFirstTouch_CallsUIETouchArgOne(){
		UIAStateEngineConstArg arg;
		TestUIAStateEngine engine = CreateTestUIAStateEngine(out arg);
		ICustomEventData eventData = Substitute.For<ICustomEventData>();
		IUIElement uie = arg.uie;
		engine.OnPointerDown(eventData);

		uie.Received().OnTouch(1);
	}
	[Test]
	public void TestUIAStateEngine_OnPointerUp_WhileWFTapState_BecomesWFNextTouchState(){
		TestUIAStateEngine engine = CreateTestUIAStateEngine();
		ICustomEventData eventData = Substitute.For<ICustomEventData>();
		engine.OnPointerDown(eventData);
		Assert.That(engine.IsWaitingForTap(), Is.True);
		
		engine.OnPointerUp(eventData);

		Assert.That(engine.IsWaitingForNextTouch(), Is.True);
	}
	[Test]
	public void TestUIAStateEngine_OnPointerUp_WhileWFTapState_RunsWFNTProcess(){
		UIAStateEngineConstArg arg;
		TestUIAStateEngine engine = CreateTestUIAStateEngine(out arg);
		ICustomEventData eventData = Substitute.For<ICustomEventData>();
		IWaitAndExpireProcess wfNTProcess = arg.wfNextTouchProcess;
		engine.OnPointerDown(eventData);
		Assert.That(engine.IsWaitingForTap(), Is.True);
		
		engine.OnPointerUp(eventData);

		wfNTProcess.Received(1).Run();
	}
	[Test]
	public void TestUIAStateEngine_OnPointerUp_WhileWFTapState_UIECalledOnTapArgOne(){
		UIAStateEngineConstArg arg;
		TestUIAStateEngine engine = CreateTestUIAStateEngine(out arg);
		ICustomEventData eventData = Substitute.For<ICustomEventData>();
		IUIElement uie = arg.uie;
		engine.OnPointerDown(eventData);
		Assert.That(engine.IsWaitingForTap(), Is.True);
		
		engine.OnPointerUp(eventData);

		uie.Received(1).OnTap(1);
	}
	[Test]
	public void TestUIAStateEngine_OnPointerUp_WhileWFTapState_WFTapProcessStops(){
		UIAStateEngineConstArg arg;
		TestUIAStateEngine engine = CreateTestUIAStateEngine(out arg);
		ICustomEventData eventData = Substitute.For<ICustomEventData>();
		IWaitAndExpireProcess wfTapProcess = arg.wfTapProcess;
		engine.OnPointerDown(eventData);
		Assert.That(engine.IsWaitingForTap(), Is.True);
		
		engine.OnPointerUp(eventData);

		wfTapProcess.Received(1).Stop();
	}
	[Test]
	public void TestUIAStateEngine_ProcessExpires_WhileWFTapState_BecomesWFReleaseState(){
		UIAStateEngineConstArg arg;
		TestUIAStateEngine engine = CreateTestUIAStateEngine(out arg);
		ICustomEventData eventData = Substitute.For<ICustomEventData>();
		IWaitAndExpireProcess wfTapProcess = arg.wfTapProcess;
		engine.OnPointerDown(eventData);
		Assert.That(engine.IsWaitingForTap(), Is.True);
		
		wfTapProcess.Expire();

		Assert.That(engine.IsWaitingForRelease(), Is.True);
	}
	[Test]
	public void TestUIAStateEngine_ProcessExpires_WhileWFTapState_RunsWFReleaseProcess(){
		UIAStateEngineConstArg arg;
		TestUIAStateEngine engine = CreateTestUIAStateEngine(out arg);
		ICustomEventData eventData = Substitute.For<ICustomEventData>();
		IWaitAndExpireProcess wfTapProcess = arg.wfTapProcess;
		IWaitAndExpireProcess wfReleaseProcess = arg.wfReleaseProcess;
		engine.OnPointerDown(eventData);
		Assert.That(engine.IsWaitingForTap(), Is.True);
		
		wfTapProcess.Expire();

		wfReleaseProcess.Received(1).Run();
	}
	[Test]
	public void TestUIAStateEngine_ProcessExpires_WhileWFTapState_UIECalledOnDelayedTouch(){
		UIAStateEngineConstArg arg;
		TestUIAStateEngine engine = CreateTestUIAStateEngine(out arg);
		IUIElement uie = arg.uie;
		ICustomEventData eventData = Substitute.For<ICustomEventData>();
		IWaitAndExpireProcess wfTapProcess = arg.wfTapProcess;
		engine.OnPointerDown(eventData);
		Assert.That(engine.IsWaitingForTap(), Is.True);

		wfTapProcess.Expire();

		uie.Received(1).OnDelayedTouch();
	}
	[Test]
	public void TestUIAStateEngine_OnPointerExit_WhileWFTapState_BecomesWFRelease(){//and run process, reset touchC
		UIAStateEngineConstArg arg;
		TestUIAStateEngine engine = CreateTestUIAStateEngine(out arg);
		IUIElement uie = arg.uie;
		ICustomEventData eventData = Substitute.For<ICustomEventData>();
		engine.OnPointerDown(eventData);
		Assert.That(engine.IsWaitingForTap(), Is.True);

		engine.OnPointerExit(eventData);

		Assert.That(engine.IsWaitingForRelease(), Is.True);
	}
	[Test]
	public void TestUIAStateEngine_OnPointerExit_WhileWFTapState_ResetTouchCount(){
		UIAStateEngineConstArg arg;
		TestUIAStateEngine engine = CreateTestUIAStateEngine(out arg);
		IUIElement uie = arg.uie;
		ICustomEventData eventData = Substitute.For<ICustomEventData>();
		engine.OnPointerDown(eventData);
		Assert.That(engine.IsWaitingForTap(), Is.True);

		engine.OnPointerExit(eventData);

		Assert.That(engine.GetTouchCount(), Is.EqualTo(0));
	}
	[Test]
	public void TestUIAStateEngine_OnPointerExit_WhileWFTapState_RunsWFReleaseProcess(){
		UIAStateEngineConstArg arg;
		TestUIAStateEngine engine = CreateTestUIAStateEngine(out arg);
		IUIElement uie = arg.uie;
		ICustomEventData eventData = Substitute.For<ICustomEventData>();
		IWaitAndExpireProcess wfReleaseProcess = arg.wfReleaseProcess;
		engine.OnPointerDown(eventData);
		Assert.That(engine.IsWaitingForTap(), Is.True);

		engine.OnPointerExit(eventData);

		wfReleaseProcess.Received(1).Run();
	}
	[Test]
	public void TestUIAStateEngine_OnPointerUp_WhielInWFReleaseState_BecomesWaitingForNextTouch(){
		UIAStateEngineConstArg arg;
		TestUIAStateEngine engine = CreateTestUIAStateEngine(out arg);
		ICustomEventData eventData = Substitute.For<ICustomEventData>();
		IWaitAndExpireProcess wfTapProcess = arg.wfTapProcess;
		engine.OnPointerDown(eventData);
		Assert.That(engine.IsWaitingForTap(), Is.True);
		wfTapProcess.Expire();
		Assert.That(engine.IsWaitingForRelease(), Is.True);

		engine.OnPointerUp(eventData);

		Assert.That(engine.IsWaitingForNextTouch(), Is.True);
	}
	[Test]
	public void TestUIAStateEngine_OnPointerUp_WhielInWFReleaseState_StopsWFReleaseProcess(){
		UIAStateEngineConstArg arg;
		TestUIAStateEngine engine = CreateTestUIAStateEngine(out arg);
		ICustomEventData eventData = Substitute.For<ICustomEventData>();
		IWaitAndExpireProcess wfTapProcess = arg.wfTapProcess;
		IWaitAndExpireProcess wfReleaseProcess = arg.wfReleaseProcess;
		engine.OnPointerDown(eventData);
		wfTapProcess.Expire();
		Assert.That(engine.IsWaitingForRelease(), Is.True);

		engine.OnPointerUp(eventData);

		wfReleaseProcess.Received(1).Stop();
	}
	[Test]
	public void TestUIAStateEngine_OnPointerUP_WhileWFRelease_CallsUIEOnRelease(){
		UIAStateEngineConstArg arg;
		TestUIAStateEngine engine = CreateTestUIAStateEngine(out arg);
		IUIElement uie = arg.uie;
		IWaitAndExpireProcess wfTapProcess = arg.wfTapProcess;
		ICustomEventData eventData = Substitute.For<ICustomEventData>();
		engine.OnPointerDown(eventData);
		wfTapProcess.Expire();
		Assert.That(engine.IsWaitingForRelease(), Is.True);

		engine.OnPointerUp(eventData);

		uie.Received(1).OnRelease();
	}
	[Test]
	public void TestUIAStateEngine_OnPointerDown_WhileInWFNextTouchState_BecomesWFTapState(){
		UIAStateEngineConstArg arg;
		TestUIAStateEngine engine = CreateTestUIAStateEngine(out arg);
		ICustomEventData eventData = Substitute.For<ICustomEventData>();
		IWaitAndExpireProcess wfTapProcess = arg.wfTapProcess;
		engine.OnPointerDown(eventData);
		Assert.That(engine.IsWaitingForTap(), Is.True);
		engine.OnPointerUp(eventData);
		Assert.That(engine.IsWaitingForNextTouch(), Is.True);

		engine.OnPointerDown(eventData);

		Assert.That(engine.IsWaitingForTap(), Is.True);
	}
	[Test]
	public void TestUIAStateEngine_OnPointerDown_WhileInWFNextTouchState_RunsWFTapProcess(){
		UIAStateEngineConstArg arg;
		TestUIAStateEngine engine = CreateTestUIAStateEngine(out arg);
		ICustomEventData eventData = Substitute.For<ICustomEventData>();
		IWaitAndExpireProcess wfTapProcess = arg.wfTapProcess;
		engine.OnPointerDown(eventData);
		Assert.That(engine.IsWaitingForTap(), Is.True);
		wfTapProcess.Received(1).Run();
		engine.OnPointerUp(eventData);
		Assert.That(engine.IsWaitingForNextTouch(), Is.True);

		engine.OnPointerDown(eventData);

		wfTapProcess.Received(2).Run();
	}
	[Test]
	public void TestUIAStateEngine_OnPointerDown_WhileWFNextTouch_IncrementTouchCount([NUnit.Framework.Values(100)]int count){
		UIAStateEngineConstArg arg;
		TestUIAStateEngine engine = CreateTestUIAStateEngine(out arg);
		ICustomEventData eventData = Substitute.For<ICustomEventData>();

		for(int i =0; i < count; i++){
			engine.OnPointerDown(eventData);
			Assert.That(engine.GetTouchCount(), Is.EqualTo(i + 1));
			engine.OnPointerUp(eventData);
		}
	}
	[Test]
	public void TestUIAStateEngine_OnPointerDown_WhileWFNextTouch_TouchCountX_CallsUIEOnTouchWithArgX([Values(100)]int count){
		UIAStateEngineConstArg arg;
		TestUIAStateEngine engine = CreateTestUIAStateEngine(out arg);
		ICustomEventData eventData = Substitute.For<ICustomEventData>();
		IUIElement uie = arg.uie;

		for(int i = 0; i < count; i++){
			engine.OnPointerDown(eventData);
			uie.Received(1).OnTouch(i + 1);
			engine.OnPointerUp(eventData);
		}
	}
	[Test]
	public void TestUIAStateEngine_OnPointerUp_WhileWFTap_TouchCountX_EligibleForTap_CallsUIEOnTapWithArgX([Values(100)]int count){
		UIAStateEngineConstArg arg;
		TestUIAStateEngine engine = CreateTestUIAStateEngine(out arg);
		ICustomEventData eventData = Substitute.For<ICustomEventData>();
		IUIElement uie = arg.uie;

		for(int i = 0; i < count; i++){
			engine.OnPointerDown(eventData);
			engine.OnPointerUp(eventData);
			uie.Received(1).OnTap(i + 1);
		}
	}
	[Test]
	public void TestUIAStateEngine_ProcessExpires_WhileWFNextTouch_TouchCountX_ResetTouchCountToZero([Values(1, 10, 100)]int count){
		UIAStateEngineConstArg arg;
		TestUIAStateEngine engine = CreateTestUIAStateEngine(out arg);
		ICustomEventData eventData = Substitute.For<ICustomEventData>();
		IUIElement uie = arg.uie;
		IWaitAndExpireProcess wfNTProcess = arg.wfNextTouchProcess;

		for(int i = 0; i < count; i++){
			engine.OnPointerDown(eventData);
			engine.OnPointerUp(eventData);
		}
		wfNTProcess.Expire();

		Assert.That(engine.GetTouchCount(), Is.EqualTo(0));
	}
	[Test]
	public void TestUIAStateEngine_ProcessExpires_WhileWFNextTouch_CallsUIEOnDelayedRelease(){
		UIAStateEngineConstArg arg;
		TestUIAStateEngine engine = CreateTestUIAStateEngine(out arg);
		ICustomEventData eventData = Substitute.For<ICustomEventData>();
		IUIElement uie = arg.uie;
		IWaitAndExpireProcess wfNTProcess = arg.wfNextTouchProcess;
		engine.OnPointerDown(eventData);
		engine.OnPointerUp(eventData);
		Assert.That(engine.IsWaitingForNextTouch(), Is.True);
		wfNTProcess.Expire();

		uie.Received(1).OnDelayedRelease();
	}
	[Test][TestCaseSource(typeof(OnDragTestCase), "case1")]
	public void TestUIAStateEngine_OnDrag_WhilePointerDownState_OverThreshold_CallsUIEOnDrag(float vector2X, bool overThreshold){
		UIAStateEngineConstArg arg;
		TestUIAStateEngine engine = CreateTestUIAStateEngine(out arg);
		IUIElement uie = arg.uie;
		Vector2 dragDeltaP = new Vector2(vector2X, 0f);
		engine.OnPointerDown(Substitute.For<ICustomEventData>());
		Assert.That(engine.GetCurState(), Is.InstanceOf(typeof(PointerDownInputState)));

		engine.OnDrag(Vector2.zero, dragDeltaP);
		
		if(dragDeltaP.sqrMagnitude >= engine.GetDragThreshold() * engine.GetDragThreshold()){
			Assert.That(overThreshold, Is.True);
			uie.Received(1).OnDrag(Vector2.zero, dragDeltaP);
		}else{
			Assert.That(overThreshold, Is.False);
			uie.DidNotReceive().OnDrag(Vector2.zero, dragDeltaP);
		}
	}
	class OnDragTestCase{
		static object[] case1 = {
			new object[]{0f, false},
			new object[]{4.5f, false},
			new object[]{4.999999f, false},/* dunno the boundary */
			new object[]{5f - float.Epsilon, true},/* this is bit weird, minor though */
			new object[]{5f, true},
			new object[]{10f, true},
		};
		static object[] case2 = {
			new object[]{1 ,5f - float.Epsilon, true},
			new object[]{10 ,5f, true},
			new object[]{100 ,10f, true},
			new object[]{4 ,1f, false},
			new object[]{4 ,4f, false},
			new object[]{4 ,4.999f, false},
		};
	}
	[Test][TestCaseSource(typeof(OnDragTestCase), "case1")]
	public void TestUIAStateEngine_OnDrag_WhileWFTapState_OverThreshold_DoesNotBecomeWFRelease(float vector2X, bool overThreshold){
		UIAStateEngineConstArg arg;
		TestUIAStateEngine engine = CreateTestUIAStateEngine(out arg);
		IUIElement uie = arg.uie;
		Vector2 dragDeltaP = new Vector2(vector2X, 0f);
		engine.OnPointerDown(Substitute.For<ICustomEventData>());
		Assert.That(engine.IsWaitingForTap(), Is.True);
		
		engine.OnDrag(Vector2.zero, dragDeltaP);
		
		Assert.That(engine.IsWaitingForTap(), Is.True);
	}
	/* Test Support Classes */
		[Test]
		public void TestUIAStateEngine_TestMethods_WorksProperly(){
			UIAStateEngineConstArg arg;
			TestUIAStateEngine engine = CreateTestUIAStateEngine(out arg);

			Assert.That(engine.GetWFFirstTouchState(), Is.TypeOf(typeof(WaitingForFirstTouchState)));
			Assert.That(engine.GetWFTapState(), Is.TypeOf(typeof(WaitingForTapState)));
			Assert.That(engine.GetWFReleaseState(), Is.TypeOf(typeof(WaitingForReleaseState)));
			Assert.That(engine.GetWFNextTouchState(), Is.TypeOf(typeof(WaitingForNextTouchState)));
		}
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
			public override void OnDrag(Vector2 dragPos, Vector2 dragDeltaP){}
			public override void OnPointerEnter(ICustomEventData eventData){}
			public override void OnPointerExit(ICustomEventData eventData){}
		}
		class UIAStateEngineConstArg{
			public UIAStateEngineConstArg(IUIAdaptor uia, IUIElement uie, IProcessFactory procFac, IWaitAndExpireProcess wfTapProcess, IWaitAndExpireProcess wfNextTouchProcess, IWaitAndExpireProcess wfReleaseProcess){
				this.uia = uia;
				this.uie = uie;
				this.procFac = procFac;
				this.wfTapProcess = wfTapProcess;
				this.wfNextTouchProcess = wfNextTouchProcess;
				this.wfReleaseProcess = wfReleaseProcess;
			}
			public IUIAdaptor uia;
			public IUIElement uie;
			public IProcessFactory procFac;
			public IWaitAndExpireProcess wfTapProcess;
			public IWaitAndExpireProcess wfNextTouchProcess;
			public IWaitAndExpireProcess wfReleaseProcess;

		}
		class TestUIAStateEngine: UIAdaptorStateEngine{
			public TestUIAStateEngine(IUIAdaptor uia, IProcessFactory procFac): base(uia, procFac){}
			public IUIAdaptorInputState GetCurState(){
				return this.curState;
			}
			public bool IsWaitingForFirstTouch(){
				return this.curState is WaitingForFirstTouchState;
			}
			public WaitingForFirstTouchState GetWFFirstTouchState(){
				return this.waitingForFirstTouchState;
			}
			public bool IsWaitingForTap(){
				return this.curState is WaitingForTapState;
			}
			public WaitingForTapState GetWFTapState(){
				return this.waitingForTapState;
			}
			public bool IsWaitingForRelease(){
				return this.curState is WaitingForReleaseState;
			}
			public WaitingForReleaseState GetWFReleaseState(){
				return this.waitingForReleaseState;
			}
			public bool IsWaitingForNextTouch(){
				return this.curState is WaitingForNextTouchState;
			}
			public WaitingForNextTouchState GetWFNextTouchState(){
				return this.waitingForNextTouchState;
			}
			public bool StatesAreAllSet(){
				return 
					this.waitingForFirstTouchState != null &&
					this.waitingForTapState != null &&
					this.waitingForReleaseState != null &&
					this.waitingForNextTouchState != null;
			}
			public float GetDragThreshold(){
				return this.dragDeltaPThreshold;
			}
		}
		TestUIAStateEngine CreateTestUIAStateEngine(out UIAStateEngineConstArg arg){
			/*  ** Note **
				tapExpT and ntExpT MUST be different value in order this to work properly
			*/
			IUIAdaptor mockUIA = Substitute.For<IUIAdaptor>();
				IUIElement mockUIE = Substitute.For<IUIElement>();
				mockUIA.GetUIElement().Returns(mockUIE);
			IProcessFactory mockProcFac = Substitute.For<IProcessFactory>();
				IWaitAndExpireProcess mockWFTapProcess = Substitute.For<IWaitAndExpireProcess>();
				IWaitAndExpireProcess mockWFNextTouchProcess = Substitute.For<IWaitAndExpireProcess>();
				IWaitAndExpireProcess mockWFReleaseProcess = Substitute.For<IWaitAndExpireProcess>();
				mockWFTapProcess.When(x => x.Run()).Do(x => mockWFTapProcess.IsRunning().Returns(true));
				mockWFNextTouchProcess.When(x => x.Run()).Do(x => mockWFNextTouchProcess.IsRunning().Returns(true));
				mockWFReleaseProcess.When(x => x.Run()).Do(x => mockWFReleaseProcess.IsRunning().Returns(true));
				mockProcFac.CreateWaitAndExpireProcess(Arg.Any<WaitingForTapState>(), Arg.Any<float>()).Returns(mockWFTapProcess);
				mockProcFac.CreateWaitAndExpireProcess(Arg.Any<WaitingForNextTouchState>(), Arg.Any<float>()).Returns(mockWFNextTouchProcess);
				mockProcFac.CreateWaitAndExpireProcess(Arg.Any<WaitingForReleaseState>(), Arg.Any<float>()).Returns(mockWFReleaseProcess);
			TestUIAStateEngine engine = new TestUIAStateEngine(mockUIA, mockProcFac);
				mockWFTapProcess.When(x => x.Expire()).Do(x => engine.GetWFTapState().OnProcessExpire());
				mockWFNextTouchProcess.When(x => x.Expire()).Do(x => engine.GetWFNextTouchState().OnProcessExpire());
				mockWFReleaseProcess.When(x => x.Expire()).Do(x => engine.GetWFReleaseState().OnProcessExpire());
			arg = new UIAStateEngineConstArg(mockUIA, mockUIE, mockProcFac, mockWFTapProcess, mockWFNextTouchProcess, mockWFReleaseProcess);
			return engine;
		}
		TestUIAStateEngine CreateTestUIAStateEngine(){
			UIAStateEngineConstArg arg;
			return CreateTestUIAStateEngine(out arg);
		}
	/*  */
}
