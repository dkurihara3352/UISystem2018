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
	const int velocityStackSize = 3;
	[Test]
	public void AbsPointerUpInputState_OnPointerUp_ThrowsException(){
		AbsPointerUpInputState state = new TestAbsPointerUpState(Substitute.For<IUIAdaptorInputStateEngine>());
		
		Assert.Throws(
			Is.TypeOf(typeof(System.InvalidOperationException)).And.Message.EqualTo("OnPointerUp should not be called while pointer is already held up"),
			() =>{
				state.OnPointerUp(Substitute.For<ICustomEventData>());
			}
		);
	}
	[Test]
	public void AbsPointerUpInputState_OnDrag_ThrowsException(){
		AbsPointerUpInputState state = new TestAbsPointerUpState(Substitute.For<IUIAdaptorInputStateEngine>());
		
		Assert.Throws(
			Is.TypeOf(typeof(System.InvalidOperationException)).And.Message.EqualTo("OnDrag should be impossible when pointer is held up, something's wrong"),
			() =>{
				state.OnDrag(Substitute.For<ICustomEventData>());
			}
		);
	}
	[Test]
	public void AbsPointerUpInputState_OnBeginDrag_ThrowsException(){
		AbsPointerUpInputState state = new TestAbsPointerUpState(Substitute.For<IUIAdaptorInputStateEngine>());

		Assert.Throws(
			Is.TypeOf(typeof(System.InvalidOperationException)).And.Message.EqualTo("OnBeginDrag should not be called while pointer is held up"),
			() => {
				state.OnBeginDrag(Substitute.For<ICustomEventData>());
			}
		);
	}
	[Test]
	public void AbsPointerUpInputProcessState_OnEnter_CallsThisProcessRun(){
		TestAbsPointerUpInputProcessState state = new TestAbsPointerUpInputProcessState(Substitute.For<IUISystemProcessFactory>(), Substitute.For<IUIAdaptorInputStateEngine>());
		
		state.OnEnter();

		state.process.Received(1).Run();
	}
	[Test]
	public void AbsPointerUpInputProcessState_ExpireProcess_ThisProcessIsRunning_CallsThisProcessExpire(){
		TestAbsPointerUpInputProcessState state = new TestAbsPointerUpInputProcessState(Substitute.For<IUISystemProcessFactory>(), Substitute.For<IUIAdaptorInputStateEngine>());

		state.OnEnter();
		Assert.That(state.process.IsRunning(), Is.True);

		state.ExpireProcess();
		state.process.Received(1).Expire();
		Assert.That(state.process.IsRunning(), Is.False);

		state.ExpireProcess();
		state.process.Received(1).Expire();
	}
	[Test]
	public void AbsPointerUpInputProcessState_OnExit_ThisProcessIsRunning_CallsThisProcessStopAndSetNull(){
		TestAbsPointerUpInputProcessState state = new TestAbsPointerUpInputProcessState(Substitute.For<IUISystemProcessFactory>(), Substitute.For<IUIAdaptorInputStateEngine>());
		IWaitAndExpireProcess process = state.process;
		
		state.OnEnter();
		Assert.That(process.IsRunning(), Is.True);

		state.OnExit();
		process.Received(1).Stop();

		Assert.That(state.process, Is.Null);
	}
	[Test]
	public void AbsPointerDownIputState_OnPointerDown_ThrowsException(){
		TestAbsPointerDownInputState state = new TestAbsPointerDownInputState(Substitute.For<IUIAdaptorInputStateEngine>(), Substitute.For<IUIManager>(), velocityStackSize);

		Assert.Throws(
			Is.TypeOf(typeof(System.InvalidOperationException)).And.Message.EqualTo("OnPointerDown should not be called while pointer is already held down"),
			() => {
				state.OnPointerDown(Substitute.For<ICustomEventData>());
			}
		);
	}
	[Test, TestCaseSource(typeof(AbsPointerDownInputState_VelocityIsOverSwipeVelocityThreshold_TestCase), "cases")]
	public void AbsPointerDownInputState_VelocityIsOverSwipeVelocityThreshold_Various(Vector2 velocity, float velocityThreshold, bool expected){
		IUIAdaptorInputStateEngine engine = Substitute.For<IUIAdaptorInputStateEngine>();
		TestAbsPointerDownInputState state = new TestAbsPointerDownInputState(engine, Substitute.For<IUIManager>(), velocityStackSize);
		engine.GetSwipeVelocityThreshold().Returns(velocityThreshold);

		bool actual = state.VelocityIsOverSwipeVelocityThreshold_Test(velocity);

		Assert.That(actual, Is.EqualTo(expected));
	}
	public class AbsPointerDownInputState_VelocityIsOverSwipeVelocityThreshold_TestCase{
		public static object[] cases = {
			new object[]{new Vector2(5f, 0f), 4.9f, true},
			new object[]{new Vector2(5f, 0f), 5f, true},
			new object[]{new Vector2(5f, 0f), 5.1f, false},

			new object[]{new Vector2(0f, 5f), 4.9f, true},
			new object[]{new Vector2(0f, 5f), 5f, true},
			new object[]{new Vector2(0f, 5f), 5.1f, false},
		};
	}
	[Test, TestCaseSource(typeof(AbsPointerDownInputState_OnDrag_CallsUIMUpdateDragWorldPosition_TestCase), "cases")]
	public void AbsPointerDownInputState_OnDrag_CallsUIMUpdateDragWorldPosition(Vector2 dragWorldPosition){
		IUIManager uim = Substitute.For<IUIManager>();
		TestAbsPointerDownInputState state = new TestAbsPointerDownInputState(Substitute.For<IUIAdaptorInputStateEngine>(), uim, velocityStackSize);
		ICustomEventData customEventData = Substitute.For<ICustomEventData>();
		customEventData.position.Returns(dragWorldPosition);

		state.OnDrag(customEventData);

		uim.Received(1).SetDragWorldPosition(dragWorldPosition);
	}
	public class AbsPointerDownInputState_OnDrag_CallsUIMUpdateDragWorldPosition_TestCase{
		public static object[] cases = {
			new object[]{new Vector2(10f, 20f)}
		};
	}
	[Test]
	public void AbsPointerDownInputState_OnDrag_CallsEngineDragUIE(){
		IUIAdaptorInputStateEngine engine = Substitute.For<IUIAdaptorInputStateEngine>();
		AbsPointerDownInputState state = new TestAbsPointerDownInputState(engine, Substitute.For<IUIManager>(), velocityStackSize);

		ICustomEventData data = Substitute.For<ICustomEventData>();
		state.OnDrag(data);

		engine.Received(1).DragUIE(data);
	}
	[Test]
	public void AbsPointerDownInputState_OnBeginDrag_CallsEngineBeginDragUIE(){
		IUIAdaptorInputStateEngine engine = Substitute.For<IUIAdaptorInputStateEngine>();
		AbsPointerDownInputState state = new TestAbsPointerDownInputState(engine, Substitute.For<IUIManager>(), velocityStackSize);

		ICustomEventData data = Substitute.For<ICustomEventData>();
		state.OnBeginDrag(data);

		engine.Received(1).BeginDragUIE(data);
	}
	[Test]
	public void AbsPointerDownInputProcessState_OnEnter_CallsThisProcessRun(){
		TestAbsPointerDownInputProcessState state = new TestAbsPointerDownInputProcessState(Substitute.For<IUISystemProcessFactory>(), Substitute.For<IUIAdaptorInputStateEngine>(), Substitute.For<IUIManager>(), velocityStackSize);
		
		state.OnEnter();

		state.process.Received(1).Run();
	}
	[Test]
	public void AbsPointerDownInputProcessState_ExpireProcess_ThisProcessIsRunning_CallsThisProcessExpire(){
		TestAbsPointerDownInputProcessState state = new TestAbsPointerDownInputProcessState(Substitute.For<IUISystemProcessFactory>(), Substitute.For<IUIAdaptorInputStateEngine>(), Substitute.For<IUIManager>(), velocityStackSize);

		state.OnEnter();
		Assert.That(state.process.IsRunning(), Is.True);

		state.ExpireProcess();
		state.process.Received(1).Expire();
		Assert.That(state.process.IsRunning(), Is.False);

		state.ExpireProcess();
		state.process.Received(1).Expire();
	}
	[Test]
	public void AbsPointerDownInputProcessState_OnExit_ThisProcessIsRunning_CallsThisProcessStopAndSetNull(){
		TestAbsPointerDownInputProcessState state = new TestAbsPointerDownInputProcessState(Substitute.For<IUISystemProcessFactory>(), Substitute.For<IUIAdaptorInputStateEngine>(), Substitute.For<IUIManager>(), velocityStackSize);
		IWaitAndExpireProcess process = state.process;
		
		state.OnEnter();
		Assert.That(process.IsRunning(), Is.True);

		state.OnExit();
		process.Received(1).Stop();

		Assert.That(state.process, Is.Null);
	}





	public class TestAbsPointerUpState: AbsPointerUpInputState{
		public TestAbsPointerUpState(IUIAdaptorInputStateEngine engine):base(engine){}
		public override void OnPointerDown(ICustomEventData eventData){}
		public override void OnEnter(){}
		public override void OnExit(){}
	}
	public class TestAbsPointerUpInputProcessState: AbsPointerUpInputProcessState<IWaitAndExpireProcess>{
		public TestAbsPointerUpInputProcessState(IUISystemProcessFactory processFactory, IUIAdaptorInputStateEngine engine):base(processFactory, engine){
			thisProcess = Substitute.For<IWaitAndExpireProcess>();
			thisProcess.When(x => {
				x.Run();
			}).Do(x =>{
				thisProcess.IsRunning().Returns(true);
			});
			thisProcess.When(x => {
				x.Stop();
			}).Do(x =>{
				thisProcess.IsRunning().Returns(false);
			});
			thisProcess.When(x => {
				x.Expire();
			}).Do(x => {
				thisProcess.Stop();
			});
		}
		protected override IWaitAndExpireProcess CreateProcess(){return thisProcess;}
		public override void OnPointerDown(ICustomEventData eventData){}
		public IWaitAndExpireProcess process{get{return thisProcess;}}
	}
	[Test]
	public void TestAbsPointerUpInputProcessState_WorksFine(){
		TestAbsPointerUpInputProcessState state = new TestAbsPointerUpInputProcessState(Substitute.For<IUISystemProcessFactory>(), Substitute.For<IUIAdaptorInputStateEngine>());
		IWaitAndExpireProcess process = state.process;
		
		Assert.That(process.IsRunning(), Is.False);
		process.Run();
		Assert.That(process.IsRunning(), Is.True);
		process.Stop();
		Assert.That(process.IsRunning(), Is.False);
		process.Run();
		Assert.That(process.IsRunning(), Is.True);
		process.Expire();
		Assert.That(process.IsRunning(), Is.False);
	}
	public class TestAbsPointerDownInputProcessState: AbsPointerDownInputProcessState<IWaitAndExpireProcess>{
		public TestAbsPointerDownInputProcessState(IUISystemProcessFactory processFactory, IUIAdaptorInputStateEngine engine, IUIManager uim, int velocityStackSize):base(processFactory, engine, uim, velocityStackSize){
			thisProcess = Substitute.For<IWaitAndExpireProcess>();
			thisProcess.When(x => {
				x.Run();
			}).Do(x =>{
				thisProcess.IsRunning().Returns(true);
			});
			thisProcess.When(x => {
				x.Stop();
			}).Do(x =>{
				thisProcess.IsRunning().Returns(false);
			});
			thisProcess.When(x => {
				x.Expire();
			}).Do(x => {
				thisProcess.Stop();
			});
		}
		protected override IWaitAndExpireProcess CreateProcess(){return thisProcess;}
		public IWaitAndExpireProcess process{get{return thisProcess;}}
		public override void OnPointerUp(ICustomEventData eventData){}
		public override void OnPointerEnter(ICustomEventData eventData){}
		public override void OnPointerExit(ICustomEventData eventData){}
	}
	[Test]
	public void TestAbsPointerDownInputProcessState_WorksFine(){
		TestAbsPointerDownInputProcessState state = new TestAbsPointerDownInputProcessState(Substitute.For<IUISystemProcessFactory>(), Substitute.For<IUIAdaptorInputStateEngine>(), Substitute.For<IUIManager>(), 3);
		IWaitAndExpireProcess process = state.process;
		
		Assert.That(process.IsRunning(), Is.False);
		process.Run();
		Assert.That(process.IsRunning(), Is.True);
		process.Stop();
		Assert.That(process.IsRunning(), Is.False);
		process.Run();
		Assert.That(process.IsRunning(), Is.True);
		process.Expire();
		Assert.That(process.IsRunning(), Is.False);
	}
	public class TestAbsPointerDownInputState: AbsPointerDownInputState{
		public TestAbsPointerDownInputState(IUIAdaptorInputStateEngine engine, IUIManager uim, int velocityStackSize): base(engine, uim, velocityStackSize){}
		public override void OnEnter(){}
		public override void OnExit(){}
		public override void OnPointerUp(ICustomEventData eventData){}
		public override void OnPointerEnter(ICustomEventData eventData){}
		public override void OnPointerExit(ICustomEventData eventData){}
		public bool VelocityIsOverSwipeVelocityThreshold_Test(Vector2 velocity){
			return this.VelocityIsOverSwipeVelocityThreshold(velocity);
		}
	}
}
