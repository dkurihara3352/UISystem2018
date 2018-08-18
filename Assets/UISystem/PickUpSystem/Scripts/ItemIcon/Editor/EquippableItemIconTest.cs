using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using NSubstitute;
using UISystem;
using UISystem.PickUpUISystem;

[TestFixture, Category("PickUpSystem"), Ignore]
public class EquippableItemIconTest {
    [Test]
    public void CheckForPickUp_ThisIsPicked_DoesNotCallEnginePickUp(){
        IEquippableItemIconConstArg arg;
        TestEqpII testEqpII = CreateTestEqpIIWithPickability(isPicked: true, arg: out arg);
        IEqpIITransactionStateEngine eqpIITAStateEngine = (IEqpIITransactionStateEngine)arg.iiTAStateEngine;

        testEqpII.TestCheckForPickUp();

        eqpIITAStateEngine.DidNotReceive().PickUp();
    }
    [Test]
    public void CheckForPickUp_ThisIsNotPickedUp_ThisIsNotPickable_DoesNotCallEnginePickUp(){
        IEquippableItemIconConstArg arg;
        TestEqpII testEqpII = CreateTestEqpII(out arg);
        IEqpIITransactionStateEngine eqpIITAStateEngine = (IEqpIITransactionStateEngine)arg.iiTAStateEngine;
        eqpIITAStateEngine.IsPicked().Returns(false);
        Assert.That(testEqpII.IsPicked(), Is.False);
        eqpIITAStateEngine.IsPickable().Returns(false);
        Assert.That(testEqpII.IsPickable(),Is.False);

        testEqpII.TestCheckForPickUp();

        eqpIITAStateEngine.DidNotReceive().PickUp();
    }
    [Test]
    public void CheckForPickUp_ThisIsNotPickedUp_ThisIsPickable_CallsEnginePickUp(){
        IEquippableItemIconConstArg arg;
        TestEqpII testEqpII = CreateTestEqpII(out arg);
        IEqpIITransactionStateEngine eqpIITAStateEngine = (IEqpIITransactionStateEngine)arg.iiTAStateEngine;
        eqpIITAStateEngine.IsPicked().Returns(false);
        Assert.That(testEqpII.IsPicked(), Is.False);
        eqpIITAStateEngine.IsPickable().Returns(true);
        Assert.That(testEqpII.IsPickable(),Is.True);

        testEqpII.TestCheckForPickUp();

        eqpIITAStateEngine.Received(1).PickUp();
    }
    [Test]
    public void IsEligibleForQuickDrop_ThisItemIsNotStackable_ReturnsTrue(){
        IEquippableItemIconConstArg arg;
        TestEqpII testEqpII = CreateTestEqpII(out arg);
        IUIItem item = arg.item;
        item.IsStackable().Returns(false);

        Assert.That(testEqpII.TestIsEligibleForQuickDrop(), Is.True);
    }
    [Test]
    public void IsEligibleForQuickDrop_ThisItemIsStackable_ThisDoesNotHaveSameItemAsHoveredEqpII_ReturnsTrue(){
        IEquippableItemIconConstArg arg;
        TestEqpII testEqpII = CreateTestEqpII(out arg);
        IUIItem item = arg.item;
        item.IsStackable().Returns(true);
        IEquippableItemIcon hoveredEqpII = Substitute.For<IEquippableItemIcon>();
        IEquippableUIItem hoveredItem = Substitute.For<IEquippableUIItem>();
        hoveredEqpII.GetEquippableItem().Returns(hoveredItem);
        item.IsSameAs(hoveredItem).Returns(false);
        ((IEquippableIITAManager)arg.iiTAM).GetHoveredEqpII().Returns(hoveredEqpII);

        Assert.That(testEqpII.TestIsEligibleForQuickDrop(), Is.True);
    }
    [Test]
    public void IsEligibleForQuickDrop_ThisItemIsStackable_ThisHasSameItemAsHoveredEqpII_ReturnsFalse(){
        IEquippableItemIconConstArg arg;
        TestEqpII testEqpII = CreateTestEqpII(out arg);
        IUIItem item = arg.item;
        item.IsStackable().Returns(true);
        IEquippableItemIcon hoveredEqpII = Substitute.For<IEquippableItemIcon>();
        IEquippableUIItem hoveredItem = Substitute.For<IEquippableUIItem>();
        hoveredEqpII.GetEquippableItem().Returns(hoveredItem);
        item.IsSameAs(hoveredItem).Returns(true);
        ((IEquippableIITAManager)arg.iiTAM).GetHoveredEqpII().Returns(hoveredEqpII);

        Assert.That(testEqpII.TestIsEligibleForQuickDrop(), Is.False);
    }
    [Test]
    public void IsEligibleForHover_ThisIsInSourceIG_ThisIsEmpty_ReturnsTrue(){
        IEquippableItemIconConstArg arg;
        TestEqpII testEqpII = CreateTestEqpII(out arg);
        IEquippableItemIcon pickedEqpII = Substitute.For<IEquippableItemIcon>();
        IIconGroup thisIG = Substitute.For<IIconGroup>();
        testEqpII.SetIconGroup(thisIG);
        pickedEqpII.GetIconGroup().Returns(thisIG);
        IItemIconEmptinessStateEngine emptinessStateEngine = arg.emptinessStateEngine;
        emptinessStateEngine.IsEmpty().Returns(true);

        Assert.That(testEqpII.TestIsEligibleForHover(pickedEqpII), Is.True);
    }
    [Test]
    public void IsEligibleForHover_ThisIsInSourceIG_ThisIsNotEmpty_ThisHasSameItemAsPickedEqpII_ReturnsTrue(){
        IEquippableItemIconConstArg arg;
        TestEqpII testEqpII = CreateTestEqpII(out arg);
        IEquippableItemIcon pickedEqpII = Substitute.For<IEquippableItemIcon>();
        IIconGroup thisIG = Substitute.For<IIconGroup>();
        testEqpII.SetIconGroup(thisIG);
        pickedEqpII.GetIconGroup().Returns(thisIG);
        IEquippableUIItem pickedEqpItem = Substitute.For<IEquippableUIItem>();
        pickedEqpII.GetEquippableItem().Returns(pickedEqpItem);
        arg.item.IsSameAs(pickedEqpItem).Returns(true);
        IItemIconEmptinessStateEngine emptinessStateEngine = arg.emptinessStateEngine;
        emptinessStateEngine.IsEmpty().Returns(false);

        Assert.That(testEqpII.TestIsEligibleForHover(pickedEqpII), Is.True);
    }
    [Test]
    public void IsEligibleForHover_ThisIsInSourceIG_ThisIsNotEmpty_ThisDoesNotHaveSameItemAsPickedEqpII_ReturnsFalse(){
        IEquippableItemIconConstArg arg;
        TestEqpII testEqpII = CreateTestEqpII(out arg);
        IEquippableItemIcon pickedEqpII = Substitute.For<IEquippableItemIcon>();
        IIconGroup thisIG = Substitute.For<IIconGroup>();
        testEqpII.SetIconGroup(thisIG);
        pickedEqpII.GetIconGroup().Returns(thisIG);
        IEquippableUIItem pickedEqpItem = Substitute.For<IEquippableUIItem>();
        pickedEqpII.GetEquippableItem().Returns(pickedEqpItem);
        arg.item.IsSameAs(pickedEqpItem).Returns(false);
        IItemIconEmptinessStateEngine emptinessStateEngine = arg.emptinessStateEngine;
        emptinessStateEngine.IsEmpty().Returns(false);

        Assert.That(testEqpII.TestIsEligibleForHover(pickedEqpII), Is.False);
    }
    [Test]
    public void IsEligibleForHover_ThisIsInDestIG_ThisIsEmpty_ReturnsTrue(){
        IEquippableItemIconConstArg arg;
        TestEqpII testEqpII = CreateTestEqpII(out arg);
        IEquippableItemIcon pickedEqpII = Substitute.For<IEquippableItemIcon>();
        IIconGroup thisIG = Substitute.For<IIconGroup>();
        testEqpII.SetIconGroup(thisIG);
        IIconGroup otherIG = Substitute.For<IIconGroup>();
        pickedEqpII.GetIconGroup().Returns(otherIG);
        IItemIconEmptinessStateEngine emptinessStateEngine = arg.emptinessStateEngine;
        emptinessStateEngine.IsEmpty().Returns(true);

        Assert.That(testEqpII.TestIsEligibleForHover(pickedEqpII), Is.True);
    }
    [Test]
    public void IsEligibleForHover_ThisIsInDestIG_ThisIsNotEmpty_ThisHasSameItemTempAsPicked_ThisIsInPoolIG_ThisIsNotTransferable_ReturnsFalse(){
        IEquippableItemIconConstArg arg;
        TestEqpII testEqpII = CreateTestEqpII(quantity: 1, tempType: typeof(IBowTemplate) ,arg: out arg);
        IEquippableItemIcon pickedEqpII = Substitute.For<IEquippableItemIcon>();
        IEquipToolPoolIG thisPoolIG = Substitute.For<IEquipToolPoolIG>();
        testEqpII.SetIconGroup(thisPoolIG);
        IIconGroup otherIG = Substitute.For<IIconGroup>();
        pickedEqpII.GetIconGroup().Returns(otherIG);
        IItemIconEmptinessStateEngine emptinessStateEngine = arg.emptinessStateEngine;
        emptinessStateEngine.IsEmpty().Returns(false);
        pickedEqpII.GetItemTemplate().Returns(Substitute.For<IBowTemplate>());
        testEqpII.UpdateTransferableQuantity(1);
        Assert.That(testEqpII.IsTransferable(), Is.False);

        Assert.That(testEqpII.TestIsEligibleForHover(pickedEqpII), Is.False);
    }
    [Test]
    public void IsEligibleForHover_ThisIsInDestIG_ThisIsNotEmpty_ThisHasSameItemTempAsPicked_ThisIsNotInPoolIG_ReturnsTrue(){
        IEquippableItemIconConstArg arg;
        TestEqpII testEqpII = CreateTestEqpII(quantity: 1, tempType: typeof(IBowTemplate) ,arg: out arg);
        IEquippableItemIcon pickedEqpII = Substitute.For<IEquippableItemIcon>();
        IEquipToolEquippedBowIG thisEqpIG = Substitute.For<IEquipToolEquippedBowIG>();
        testEqpII.SetIconGroup(thisEqpIG);
        IIconGroup otherIG = Substitute.For<IIconGroup>();
        pickedEqpII.GetIconGroup().Returns(otherIG);
        IItemIconEmptinessStateEngine emptinessStateEngine = arg.emptinessStateEngine;
        emptinessStateEngine.IsEmpty().Returns(false);
        pickedEqpII.GetItemTemplate().Returns(Substitute.For<IBowTemplate>());

        Assert.That(testEqpII.TestIsEligibleForHover(pickedEqpII), Is.True);
    }
    [Test]
    public void IsEligibleForHover_ThisIsInDestIG_ThisIsNotEmpty_ThisDoesNotHaveSameItemTempAsPicked_ReturnsFalse(){
        IEquippableItemIconConstArg arg;
        TestEqpII testEqpII = CreateTestEqpII(quantity: 1, tempType: typeof(IBowTemplate) ,arg: out arg);
        IEquippableItemIcon pickedEqpII = Substitute.For<IEquippableItemIcon>();
        IEquipToolEquippedBowIG thisEqpIG = Substitute.For<IEquipToolEquippedBowIG>();
        testEqpII.SetIconGroup(thisEqpIG);
        IIconGroup otherIG = Substitute.For<IIconGroup>();
        pickedEqpII.GetIconGroup().Returns(otherIG);
        IItemIconEmptinessStateEngine emptinessStateEngine = arg.emptinessStateEngine;
        emptinessStateEngine.IsEmpty().Returns(false);
        pickedEqpII.GetItemTemplate().Returns(Substitute.For<IWearTemplate>());

        Assert.That(testEqpII.TestIsEligibleForHover(pickedEqpII), Is.False);
    }
    /*  */   
    [Test]
    public void IsBowOrWearItemIcon_ItemTempIsBow_ReturnTrue(){
        IEquippableItemIconConstArg arg;
        TestEqpII testEqpII = CreateTestEqpII(typeof(IBowTemplate), out arg);

        Assert.That(testEqpII.IsBowOrWearItemIcon(), Is.True);
    }
    [Test]
    public void IsBowOrWearItemIcon_ItemTempIsWear_ReturnTrue(){
        IEquippableItemIconConstArg arg;
        TestEqpII testEqpII = CreateTestEqpII(typeof(IWearTemplate), out arg);

        Assert.That(testEqpII.IsBowOrWearItemIcon(), Is.True);
    }
    [Test]
    public void IsBowOrWearItemIcon_ItemTempIsCGear_ReturnsFalse(){
        IEquippableItemIconConstArg arg;
        TestEqpII testEqpII = CreateTestEqpII(typeof(ICarriedGearTemplate), out arg);

        Assert.That(testEqpII.IsBowOrWearItemIcon(), Is.False);
    }
    [Test]
    public void GetItemQuantity_ReturnsItemQuantity([Values(0, 1, 10)]int quantity){
        IEquippableItemIconConstArg arg;
        TestEqpII testEqpII = CreateTestEqpII(quantity, typeof(ICarriedGearTemplate), out arg);

        Assert.That(testEqpII.GetItemQuantity, Is.EqualTo(quantity));
    }
    [Test]
    public void IsInEqpIG_ThisIGIsEqpToolEqpIG_ReturnsTrue(){
        IEquippableItemIconConstArg arg;
        TestEqpII testEqpII = CreateTestEqpII(true, 0, typeof(IBowTemplate), out arg);

        Assert.That(testEqpII.IsInEqpIG(), Is.True);
    }
    [Test]
    public void IsInEqpIG_ThisIGIsEqpToolEqpIG_ReturnsFalse(){
        IEquippableItemIconConstArg arg;
        TestEqpII testEqpII = CreateTestEqpII(false, 0, typeof(IBowTemplate), out arg);

        Assert.That(testEqpII.IsInEqpIG(), Is.False);
    }
    [Test]
    public void IsInPoolIG_ThisIGIsEqpToolPoolIG_ReturnsTrue(){
        IEquippableItemIconConstArg arg;
        TestEqpII testEqpII = CreateTestEqpII(out arg);
        IEquipToolPoolIG poolIG = Substitute.For<IEquipToolPoolIG>();
        testEqpII.SetIconGroup(poolIG);

        Assert.That(testEqpII.IsInPoolIG(), Is.True);
    }
    /*  */
    public TestEqpII CreateTestEqpII(out IEquippableItemIconConstArg arg){
        IEquippableItemIconConstArg thisArg = Substitute.For<IEquippableItemIconConstArg>();
        IUIManager uim = Substitute.For<IUIManager>();
        thisArg.uim.Returns(uim);
        IPickUpSystemProcessFactory pickUpSystemProcessFactory = Substitute.For<IPickUpSystemProcessFactory>();
        thisArg.processFactory.Returns(pickUpSystemProcessFactory);
        IEquipToolUIEFactory eqpToolUIEFactory = Substitute.For<IEquipToolUIEFactory>();
        thisArg.uiElementFactory.Returns(eqpToolUIEFactory);
        IEquippableItemIconAdaptor eqpIIUIA = Substitute.For<IEquippableItemIconAdaptor>();
        thisArg.uia.Returns(eqpIIUIA);
        IItemIconImage itemIconImage = Substitute.For<IItemIconImage>();
        thisArg.image.Returns(itemIconImage);
        IEquippableIITAManager eqpIITAM = Substitute.For<IEquippableIITAManager>();
        thisArg.iiTAM.Returns(eqpIITAM);
        IEquippableUIItem eqpItem = Substitute.For<IEquippableUIItem>();
        thisArg.item.Returns(eqpItem);
        IEqpIITransactionStateEngine eqpIITAStateEngine = Substitute.For<IEqpIITransactionStateEngine>();
        thisArg.iiTAStateEngine.Returns(eqpIITAStateEngine);
        IItemIconEmptinessStateEngine emptinessStateEngine = Substitute.For<IItemIconEmptinessStateEngine>();
        thisArg.emptinessStateEngine.Returns(emptinessStateEngine);
        IItemIconPickUpImplementor pickUpImplementor = Substitute.For<IItemIconPickUpImplementor>();
        thisArg.iiPickUpImplementor.Returns(pickUpImplementor);

        arg = thisArg;
        
        TestEqpII eqpII = new TestEqpII(thisArg);
        return eqpII;
    }
    public TestEqpII CreateTestEqpII(System.Type tempType, out IEquippableItemIconConstArg arg){
        IEquippableItemIconConstArg thisArg;
        TestEqpII testEqpII = CreateTestEqpII(out thisArg);
        IEquippableUIItem item = (IEquippableUIItem)thisArg.item;
        if(tempType == typeof(IBowTemplate))
            item.GetItemTemplate().Returns(Substitute.For<IBowTemplate>());
        else if (tempType == typeof(IWearTemplate))
            item.GetItemTemplate().Returns(Substitute.For<IWearTemplate>());
        else if(tempType == typeof(ICarriedGearTemplate))
            item.GetItemTemplate().Returns(Substitute.For<ICarriedGearTemplate>());
        else
            throw new System.ArgumentException("tempType must be of type IItemTemplate");
        
        arg = thisArg;
        return testEqpII;
    }
    public TestEqpII CreateTestEqpII(int quantity, System.Type tempType, out IEquippableItemIconConstArg arg){
        IEquippableItemIconConstArg thisArg;
        TestEqpII testEqpII = CreateTestEqpII(tempType, out thisArg);
        IEquippableUIItem eqpItem = (IEquippableUIItem)thisArg.item;
        eqpItem.GetQuantity().Returns(quantity);

        arg = thisArg;
        return testEqpII;
    }
    public TestEqpII CreateTestEqpII(bool isInEqpIG, int quantity, System.Type tempType, out IEquippableItemIconConstArg arg){
        IEquippableItemIconConstArg thisArg;
        TestEqpII testEqpII = CreateTestEqpII(quantity, tempType, out thisArg);
        if(isInEqpIG)
            testEqpII.SetIconGroup(Substitute.For<IEquipToolEquipIG>());
        else
            testEqpII.SetIconGroup(Substitute.For<IEquipToolPoolIG>());
        
        arg = thisArg;
        return testEqpII;
    }
    public TestEqpII CreateTestEqpII(bool isStackable, bool isInEqpIG, int quantity, System.Type tempType, out IEquippableItemIconConstArg arg){
        IEquippableItemIconConstArg thisArg;
        TestEqpII testEqpII = CreateTestEqpII(isInEqpIG,quantity, tempType, out thisArg);
        if(isStackable)
            thisArg.item.GetItemTemplate().IsStackable().Returns(true);
        else
            thisArg.item.GetItemTemplate().IsStackable().Returns(false);
        
        arg = thisArg;
        return testEqpII;
    }
    public TestEqpII CreateTestEqpIIWithPickability(bool isPicked, out IEquippableItemIconConstArg arg){
        IEquippableItemIconConstArg thisArg;
        TestEqpII testEqpII = CreateTestEqpII(out thisArg);
        IEqpIITransactionStateEngine eqpIITAStateEngine = (IEqpIITransactionStateEngine)thisArg.iiTAStateEngine;
        eqpIITAStateEngine.IsPicked().Returns(isPicked);

        arg = thisArg;
        return testEqpII;
    }
    public class TestEqpII: EquippableItemIcon{
        public TestEqpII(IEquippableItemIconConstArg arg): base(arg){}
        public void SetIconGroup(IIconGroup ig){
            thisIG = ig;
        }
        public void TestCheckForPickUp(){
            this.CheckForPickUp();
        }
        public bool TestIsEligibleForQuickDrop(){
            return this.IsEligibleForQuickDrop();
        }
        public bool TestIsEligibleForHover(IItemIcon pickedII){
            return this.IsEligibleForHover(pickedII);
        }
    }
}
