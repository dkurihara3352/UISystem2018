using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using NSubstitute;
using UISystem;

[TestFixture]
public class ItemIconTest{
    [Test, TestCaseSource(typeof(UpdateTransferableQuantity_TestCases), "validCases")]
    public void UpdateTransferableQuantity_DiffAtLeastZero_SetsDiffAsTransferableQuantity(int pickedQuantity, int maxTransferableQuantity){
        IItemIconConstArg arg;
        TestItemIcon testItemIcon = CreateTestItemIcon(maxTransferableQuantity, out arg);
        testItemIcon.UpdateTransferableQuantity(pickedQuantity);

        int actualQ = testItemIcon.GetTransferableQuantity();
        int expectedQ = maxTransferableQuantity - pickedQuantity;

        Assert.That(actualQ, Is.EqualTo(expectedQ));
    }
    [Test][ExpectedException(typeof(System.InvalidOperationException))][TestCaseSource(typeof(UpdateTransferableQuantity_TestCases), "invalidCases")]
    public void UpdateTransferableQuantity_PickedQExceedsMaxTransQ_ThrowsException(int pickedQuantity, int maxTransferableQuantity){
        IItemIconConstArg arg;
        TestItemIcon testItemIcon = CreateTestItemIcon(maxTransferableQuantity, out arg);
        testItemIcon.UpdateTransferableQuantity(pickedQuantity);
    }
    public class UpdateTransferableQuantity_TestCases{
        public static object[] validCases = {
            new object[]{1, 2},
            new object[]{12, 26},
            new object[]{0, 10},
            new object[]{3, 3}
        };
        public static object[] invalidCases = {
            new object[]{1, 0},
            new object[]{2, 1}
        };
    }
    [Test, TestCaseSource(typeof(IsTransferable_TestCases), "greaterCases")]
    public void IsTransferable_DiffGreaterThanZero_ReturnsTrue(int pickedQuantity, int maxTransferableQuantity){
        IItemIconConstArg arg;
        TestItemIcon testItemIcon = CreateTestItemIcon(maxTransferableQuantity, out arg);
        testItemIcon.UpdateTransferableQuantity(pickedQuantity);

        Assert.That(testItemIcon.IsTransferable(), Is.True);
    }
    [Test, TestCaseSource(typeof(IsTransferable_TestCases), "equalCases")]
    public void IsTransferable_PickedQEqualsToMaxTransQ_ReturnsFalse(int pickedQuantity, int maxTransferableQuantity){
        IItemIconConstArg arg;
        TestItemIcon testItemIcon = CreateTestItemIcon(maxTransferableQuantity, out arg);
        testItemIcon.UpdateTransferableQuantity(pickedQuantity);

        Assert.That(testItemIcon.IsTransferable(), Is.False);
    }
    [Test]
    public void EvaluatePickability_ThisIsEmpty_CallsEngineBecomeUnpickable(){
        IItemIconConstArg arg;
        IItemIcon itemIcon = CreateTestItemIcon(0, out arg);
        IItemIconTransactionStateEngine iiTAStateEngine = arg.iiTAStateEngine;
        IItemIconEmptinessStateEngine emptinessStateEngine = arg.emptinessStateEngine;
        emptinessStateEngine.IsEmpty().Returns(true);
        Assert.That(itemIcon.IsEmpty(), Is.True);

        itemIcon.EvaluatePickability();

        iiTAStateEngine.Received(1).BecomeUnpickable();
    }
    [Test]
    public void EvaluatePickability_ThisIsNotEmpty_ThisIsNotReorderableNorIsTransferable_CallsEngineBecomeUnpickable(){
        IItemIconConstArg arg;
        TestItemIcon itemIcon = CreateTestItemIcon(isEmpty: false, isReorderable: false, isTransferable: false, arg: out arg);
        IItemIconTransactionStateEngine iiTAStateEngine = arg.iiTAStateEngine;

        itemIcon.EvaluatePickability();

        iiTAStateEngine.DidNotReceive().BecomePickable();
        iiTAStateEngine.Received(1).BecomeUnpickable();
    }
    [Test]
    public void EvaluatePickability_ThisIsNotEmpty_ThisIsReorderable_ThisIsNotIsTransferable_CallsEngineBecomePickable(){
        IItemIconConstArg arg;
        TestItemIcon itemIcon = CreateTestItemIcon(isEmpty: false, isReorderable: true, isTransferable: false, arg: out arg);
        IItemIconTransactionStateEngine iiTAStateEngine = arg.iiTAStateEngine;

        itemIcon.EvaluatePickability();

        iiTAStateEngine.Received(1).BecomePickable();
        iiTAStateEngine.DidNotReceive().BecomeUnpickable();
    }
    [Test]
    public void EvaluatePickability_ThisIsNotEmpty_ThisIsNotReorderable_ThisIsTransferable_CallsEngineBecomePickable(){
        IItemIconConstArg arg;
        TestItemIcon itemIcon = CreateTestItemIcon(isEmpty: false, isReorderable: false, isTransferable: true, arg: out arg);
        IItemIconTransactionStateEngine iiTAStateEngine = arg.iiTAStateEngine;

        itemIcon.EvaluatePickability();

        iiTAStateEngine.Received(1).BecomePickable();
        iiTAStateEngine.DidNotReceive().BecomeUnpickable();
    }
    [Test]
    public void HandOverTravel_IrperNotNull_CallsTravelProcessUpdateTravellingUIEFromThisToOther(){
        IItemIconConstArg arg;
        TestItemIcon itemIcon = CreateTestItemIconWithIG(0, out arg);
        ITravelProcess process = Substitute.For<ITravelProcess>();
        itemIcon.SetRunningTravelProcess(process);
        IItemIcon other = Substitute.For<IItemIcon>();

        itemIcon.HandOverTravel(other);

        process.Received(1).UpdateTravellingUIEFromTo(itemIcon, other);
    }
    [Test]
    public void HandOverTravel_IrperNotNull_CallsProcessUpdateTravellingII(){
        IItemIconConstArg arg;
        TestItemIcon itemIcon = CreateTestItemIconWithIG(0, out arg);
        ITravelProcess process = Substitute.For<ITravelProcess>();
        itemIcon.SetRunningTravelProcess(process);
        IItemIcon other = Substitute.For<IItemIcon>();

        itemIcon.HandOverTravel(other);

        process.Received(1).UpdateTravellingUIEFromTo(itemIcon, other);
    }
    public class IsTransferable_TestCases{
        public static object[] greaterCases = {
            new object[]{1, 2},
            new object[]{10, 11},
            new object[]{0, 1}
        };
        public static object[] equalCases = {
            new object[]{0, 0},
            new object[]{2, 2},
            new object[]{1, 1}
        };
    }
    public class TestItemIcon: AbsItemIcon{
        public TestItemIcon(int maxTransferableQuantity, IItemIconConstArg arg): base(arg){  
            thisMaxTransferableQuantity = maxTransferableQuantity;
        }
        public void SetIconGroup(IIconGroup ig){
            thisIG = ig;
        }
        protected  override bool IsEligibleForHover(IItemIcon pickedII){
            return false;
        }
        public override bool LeavesGhost(){
            return false;
        }
        protected override int GetMaxTransferableQuantity(){
            return thisMaxTransferableQuantity;
        }
        int thisMaxTransferableQuantity;
        public override bool HasSameItem(IItemIcon other){
            return false;
        }
        public override void CheckForHover(){}
        public override void CheckForDelayedPickUp(){}
        public override void CheckForSecondTouchPickUp(){}
        public override void CheckForDelayedDrop(){}
        public override void CheckForImmediatePickUp(){}
        public override void CheckForQuickDrop(){}
        public override void CheckForDragPickUp(ICustomEventData eventData){}
    }
    public TestItemIcon CreateTestItemIcon(int maxTransferableQuantity, out IItemIconConstArg arg){
        IItemIconConstArg thisArg = Substitute.For<IItemIconConstArg>();
        IUIManager uim = Substitute.For<IUIManager>();
        thisArg.uim.Returns(uim);
        IItemIconUIAdaptor iiUIA = Substitute.For<IItemIconUIAdaptor>();
        thisArg.uia.Returns(iiUIA);
        IUIImage image = Substitute.For<IUIImage>();
        thisArg.image.Returns(image);
        IItemIconTransactionManager iiTAM = Substitute.For<IItemIconTransactionManager>();
        thisArg.iiTAM.Returns(iiTAM);
        IUIItem item = Substitute.For<IUIItem>();
        thisArg.item.Returns(item);
        IItemIconTransactionStateEngine iiTAStateEngine = Substitute.For<IItemIconTransactionStateEngine>();
        thisArg.iiTAStateEngine.Returns(iiTAStateEngine);
        IItemIconEmptinessStateEngine emptinessStateEngine = Substitute.For<IItemIconEmptinessStateEngine>();
        thisArg.emptinessStateEngine.Returns(emptinessStateEngine);

        TestItemIcon testItemIcon = new TestItemIcon(maxTransferableQuantity, thisArg);

        arg = thisArg;
        return testItemIcon;
    }
    public TestItemIcon CreateTestItemIcon(bool isEmpty, bool isReorderable, bool isTransferable, out IItemIconConstArg arg){
        IItemIconConstArg thisArg;
        TestItemIcon itemIcon = CreateTestItemIcon(isTransferable? 2 : 0, out thisArg);
        IItemIconEmptinessStateEngine emptinessStateEngine = thisArg.emptinessStateEngine;
        emptinessStateEngine.IsEmpty().Returns(isEmpty);
        IIconGroup ig = Substitute.For<IIconGroup>();
        itemIcon.SetIconGroup(ig);
            ig.GetSize().Returns(2);
        if(isReorderable)
            ig.AllowsInsert().Returns(true);
        else
            ig.AllowsInsert().Returns(false);
        if(isTransferable)
            itemIcon.UpdateTransferableQuantity(1);
        else
            itemIcon.UpdateTransferableQuantity(0);
        
        arg = thisArg;
        return itemIcon;
    }
    public TestItemIcon CreateTestItemIconWithIG(int maxTransferableQuantity, out IItemIconConstArg arg){
        IItemIconConstArg thisArg;
        TestItemIcon itemIcon = CreateTestItemIcon(maxTransferableQuantity, out thisArg);
        IIconGroup ig = Substitute.For<IIconGroup>();
        itemIcon.SetIconGroup(ig);
        
        arg = thisArg;
        return itemIcon;
    }
}
