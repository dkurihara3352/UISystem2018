using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using NSubstitute;
using UISystem;
using UISystem.PickUpUISystem;

[TestFixture, Category("PickUpSystem"), Ignore]
public class ItemIconTransactionManagerTest {
	[Test]
	public void Construction_CallsStateEngineSetIITAMThis(){
		IItemIconTAManagerStateEngine stateEngine = Substitute.For<IItemIconTAManagerStateEngine>();
		
		TestIITAM testIITAM = new TestIITAM(stateEngine);
		
		stateEngine.Received(1).SetIITAM(testIITAM);
	}
	[Test]
	public void Activate_CallsEngineSetToDefaultState(){
		IItemIconTAManagerStateEngine stateEngine = Substitute.For<IItemIconTAManagerStateEngine>();
		TestIITAM testIITAM = new TestIITAM(stateEngine);

		testIITAM.Activate();

		stateEngine.Received(1).SetToDefaultState();
	}

	/*  */
	public class TestIITAM: AbsItemIconTransactionManager{
		public TestIITAM(IItemIconTAManagerStateEngine stateEngine): base(stateEngine){}
		public override IPickUpContextUIE GetPickUpContextUIE(){
			return null;
		}
		public override void ExecuteTransaction(){}
		protected override void ClearHoverFields(){}
		public override void EvaluateHoverability(){}
		public override void ResetHoverability(){}
		public override List<IIconGroup> GetAllRelevantIGs(IItemIcon pickedII){
			return null;
		}
		public override void HoverInitialPickUpReceiver(){}
	}
}
