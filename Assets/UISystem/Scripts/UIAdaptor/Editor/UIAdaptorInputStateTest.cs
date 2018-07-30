using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using NSubstitute;
using UISystem;
using DKUtility;

[TestFixture, Category("UISystem")]
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
		TestPointerDownInputState state = new TestPointerDownInputState(engine, Substitute.For<IUIManager>());
		ICustomEventData eventData = Substitute.For<ICustomEventData>();

		state.OnPointerDown(eventData);
	}
	[Test]
	public void PointerUpInputState_OnDrag_WhenCalled_ThrowsException(){
		IUIAdaptorStateEngine engine = Substitute.For<IUIAdaptorStateEngine>();
		TestPointerUpInputState state = new TestPointerUpInputState(engine);

		Assert.Throws(typeof(System.InvalidOperationException), () => state.OnDrag(Substitute.For<ICustomEventData>()));
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
	public void TestUIAStateEngine_OnPointerUp_WhileWFTap_BecomesWFNextTouchState(){
		TestUIAStateEngine engine = CreateTestUIAStateEngine();
		ICustomEventData eventData = Substitute.For<ICustomEventData>();
		engine.OnPointerDown(eventData);
		Assert.That(engine.IsWaitingForTap(), Is.True);
		
		engine.OnPointerUp(eventData);

		Assert.That(engine.IsWaitingForNextTouch(), Is.True);
	}
	[Test]
	public void TestUIAStateEngine_OnPointerUp_WhileWFTap_RunsWFNTProcess(){
		UIAStateEngineConstArg arg;
		TestUIAStateEngine engine = CreateTestUIAStateEngine(out arg);
		ICustomEventData eventData = Substitute.For<ICustomEventData>();
		IWaitAndExpireProcess wfNTProcess = arg.wfNextTouchProcess;
		engine.OnPointerDown(eventData);
		Assert.That(engine.IsWaitingForTap(), Is.True);
		
		engine.OnPointerUp(eventData);

		wfNTProcess.Received(1).Run();
	}
	[Test][TestCaseSource(typeof(PointerUpDeltaCase), "underThreshCases")]
	public void TestUIAStateEngine_OnPointerUp_WhileWFTap_DeltaPUnderThreshold_CallsUIEOnTapArgOne(float vector2X){
		UIAStateEngineConstArg arg;
		TestUIAStateEngine engine = CreateTestUIAStateEngine(out arg);
		ICustomEventData eventData = Substitute.For<ICustomEventData>();
			eventData.deltaPos.Returns(new Vector2(vector2X, 0f));
		IUIElement uie = arg.uie;
		engine.OnPointerDown(eventData);
		Assert.That(engine.IsWaitingForTap(), Is.True);
		
		engine.OnPointerUp(eventData);

		uie.DidNotReceive().OnSwipe(Arg.Any<ICustomEventData>());
		uie.Received(1).OnTap(1);
	}
	[Test][TestCaseSource(typeof(PointerUpDeltaCase), "overThreshCases")]
	public void TestUIAStateEngine_OnPointerUp_WhileWFTap_DeltaPOverThreshold_CallsUIEOnSwipe(float vector2X){
		UIAStateEngineConstArg arg;
		TestUIAStateEngine engine = CreateTestUIAStateEngine(out arg);
		ICustomEventData eventData = Substitute.For<ICustomEventData>();
			eventData.deltaPos.Returns(new Vector2(vector2X, 0f));
		IUIElement uie = arg.uie;
		engine.OnPointerDown(eventData);
		Assert.That(engine.IsWaitingForTap(), Is.True);
		
		engine.OnPointerUp(eventData);
		
		uie.DidNotReceive().OnTap(Arg.Any<int>());
		uie.Received(1).OnSwipe(eventData);
	}
		class PointerUpDeltaCase{
			static object[] overThreshCases = {
				new object[]{5f},
				new object[]{5.001f},
				new object[]{6f},
				new object[]{200f}
			};
			static object[] underThreshCases = {
				new object[]{4.9f},
				new object[]{4.9999f},
				new object[]{.0001f}
			};
			static object[] variousThreshCases = {
				new object[]{5f},
				new object[]{5.001f},
				new object[]{6f},
				new object[]{200f},
				new object[]{4.9f},
				new object[]{4.9999f},
				new object[]{.0001f}

			};
		}
	[Test][TestCaseSource(typeof(PointerUpDeltaCase), "variousThreshCases")]
	public void TestUIAStateEngine_OnPointerUp_WhileWFTap_CallsUIEOnRelease(float vector2X){
		UIAStateEngineConstArg arg;
		TestUIAStateEngine engine = CreateTestUIAStateEngine(out arg);
		IUIElement uie = arg.uie;
		ICustomEventData eventData = Substitute.For<ICustomEventData>();
			eventData.deltaPos.Returns(new Vector2(vector2X, 0f));
		engine.OnPointerDown(eventData);
		Assert.That(engine.IsWaitingForTap(), Is.True);

		engine.OnPointerUp(eventData);

		uie.Received(1).OnRelease();
	}
	[Test]
	public void TestUIAStateEngine_OnPointerUp_WhileWFTap_WFTapProcessStops(){
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
	public void TestUIAStateEngine_ProcessExpires_WhileWFTap_BecomesWFReleaseState(){
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
	public void TestUIAStateEngine_ProcessExpires_WhileWFTap_RunsWFReleaseProcess(){
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
	public void TestUIAStateEngine_ProcessExpires_WhileWFTap_UIECalledOnDelayedTouch(){
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
	public void TestUIAStateEngine_OnPointerExit_WhileWFTap_BecomesWFRelease(){//and run process, reset touchC
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
	public void TestUIAStateEngine_OnPointerExit_WhileWFTap_ResetTouchCount(){
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
	public void TestUIAStateEngine_OnPointerExit_WhileWFTap_RunsWFReleaseProcess(){
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
	public void TestUIAStateEngine_OnPointerUp_WhielWFRelease_BecomesWaitingForNextTouch(){
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
	public void TestUIAStateEngine_OnPointerUp_WhielWFRelease_StopsWFReleaseProcess(){
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
	[Test, TestCaseSource(typeof(PointerUpDeltaCase), "overThreshCases")]
	public void TestUIAStateEngine_OnPointerUp_WhileWFRelease_DeltaPIsOverThreshold_CallsUIEOnSwipe(float vector2X){
		UIAStateEngineConstArg arg;
		TestUIAStateEngine engine = CreateTestUIAStateEngine(out arg);
		ICustomEventData eventData = Substitute.For<ICustomEventData>();
			eventData.deltaPos.Returns(new Vector2(vector2X, 0f));
		IUIElement uie = arg.uie;
		IWaitAndExpireProcess wfTapProcess = arg.wfTapProcess;
		engine.OnPointerDown(eventData);
		wfTapProcess.Expire();
		Assert.That(engine.IsWaitingForRelease(), Is.True);

		engine.OnPointerUp(eventData);

		uie.Received(1).OnSwipe(eventData);
	}
	[Test, TestCaseSource(typeof(PointerUpDeltaCase), "underThreshCases")]
	public void TestUIAStateEngine_OnPointerUp_WhileWFRelease_DeltaPIsNotOverThreshold_DoesNotCallUIEOnSwipe(float vector2X){
		UIAStateEngineConstArg arg;
		TestUIAStateEngine engine = CreateTestUIAStateEngine(out arg);
		ICustomEventData eventData = Substitute.For<ICustomEventData>();
			eventData.deltaPos.Returns(new Vector2(vector2X, 0f));
		IUIElement uie = arg.uie;
		IWaitAndExpireProcess wfTapProcess = arg.wfTapProcess;
		engine.OnPointerDown(eventData);
		wfTapProcess.Expire();
		Assert.That(engine.IsWaitingForRelease(), Is.True);

		engine.OnPointerUp(eventData);

		uie.DidNotReceive().OnSwipe(Arg.Any<ICustomEventData>());
	}
	[Test]
	public void TestUIAStateEngine_OnPointerDown_WhileInWFNextTouch_BecomesWFTapState(){
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
	public void TestUIAStateEngine_OnPointerDown_WhileInWFNextTouch_RunsWFTapProcess(){
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
		ICustomEventData eventData = Substitute.For<ICustomEventData>();
		Vector2 dragDeltaP = new Vector2(vector2X, 0f);
		eventData.deltaPos.Returns(dragDeltaP);
		engine.OnPointerDown(Substitute.For<ICustomEventData>());
		Assert.That(engine.GetCurState(), Is.InstanceOf(typeof(AbsPointerDownInputState)));

		engine.OnDrag(eventData);
		
		if(dragDeltaP.sqrMagnitude >= engine.GetDragThreshold() * engine.GetDragThreshold()){
			Assert.That(overThreshold, Is.True);
			uie.Received(1).OnDrag(eventData);
		}else{
			Assert.That(overThreshold, Is.False);
			uie.DidNotReceive().OnDrag(eventData);
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
		ICustomEventData eventData = Substitute.For<ICustomEventData>();
		Vector2 dragDeltaP = new Vector2(vector2X, 0f);
		eventData.deltaPos.Returns(dragDeltaP);
		engine.OnPointerDown(Substitute.For<ICustomEventData>());
		Assert.That(engine.IsWaitingForTap(), Is.True);
		
		engine.OnDrag(eventData);
		
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
		class TestPointerUpInputState: AbsPointerUpInputState{
			public TestPointerUpInputState(IUIAdaptorStateEngine engine) :base(engine){}
			public override void OnEnter(){}
			public override void OnExit(){}
			public override void OnPointerDown(ICustomEventData eventData){}
		}
		class TestPointerDownInputState: AbsPointerDownInputState{
			public TestPointerDownInputState(IUIAdaptorStateEngine engine, IUIManager uim) :base(engine, uim){}
			public override void OnEnter(){}
			public override void OnExit(){}
			public override void OnPointerUp(ICustomEventData eventData){}
			public override void OnDrag(ICustomEventData eventData){}
			public override void OnPointerEnter(ICustomEventData eventData){}
			public override void OnPointerExit(ICustomEventData eventData){}
		}
		class UIAStateEngineConstArg{
			public UIAStateEngineConstArg(IUIAdaptor uia, IUIElement uie, IUISystemProcessFactory procFac, IWaitAndExpireProcess wfTapProcess, IWaitAndExpireProcess wfNextTouchProcess, IWaitAndExpireProcess wfReleaseProcess){
				this.uia = uia;
				this.uie = uie;
				this.procFac = procFac;
				this.wfTapProcess = wfTapProcess;
				this.wfNextTouchProcess = wfNextTouchProcess;
				this.wfReleaseProcess = wfReleaseProcess;
			}
			public IUIAdaptor uia;
			public IUIElement uie;
			public IUISystemProcessFactory procFac;
			public IWaitAndExpireProcess wfTapProcess;
			public IWaitAndExpireProcess wfNextTouchProcess;
			public IWaitAndExpireProcess wfReleaseProcess;

		}
		class TestUIAStateEngine: UIAdaptorStateEngine{
			public TestUIAStateEngine(IUIManager uim, IUIAdaptor uia, IUISystemProcessFactory procFac): base(uim, uia, procFac){}
			public IUIAdaptorInputState GetCurState(){
				return thisCurState;
			}
			public bool IsWaitingForFirstTouch(){
				return thisCurState is WaitingForFirstTouchState;
			}
			public WaitingForFirstTouchState GetWFFirstTouchState(){
				return thisWaitingForFirstTouchState;
			}
			public bool IsWaitingForTap(){
				return thisCurState is WaitingForTapState;
			}
			public WaitingForTapState GetWFTapState(){
				return thisWaitingForTapState;
			}
			public bool IsWaitingForRelease(){
				return thisCurState is WaitingForReleaseState;
			}
			public WaitingForReleaseState GetWFReleaseState(){
				return thisWaitingForReleaseState;
			}
			public bool IsWaitingForNextTouch(){
				return thisCurState is WaitingForNextTouchState;
			}
			public WaitingForNextTouchState GetWFNextTouchState(){
				return thisWaitingForNextTouchState;
			}
			public bool StatesAreAllSet(){
				return 
					this.thisWaitingForFirstTouchState != null &&
					this.thisWaitingForTapState != null &&
					this.thisWaitingForReleaseState != null &&
					this.thisWaitingForNextTouchState != null;
			}
			public float GetDragThreshold(){
				return this.dragDeltaPThreshold;
			}
		}
		TestUIAStateEngine CreateTestUIAStateEngine(out UIAStateEngineConstArg arg){
			/*  ** Note **
				tapExpT and ntExpT MUST be different value in order this to work properly
			*/
			IUIManager uim = Substitute.For<IUIManager>();
			IUIAdaptor mockUIA = Substitute.For<IUIAdaptor>();
				IUIElement mockUIE = Substitute.For<IUIElement>();
				mockUIA.GetUIElement().Returns(mockUIE);
			IUISystemProcessFactory mockProcFac = Substitute.For<IUISystemProcessFactory>();
				IWaitAndExpireProcess mockWFTapProcess = Substitute.For<IWaitAndExpireProcess>();
				IWaitAndExpireProcess mockWFNextTouchProcess = Substitute.For<IWaitAndExpireProcess>();
				IWaitAndExpireProcess mockWFReleaseProcess = Substitute.For<IWaitAndExpireProcess>();
				mockWFTapProcess.When(x => x.Run()).Do(x => mockWFTapProcess.IsRunning().Returns(true));
				mockWFNextTouchProcess.When(x => x.Run()).Do(x => mockWFNextTouchProcess.IsRunning().Returns(true));
				mockWFReleaseProcess.When(x => x.Run()).Do(x => mockWFReleaseProcess.IsRunning().Returns(true));
				mockProcFac.CreateWaitAndExpireProcess(Arg.Any<WaitingForTapState>(), Arg.Any<float>()).Returns(mockWFTapProcess);
				mockProcFac.CreateWaitAndExpireProcess(Arg.Any<WaitingForNextTouchState>(), Arg.Any<float>()).Returns(mockWFNextTouchProcess);
				mockProcFac.CreateWaitAndExpireProcess(Arg.Any<WaitingForReleaseState>(), Arg.Any<float>()).Returns(mockWFReleaseProcess);
			TestUIAStateEngine engine = new TestUIAStateEngine(uim, mockUIA, mockProcFac);
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
