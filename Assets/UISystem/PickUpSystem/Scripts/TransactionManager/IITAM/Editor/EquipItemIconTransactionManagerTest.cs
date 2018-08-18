using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using NSubstitute;
using UISystem;
using UISystem.PickUpUISystem;

[TestFixture, Category("PickUpSystem"), Ignore]
public class EquipItemIconTransactionManagerTest {
    [Test]
    public void GetAllRelevantIGs_ArgIsNull_ReturnsPoolIGAndAllRelevantEqpIGs(){
        IEqpIITAMConstArg arg;
        EquippableItemIconTransactionManager eqpIITAM = CreateEqpIITAM(out arg);
        IEquipToolIGManager eqpToolIGManager = arg.eqpToolIGManager;

        List<IIconGroup> actualIGs = eqpIITAM.GetAllRelevantIGs(null);
        List<IIconGroup> expectedIGs = new List<IIconGroup>(new IIconGroup[]{
            eqpToolIGManager.GetRelevantPoolIG(),
            eqpToolIGManager.GetRelevantEquippedBowIG(),
            eqpToolIGManager.GetRelevantEquippedWearIG(),
            eqpToolIGManager.GetRelevantEquippedCarriedGearsIG()
        });

        Assert.That(actualIGs, Is.EqualTo(expectedIGs));
    }
    [Test]
    public void GetAllRelevantIGs_ArgIsNotNull_ReturnsPoolIGAndRelevantEquipIG(){
        IEqpIITAMConstArg arg;
        EquippableItemIconTransactionManager eqpIITAM = CreateEqpIITAM(out arg);
        IEquipToolIGManager eqpToolIGManager = arg.eqpToolIGManager;
        IEquippableItemIcon pickedEqpII = Substitute.For<IEquippableItemIcon>();

        List<IIconGroup> actualIGs = eqpIITAM.GetAllRelevantIGs(pickedEqpII);
        List<IEquipToolIG> expectedEqpToolIGs = new List<IEquipToolIG>( new IEquipToolIG[]{
            eqpToolIGManager.GetRelevantPoolIG(),
            eqpToolIGManager.GetRelevantEquipIG(pickedEqpII)
        });

        Assert.That(actualIGs, Is.EqualTo(expectedEqpToolIGs));
    }
    [Test]
    public void SetPickedII_SetsPickedEqpIIArg(){
        IEqpIITAMConstArg arg;
        EquippableItemIconTransactionManager eqpIITAM = CreateEqpIITAM(out arg);
        IEquippableItemIcon pickedEqpII = Substitute.For<IEquippableItemIcon>();
        
        eqpIITAM.SetPickedII(pickedEqpII);

        Assert.That(eqpIITAM.GetPickedEqpII(), Is.SameAs(pickedEqpII));
    }
    [Test]
    public void EvaluateHoverability_CallsPanelsAndAllRelevIGsEvaluateHov(){
        IEqpIITAMConstArg arg;
        EquippableItemIconTransactionManager eqpIITAM = CreateEqpIITAM(out arg);
        IEquippableItemIcon pickedEqpII = Substitute.For<IEquippableItemIcon>();
        eqpIITAM.SetPickedII(pickedEqpII);

        eqpIITAM.EvaluateHoverability();

        arg.equippedItemsPanel.Received(1).EvaluateHoverability(pickedEqpII);
        arg.poolItemsPanel.Received(1).EvaluateHoverability(pickedEqpII);
        arg.eqpToolIGManager.GetRelevantPoolIG().Received(1).EvaluateAllIIsHoverability(pickedEqpII);
        arg.eqpToolIGManager.GetRelevantEquipIG(pickedEqpII).Received(1).EvaluateAllIIsHoverability(pickedEqpII);
    }
    [Test]
    public void ExcecuteTransaction_ThisEIIToUnequipIsNotNull_CallsItUnequip(){
        IEqpIITAMConstArg arg;
        TestEqpIITAM testEqpIITAM = CreateTestEqpIITAM(out arg);
        IEquippableItemIcon eii = Substitute.For<IEquippableItemIcon>();
        testEqpIITAM.SetEIIToUnequip(eii);
        Assert.That(testEqpIITAM.GetEIIToUnequip(), Is.SameAs(eii));

        testEqpIITAM.ExecuteTransaction();

        eii.Received(1).Unequip();
    }
    [Test]
    public void ExcecuteTransaction_ThisEIIToEquipIsNotNull_CallsItEquip(){
        IEqpIITAMConstArg arg;
        TestEqpIITAM testEqpIITAM = CreateTestEqpIITAM(out arg);
        IEquippableItemIcon eii = Substitute.For<IEquippableItemIcon>();
        testEqpIITAM.SetEIIToEquip(eii);
        Assert.That(testEqpIITAM.GetEIIToEquip(), Is.SameAs(eii));

        testEqpIITAM.ExecuteTransaction();

        eii.Received(1).Equip();
    }
    [Test]
    public void ExcecuteTransaction_ThisEIIToUnequipIsNotNull_ThisEIIToUnequipIsSameAsPickedEqpII_CallsItImmigrateToPool(){
        IEqpIITAMConstArg arg;
        TestEqpIITAM testEqpIITAM = CreateTestEqpIITAM(out arg);
        IEquippableItemIcon eii = Substitute.For<IEquippableItemIcon>();
        testEqpIITAM.SetEIIToUnequip(eii);
        Assert.That(testEqpIITAM.GetEIIToUnequip(), Is.SameAs(eii));
        testEqpIITAM.SetPickedII(eii);
        Assert.That(testEqpIITAM.GetPickedEqpII(), Is.SameAs(testEqpIITAM.GetEIIToUnequip()));
        IEquipToolPoolIG poolIG = arg.eqpToolIGManager.GetRelevantPoolIG();

        testEqpIITAM.ExecuteTransaction();

        eii.Received(1).TravelTransfer(poolIG);
    }
    [Test]
    public void ExcecuteTransaction_ThisEIIToEquipIsNotNull_ThisEIIToEquipIsSameAsPickedEqpII_CallsItImmigrateToRelevEqpIG(){
        IEqpIITAMConstArg arg;
        TestEqpIITAM testEqpIITAM = CreateTestEqpIITAM(out arg);
        IEquippableItemIcon eii = Substitute.For<IEquippableItemIcon>();
        testEqpIITAM.SetEIIToEquip(eii);
        Assert.That(testEqpIITAM.GetEIIToEquip(), Is.SameAs(eii));
        testEqpIITAM.SetPickedII(eii);
        Assert.That(testEqpIITAM.GetPickedEqpII(), Is.SameAs(testEqpIITAM.GetEIIToEquip()));
        IEquipToolEquipIG eqpIG = arg.eqpToolIGManager.GetRelevantEquipIG(eii);

        testEqpIITAM.ExecuteTransaction();

        eii.Received(1).TravelTransfer(eqpIG);
    }
    [Test]
    public void ExcecuteTransaction_ThisEIIToUnequipIsNotNull_ThisEIIToUnequipIsNotSameAsPickedEqpII_CallsItTransferToPool(){
        IEqpIITAMConstArg arg;
        TestEqpIITAM testEqpIITAM = CreateTestEqpIITAM(out arg);
        IEquippableItemIcon eii = Substitute.For<IEquippableItemIcon>();
        testEqpIITAM.SetEIIToUnequip(eii);
        Assert.That(testEqpIITAM.GetEIIToUnequip(), Is.SameAs(eii));
        IEquippableItemIcon otherEii = Substitute.For<IEquippableItemIcon>();
        testEqpIITAM.SetPickedII(otherEii);
        Assert.That(testEqpIITAM.GetPickedEqpII(), Is.Not.SameAs(testEqpIITAM.GetEIIToUnequip()));
        IEquipToolPoolIG poolIG = arg.eqpToolIGManager.GetRelevantPoolIG();

        testEqpIITAM.ExecuteTransaction();

        eii.Received(1).SpotTransfer(poolIG);
    }
    [Test]
    public void ExcecuteTransaction_ThisEIIToUnequipIsNotNull_ThisEIIToEquipIsNotSameAsPickedEqpII_CallsItTransferToRelevEqpIG(){
        IEqpIITAMConstArg arg;
        TestEqpIITAM testEqpIITAM = CreateTestEqpIITAM(out arg);
        IEquippableItemIcon eii = Substitute.For<IEquippableItemIcon>();
        testEqpIITAM.SetEIIToEquip(eii);
        Assert.That(testEqpIITAM.GetEIIToEquip(), Is.SameAs(eii));
        IEquippableItemIcon otherEii = Substitute.For<IEquippableItemIcon>();
        testEqpIITAM.SetPickedII(otherEii);
        Assert.That(testEqpIITAM.GetPickedEqpII(), Is.Not.SameAs(testEqpIITAM.GetEIIToEquip()));
        IEquipToolEquipIG eqpIG = arg.eqpToolIGManager.GetRelevantEquipIG(eii);

        testEqpIITAM.ExecuteTransaction();

        eii.Received(1).SpotTransfer(eqpIG);
    }
    [Test]
    public void HoverInitialPickUpReceiver_PickedEqpIIIsInEqpIG_CallsEqpPanelCheckForHover(){
        IEqpIITAMConstArg arg;
        TestEqpIITAM testEqpIITAM = CreateTestEqpIITAM(out arg);
        IEquippableItemIcon pickedEqpII = Substitute.For<IEquippableItemIcon>();
        pickedEqpII.IsInEqpIG().Returns(true);
        testEqpIITAM.SetPickedII(pickedEqpII);

        testEqpIITAM.HoverInitialPickUpReceiver();

        arg.equippedItemsPanel.Received(1).CheckForHover();
    }
    [Test]
    public void HoverInitialPickUpReceiver_PickedEqpIIIsNotInEqpIG_CallsPoolPanelCheckForHover(){
        IEqpIITAMConstArg arg;
        TestEqpIITAM testEqpIITAM = CreateTestEqpIITAM(out arg);
        IEquippableItemIcon pickedEqpII = Substitute.For<IEquippableItemIcon>();
        pickedEqpII.IsInEqpIG().Returns(false);
        testEqpIITAM.SetPickedII(pickedEqpII);

        testEqpIITAM.HoverInitialPickUpReceiver();

        arg.poolItemsPanel.Received(1).CheckForHover();
    }
    /*  */
    public EquippableItemIconTransactionManager CreateEqpIITAM(out IEqpIITAMConstArg arg){
        IEqpIITAMConstArg thisArg = CreateStubEqpIITAMConstArg();
        EquippableItemIconTransactionManager eqpIITAM = new EquippableItemIconTransactionManager(thisArg);
        arg = thisArg;
        return eqpIITAM;
    }
    public TestEqpIITAM CreateTestEqpIITAM(out IEqpIITAMConstArg arg){
        IEqpIITAMConstArg thisArg = CreateStubEqpIITAMConstArg();
        TestEqpIITAM testEqpIITAM = new TestEqpIITAM(thisArg);
        arg = thisArg;
        return testEqpIITAM;
    }
    public IEqpIITAMConstArg CreateStubEqpIITAMConstArg(){
        IEqpIITAMConstArg arg = Substitute.For<IEqpIITAMConstArg>();
        arg.eqpIITAMStateEngine.Returns(Substitute.For<IEqpIITAMStateEngine>());
        arg.equippedItemsPanel.Returns(Substitute.For<IEquipToolPanel>());
        arg.poolItemsPanel.Returns(Substitute.For<IEquipToolPanel>());
        arg.hoveredEqpIISwitch.Returns(Substitute.For<IPickUpReceiverSwitch<IEquippableItemIcon>>());
        arg.hoveredEqpToolPanelSwitch.Returns(Substitute.For<IPickUpReceiverSwitch<IEquipToolPanel>>());
        IEquipToolIGManager eqpToolIGManager = CreateStubEqpToolIGManager();
        arg.eqpToolIGManager.Returns(eqpToolIGManager);
        return arg;
    }
    public IEquipToolIGManager CreateStubEqpToolIGManager(){
        IEquipToolIGManager eqpToolIGManager = Substitute.For<IEquipToolIGManager>();
        IEquipToolPoolIG poolIG = Substitute.For<IEquipToolPoolIG>();
        IEquipToolEquippedBowIG equippedBowIG = Substitute.For<IEquipToolEquippedBowIG>();
        IEquipToolEquippedWearIG equippedWearIG = Substitute.For<IEquipToolEquippedWearIG>();
        IEquipToolEquippedCarriedGearsIG equippedCGearsIG = Substitute.For<IEquipToolEquippedCarriedGearsIG>();
        eqpToolIGManager.GetRelevantPoolIG().Returns(poolIG);
        eqpToolIGManager.GetRelevantEquippedBowIG().Returns(equippedBowIG);
        eqpToolIGManager.GetRelevantEquippedWearIG().Returns(equippedWearIG);
        eqpToolIGManager.GetRelevantEquippedCarriedGearsIG().Returns(equippedCGearsIG);
        eqpToolIGManager.GetRelevantEquipIG(Arg.Any<IEquippableItemIcon>()).Returns(Substitute.For<IEquipToolEquipIG>());
        List<IEquipToolEquipIG> allEquipIGs = new List<IEquipToolEquipIG>(new IEquipToolEquipIG[]{
            equippedBowIG, equippedWearIG, equippedCGearsIG
        });
        eqpToolIGManager.GetAllRelevantEquipIGs().Returns(allEquipIGs);

        return eqpToolIGManager;
    }
    public class TestEqpIITAM: EquippableItemIconTransactionManager{
        public TestEqpIITAM(IEqpIITAMConstArg arg): base(arg){}
        public IEquippableItemIcon GetEIIToEquip(){
            return thisEIIToEquip;
        }
        public IEquippableItemIcon GetEIIToUnequip(){
            return thisEIIToUnequip;
        }
        public void SetEIIToEquip(IEquippableItemIcon eii){
            thisEIIToEquip = eii;
        }
        public void SetEIIToUnequip(IEquippableItemIcon eii){
            thisEIIToUnequip = eii;
        }
    }
}
