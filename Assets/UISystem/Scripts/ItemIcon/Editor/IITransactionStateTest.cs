using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using NSubstitute;
using UISystem;

[TestFixture]
public class IITransactionStateTest{
	[Test]
	public void EqpIIHoveredState_OnEnter_ThisEqpIIsInEqpIG_ThisEqpIIIsEmpty_DoesNotCallEqpIITAM(){
		IEqpIITAStateConstArg arg;
		EqpIIHoveredState state = CreateEqpIIHoveredState(out arg);
		arg.eqpII.IsInEqpIG().Returns(true);
		arg.eqpII.IsEmpty().Returns(true);

		state.OnEnter();

		IEquippableIITAManager mockEqpIITAM = arg.eqpIITAM;
		mockEqpIITAM.DidNotReceive().SetEqpIIToEquip(arg.eqpII);
		mockEqpIITAM.DidNotReceive().SetEqpIIToUnequip(arg.eqpII);
	}
	[Test]
	public void EqpIIHoveredState_OnEnter_ThisEqpIIsInEqpIG_ThisEqpIIIsNotEmpty_ThisEqpIIHasSameItemAsPicked_DoesNotCallEqpIITAM(){
		IEqpIITAStateConstArg arg;
		EqpIIHoveredState state = CreateEqpIIHoveredState(out arg);
		arg.eqpII.IsInEqpIG().Returns(true);
		arg.eqpII.IsEmpty().Returns(false);
		IEquippableItemIcon pickedEqpII = Substitute.For<IEquippableItemIcon>();
		arg.eqpII.HasSameItem(pickedEqpII).Returns(true);
		state.SetPickedItemIcon(pickedEqpII);

		state.OnEnter();

		IEquippableIITAManager mockEqpIITAM = arg.eqpIITAM;
		mockEqpIITAM.DidNotReceive().SetEqpIIToEquip(arg.eqpII);
		mockEqpIITAM.DidNotReceive().SetEqpIIToUnequip(arg.eqpII);
	}
	[Test]
	public void EqpIIHoveredState_OnEnter_ThisEqpIIsInEqpIG_ThisEqpIIIsNotEmpty_ThisEqpIIDoesNotHaveSameItemAsPicked_CallsEqpIITAMSetEqpIIToUnequipThis(){
		IEqpIITAStateConstArg arg;
		EqpIIHoveredState state = CreateEqpIIHoveredState(out arg);
		arg.eqpII.IsInEqpIG().Returns(true);
		arg.eqpII.IsEmpty().Returns(false);
		IEquippableItemIcon pickedEqpII = Substitute.For<IEquippableItemIcon>();
		arg.eqpII.HasSameItem(pickedEqpII).Returns(false);
		state.SetPickedItemIcon(pickedEqpII);

		state.OnEnter();

		IEquippableIITAManager mockEqpIITAM = arg.eqpIITAM;
		mockEqpIITAM.DidNotReceive().SetEqpIIToEquip(arg.eqpII);
		mockEqpIITAM.Received(1).SetEqpIIToUnequip(arg.eqpII);
	}
	[Test]
	public void EqpIIHoveredState_OnEnter_ThisEqpIIInPoolIG_ThisEqpIIHasSameItemAsPicked_DoesNotCallEqpIITAMSetEqpIIToEquipThis(){
		IEqpIITAStateConstArg arg;
		EqpIIHoveredState state = CreateEqpIIHoveredState(out arg);
		arg.eqpII.IsInEqpIG().Returns(false);
		IEquippableItemIcon pickedEqpII = Substitute.For<IEquippableItemIcon>();
		arg.eqpII.HasSameItem(pickedEqpII).Returns(true);
		state.SetPickedItemIcon(pickedEqpII);

		state.OnEnter();

		IEquippableIITAManager mockEqpIITAM = arg.eqpIITAM;
		mockEqpIITAM.DidNotReceive().SetEqpIIToEquip(arg.eqpII);
	}
	[Test]
	public void EqpIIHoveredState_OnEnter_ThisEqpIIInPoolIG_ThisEqpIIDoesNotHaveSameItemAsPicked_CallsEqpIITAMSetEqpIIToEquipThis(){
		IEqpIITAStateConstArg arg;
		EqpIIHoveredState state = CreateEqpIIHoveredState(out arg);
		arg.eqpII.IsInEqpIG().Returns(false);
		IEquippableItemIcon pickedEqpII = Substitute.For<IEquippableItemIcon>();
		arg.eqpII.HasSameItem(pickedEqpII).Returns(false);
		state.SetPickedItemIcon(pickedEqpII);

		state.OnEnter();

		IEquippableIITAManager mockEqpIITAM = arg.eqpIITAM;
		mockEqpIITAM.Received(1).SetEqpIIToEquip(arg.eqpII);
	}
	public EqpIIHoveredState CreateEqpIIHoveredState(out IEqpIITAStateConstArg arg){
		IEqpIITAStateConstArg thisArg = Substitute.For<IEqpIITAStateConstArg>();
		IEquippableItemIcon thisEqpII = Substitute.For<IEquippableItemIcon>();
		thisArg.itemIcon.Returns(thisEqpII);
		thisArg.eqpII.Returns(thisEqpII);
		IEquippableIITAManager thisEqpIITAM = Substitute.For<IEquippableIITAManager>();
		thisArg.iiTAM.Returns(thisEqpIITAM);
		thisArg.eqpIITAM.Returns(thisEqpIITAM);
		thisArg.eqpTool.Returns(Substitute.For<IEquipTool>());

		EqpIIHoveredState hoveredState = new EqpIIHoveredState(thisArg);
		arg = thisArg;
		return hoveredState;
	}
}
