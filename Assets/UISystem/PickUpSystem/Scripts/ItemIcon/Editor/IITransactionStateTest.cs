using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using NSubstitute;
using UISystem;
using UISystem.PickUpUISystem;

[TestFixture, Category("PickUpSystem"), Ignore]
public class IITransactionStateTest{
	[Test]
	public void EqpIIHoveredState_OnEnter_ThisEqpIIsInEqpIG_ThisEqpIIIsEmpty_DoesNotCallEqpIITAM(){
		IEqpIITAStateConstArg arg;
		IEquippableItemIcon eqpII;
		EqpIIHoveredState state = CreateEqpIIHoveredState(out arg, out eqpII);
		eqpII.IsInEqpIG().Returns(true);
		eqpII.IsEmpty().Returns(true);

		state.OnEnter();

		IEquippableIITAManager mockEqpIITAM = arg.eqpIITAM;
		mockEqpIITAM.DidNotReceive().SetEqpIIToEquip(eqpII);
		mockEqpIITAM.DidNotReceive().SetEqpIIToUnequip(eqpII);
	}
	[Test]
	public void EqpIIHoveredState_OnEnter_ThisEqpIIsInEqpIG_ThisEqpIIIsNotEmpty_ThisEqpIIHasSameItemAsPicked_DoesNotCallEqpIITAM(){
		IEqpIITAStateConstArg arg;
		IEquippableItemIcon eqpII;
		EqpIIHoveredState state = CreateEqpIIHoveredState(out arg, out eqpII);
		eqpII.IsInEqpIG().Returns(true);
		eqpII.IsEmpty().Returns(false);
		IEquippableItemIcon pickedEqpII = Substitute.For<IEquippableItemIcon>();
		eqpII.HasSameItem(pickedEqpII).Returns(true);
		state.SetPickedItemIcon(pickedEqpII);

		state.OnEnter();

		IEquippableIITAManager mockEqpIITAM = arg.eqpIITAM;
		mockEqpIITAM.DidNotReceive().SetEqpIIToEquip(eqpII);
		mockEqpIITAM.DidNotReceive().SetEqpIIToUnequip(eqpII);
	}
	[Test]
	public void EqpIIHoveredState_OnEnter_ThisEqpIIsInEqpIG_ThisEqpIIIsNotEmpty_ThisEqpIIDoesNotHaveSameItemAsPicked_CallsEqpIITAMSetEqpIIToUnequipThis(){
		IEqpIITAStateConstArg arg;
		IEquippableItemIcon eqpII;
		EqpIIHoveredState state = CreateEqpIIHoveredState(out arg, out eqpII);
		eqpII.IsInEqpIG().Returns(true);
		eqpII.IsEmpty().Returns(false);
		IEquippableItemIcon pickedEqpII = Substitute.For<IEquippableItemIcon>();
		eqpII.HasSameItem(pickedEqpII).Returns(false);
		state.SetPickedItemIcon(pickedEqpII);

		state.OnEnter();

		IEquippableIITAManager mockEqpIITAM = arg.eqpIITAM;
		mockEqpIITAM.DidNotReceive().SetEqpIIToEquip(eqpII);
		mockEqpIITAM.Received(1).SetEqpIIToUnequip(eqpII);
	}
	[Test]
	public void EqpIIHoveredState_OnEnter_ThisEqpIIInPoolIG_ThisEqpIIHasSameItemAsPicked_DoesNotCallEqpIITAMSetEqpIIToEquipThis(){
		IEqpIITAStateConstArg arg;
		IEquippableItemIcon eqpII;
		EqpIIHoveredState state = CreateEqpIIHoveredState(out arg, out eqpII);
		eqpII.IsInEqpIG().Returns(false);
		IEquippableItemIcon pickedEqpII = Substitute.For<IEquippableItemIcon>();
		eqpII.HasSameItem(pickedEqpII).Returns(true);
		state.SetPickedItemIcon(pickedEqpII);

		state.OnEnter();

		IEquippableIITAManager mockEqpIITAM = arg.eqpIITAM;
		mockEqpIITAM.DidNotReceive().SetEqpIIToEquip(eqpII);
	}
	[Test]
	public void EqpIIHoveredState_OnEnter_ThisEqpIIInPoolIG_ThisEqpIIDoesNotHaveSameItemAsPicked_CallsEqpIITAMSetEqpIIToEquipThis(){
		IEqpIITAStateConstArg arg;
		IEquippableItemIcon eqpII;
		EqpIIHoveredState state = CreateEqpIIHoveredState(out arg, out eqpII);
		eqpII.IsInEqpIG().Returns(false);
		IEquippableItemIcon pickedEqpII = Substitute.For<IEquippableItemIcon>();
		eqpII.HasSameItem(pickedEqpII).Returns(false);
		state.SetPickedItemIcon(pickedEqpII);

		state.OnEnter();

		IEquippableIITAManager mockEqpIITAM = arg.eqpIITAM;
		mockEqpIITAM.Received(1).SetEqpIIToEquip(eqpII);
	}
	public EqpIIHoveredState CreateEqpIIHoveredState(out IEqpIITAStateConstArg arg, out IEquippableItemIcon eqpII){
		IEqpIITAStateConstArg thisArg = Substitute.For<IEqpIITAStateConstArg>();
		IEquippableIITAManager thisEqpIITAM = Substitute.For<IEquippableIITAManager>();
		thisArg.iiTAM.Returns(thisEqpIITAM);
		thisArg.eqpIITAM.Returns(thisEqpIITAM);
		thisArg.eqpTool.Returns(Substitute.For<IEquipTool>());

		EqpIIHoveredState hoveredState = new EqpIIHoveredState(thisArg);
		IEquippableItemIcon thisEqpII = Substitute.For<IEquippableItemIcon>();
		hoveredState.SetItemIcon(thisEqpII);
		arg = thisArg;
		eqpII = thisEqpII;
		return hoveredState;
	}
}
