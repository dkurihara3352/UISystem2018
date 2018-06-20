using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using NSubstitute;
using UISystem;

[TestFixture]
public class ItemIconPickUpImplementorTest{
    [Test, TestCaseSource(typeof(SetUpAsPickedII_TestCases), "greaterCases")]
    public void SetUpAsPickedII_ThisQuantityGreaterThanPickedQuantity_CallsIITAMCreateItemIcon(int transferableQuantity, int itemQuantity, int pickUpStepQuantity){
        IItemIcon thisII;
        IItemIconTransactionManager thisIITAM;
        ItemIconPickUpImplementor implementor = CreateIIPUImplementor(transferableQuantity, itemQuantity, pickUpStepQuantity, out thisII, out thisIITAM);

        implementor.SetUpAsPickedII();

        AssertIITAMCreateItemIconIsCalled(thisII, thisIITAM, true);
    }
    [Test, TestCaseSource(typeof(SetUpAsPickedII_TestCases), "notGreaterCases")]
    public void SetUpAsPickedII_ThisQuantityNotGreaterThanPickedQuantity_ThisIILeavesGhost_CallsIITAMCreateItemIcon(int transferableQuantity, int itemQuantity, int pickUpStepQuantity){
        IItemIcon thisII;
        IItemIconTransactionManager thisIITAM;
        ItemIconPickUpImplementor implementor = CreateIIPUImplementor(transferableQuantity, itemQuantity, pickUpStepQuantity, out thisII, out thisIITAM);
        thisII.LeavesGhost().Returns(true);

        implementor.SetUpAsPickedII();

        AssertIITAMCreateItemIconIsCalled(thisII, thisIITAM, true);
    }
    [Test, TestCaseSource(typeof(SetUpAsPickedII_TestCases), "notGreaterCases")]
    public void SetUpAsPickedII_ThisQuantityNotGreaterThanPickedQuantity_ThisIIDoesNotLeaveGhost_DoesNotCallIITAMCreateItemIcon(int transferableQuantity, int itemQuantity, int pickUpStepQuantity){
        IItemIcon thisII;
        IItemIconTransactionManager thisIITAM;
        ItemIconPickUpImplementor implementor = CreateIIPUImplementor(transferableQuantity, itemQuantity, pickUpStepQuantity, out thisII, out thisIITAM);
        thisII.LeavesGhost().Returns(false);

        implementor.SetUpAsPickedII();

        AssertIITAMCreateItemIconIsCalled(thisII, thisIITAM, false);
    }
    [Test, TestCaseSource(typeof(SetUpAsPickedII_TestCases), "pickedQuantityNonZeroCases")]
    public void CheckAndIncrementPickUpQuantity_ThisIITAMIsNotInPickedUpState_DoesNotCallPickedIIIncreaseBy(int transferableQuantity, int itemQuantity, int pickUpStepQuantity){
        IItemIcon thisII;
        IItemIconTransactionManager thisIITAM;
        ItemIconPickUpImplementor implementor = CreateIIPUImplementor(transferableQuantity, itemQuantity, pickUpStepQuantity, out thisII, out thisIITAM);
        thisIITAM.IsInPickedUpState().Returns(false);
        IItemIcon pickedII = Substitute.For<IItemIcon>();
        thisIITAM.GetPickedII().Returns(pickedII);

        implementor.CheckAndIncrementPickUpQuantity();

        pickedII.DidNotReceive().IncreaseBy(Arg.Any<int>(), Arg.Any<bool>());
    }
    [Test, TestCaseSource(typeof(SetUpAsPickedII_TestCases), "pickedQuantityZeroCase")]
    public void CheckAndIncrementPickUpQuantity_ThisIITAMIsInPickedUpState_IncrementQNotGreaterThanZero_DoesNotCallPickedIIIncreaseBy(int transferableQuantity, int itemQuantity, int pickUpStepQuantity){
        IItemIcon thisII;
        IItemIconTransactionManager thisIITAM;
        ItemIconPickUpImplementor implementor = CreateIIPUImplementor(transferableQuantity, itemQuantity, pickUpStepQuantity, out thisII, out thisIITAM);
        thisIITAM.IsInPickedUpState().Returns(true);
        IItemIcon pickedII = Substitute.For<IItemIcon>();
        thisIITAM.GetPickedII().Returns(pickedII);

        implementor.CheckAndIncrementPickUpQuantity();

        pickedII.DidNotReceive().IncreaseBy(Arg.Any<int>(), Arg.Any<bool>());
    }
    [Test, TestCaseSource(typeof(SetUpAsPickedII_TestCases), "pickedQuantityNonZeroCases")]
    public void CheckAndIncrementPickUpQuantity_ThisIITAMIsInPickedUpState_IncrementQGreaterThanZero_CallsPickedIIIncreaseBy(int transferableQuantity, int itemQuantity, int pickUpStepQuantity){
        IItemIcon thisII;
        IItemIconTransactionManager thisIITAM;
        ItemIconPickUpImplementor implementor = CreateIIPUImplementor(transferableQuantity, itemQuantity, pickUpStepQuantity, out thisII, out thisIITAM);
        thisIITAM.IsInPickedUpState().Returns(true);
        IItemIcon pickedII = Substitute.For<IItemIcon>();
        thisIITAM.GetPickedII().Returns(pickedII);

        implementor.CheckAndIncrementPickUpQuantity();

        pickedII.Received(1).IncreaseBy(Mathf.Min(transferableQuantity, pickUpStepQuantity), true);
    }
    public class SetUpAsPickedII_TestCases{
        public static object[] greaterCases = {
            new object[]{1, 3, 5},
            new object[]{1, 2, 1},
            new object[]{0, 1, 1}
        };
        public static object[] notGreaterCases = {
            new object[]{3, 3, 5},
            new object[]{2, 1, 1},
            new object[]{100, 20, 50}
        };
        public static object[] pickedQuantityNonZeroCases = {
            new object[]{1, 3, 5},
            new object[]{1, 2, 1},
            new object[]{3, 3, 5},
            new object[]{2, 1, 1},
            new object[]{100, 20, 50}

        };
        public static object[] pickedQuantityZeroCase = {
            new object[]{0, 3, 5},
            new object[]{0, 1, 1},
            new object[]{0, 20, 50}
        };
    }
    public void AssertIITAMCreateItemIconIsCalled(IItemIcon ii, IItemIconTransactionManager iiTAM, bool expectedToReceive){
        IItemIcon leftoverII = Substitute.For<IItemIcon>();
        iiTAM.CreateItemIcon(ii.GetUIItem()).Returns(leftoverII);
        if(expectedToReceive)
            iiTAM.Received(1).CreateItemIcon(ii.GetUIItem());
        else
            iiTAM.DidNotReceive().CreateItemIcon(ii.GetUIItem());
        
    }
    public ItemIconPickUpImplementor CreateIIPUImplementor(int transferableQuantity, int itemQuantity, int pickUpStepQuantity , out IItemIcon ii, out IItemIconTransactionManager iiTAM){
        IItemIcon thisII = Substitute.For<IItemIcon>();
            thisII.GetUIImage().Returns(Substitute.For<IUIImage>());
            IItemTemplate itemTemp = Substitute.For<IItemTemplate>();
            itemTemp.GetPickUpStepQuantity().Returns(pickUpStepQuantity);
            // thisII.GetItemTemplate().Returns(itemTemp);
            IUIItem item = Substitute.For<IUIItem>();
            item.GetItemTemplate().Returns(itemTemp);
            thisII.GetUIItem().Returns(item);
            thisII.GetTransferableQuantity().Returns(transferableQuantity);
            thisII.GetItemQuantity().Returns(itemQuantity);
            thisII.GetIconGroup().Returns(Substitute.For<IIconGroup>());
        IItemIconTransactionManager thisIITAM = Substitute.For<IItemIconTransactionManager>();
        ItemIconPickUpImplementor implementor = new ItemIconPickUpImplementor(thisIITAM);
        implementor.SetItemIcon(thisII);
        ii = thisII;
        iiTAM = thisIITAM;
        return implementor;
    }
}
