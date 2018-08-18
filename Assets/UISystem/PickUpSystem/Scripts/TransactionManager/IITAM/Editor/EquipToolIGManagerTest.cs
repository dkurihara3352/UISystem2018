using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using NSubstitute;
using UISystem;
using UISystem.PickUpUISystem;

[TestFixture, Category("PickUpSystem"), Ignore]
public class EquipToolIGManagerTest {
    [Test]
    public void GetRelevantEquipIG_ArgItemTempIsBow_ReturnsRelevantEquippedBowIG(){
        ITestEquipToolIGManagerConstArg arg;
        TestEquipToolIGManager testIGManager = CreateTestEquipToolIGManager(out arg);
        IEquippableItemIcon pickedEqpII = Substitute.For<IEquippableItemIcon>();
        pickedEqpII.GetItemTemplate().Returns(Substitute.For<IBowTemplate>());

        IEquipToolIG actualEqpToolIG = testIGManager.GetRelevantEquipIG(pickedEqpII);

        Assert.That(actualEqpToolIG, Is.SameAs(arg.bowIG));
    }
    [Test]
    public void GetRelevantEquipIG_ArgItemTempIsWear_ReturnsRelevantEquippedWearIG(){
        ITestEquipToolIGManagerConstArg arg;
        TestEquipToolIGManager testIGManager = CreateTestEquipToolIGManager(out arg);
        IEquippableItemIcon pickedEqpII = Substitute.For<IEquippableItemIcon>();
        pickedEqpII.GetItemTemplate().Returns(Substitute.For<IWearTemplate>());

        IEquipToolIG actualEqpToolIG = testIGManager.GetRelevantEquipIG(pickedEqpII);

        Assert.That(actualEqpToolIG, Is.SameAs(arg.wearIG));
    }
    [Test]
    public void GetRelevantEquipIG_ArgItemTempIsCGears_ReturnsRelevantEquippedCGearsIG(){
        ITestEquipToolIGManagerConstArg arg;
        TestEquipToolIGManager testIGManager = CreateTestEquipToolIGManager(out arg);
        IEquippableItemIcon pickedEqpII = Substitute.For<IEquippableItemIcon>();
        pickedEqpII.GetItemTemplate().Returns(Substitute.For<ICarriedGearTemplate>());

        IEquipToolIG actualEqpToolIG = testIGManager.GetRelevantEquipIG(pickedEqpII);

        Assert.That(actualEqpToolIG, Is.SameAs(arg.cgIG));
    }
    [Test]
    public void GetAllRelevantEquipIGs_ReturnsSumOfAllEquipIGs(){
        ITestEquipToolIGManagerConstArg arg;
        TestEquipToolIGManager testIGManager = CreateTestEquipToolIGManager(out arg);
        List<IEquipToolEquipIG> expectedEquipIGs = new List<IEquipToolEquipIG>(new IEquipToolEquipIG[]{
            arg.bowIG, arg.wearIG, arg.cgIG
        });
        List<IEquipToolEquipIG> actualEquipIGs = testIGManager.GetAllRelevantEquipIGs();

        Assert.That(actualEquipIGs, Is.EqualTo(expectedEquipIGs));
    }
    public class TestEquipToolIGManager: EquipToolIGManager{
        public TestEquipToolIGManager(ITestEquipToolIGManagerConstArg arg){
            thisPoolIG = arg.poolIG;
            thisBowIG = arg.bowIG;
            thisWearIG = arg.wearIG;
            thisCGIG = arg.cgIG;
        }
        readonly IEquipToolPoolIG thisPoolIG;
        public override IEquipToolPoolIG GetRelevantPoolIG(){return thisPoolIG;}
        readonly IEquipToolEquippedBowIG thisBowIG;
        public override IEquipToolEquippedBowIG GetRelevantEquippedBowIG(){return thisBowIG;}
        readonly IEquipToolEquippedWearIG thisWearIG;
        public override IEquipToolEquippedWearIG GetRelevantEquippedWearIG(){return thisWearIG;}
        readonly IEquipToolEquippedCarriedGearsIG thisCGIG;
        public override IEquipToolEquippedCarriedGearsIG GetRelevantEquippedCarriedGearsIG(){return thisCGIG;}
    }
    public interface ITestEquipToolIGManagerConstArg{
        IEquipToolPoolIG poolIG{get;}
        IEquipToolEquippedBowIG bowIG{get;}
        IEquipToolEquippedWearIG wearIG{get;}
        IEquipToolEquippedCarriedGearsIG cgIG{get;}
    }
    public TestEquipToolIGManager CreateTestEquipToolIGManager(out ITestEquipToolIGManagerConstArg arg){
        ITestEquipToolIGManagerConstArg thisArg = Substitute.For<ITestEquipToolIGManagerConstArg>();
        thisArg.poolIG.Returns(Substitute.For<IEquipToolPoolIG>());
        thisArg.bowIG.Returns(Substitute.For<IEquipToolEquippedBowIG>());
        thisArg.wearIG.Returns(Substitute.For<IEquipToolEquippedWearIG>());
        thisArg.cgIG.Returns(Substitute.For<IEquipToolEquippedCarriedGearsIG>());

        arg = thisArg;
        return new TestEquipToolIGManager(thisArg);
    }

}
