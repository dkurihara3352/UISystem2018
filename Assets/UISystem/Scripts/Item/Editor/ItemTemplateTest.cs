using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using NSubstitute;
using UISystem;

[TestFixture, Category("UISystem")]
public class ItemTemplateTest{
    [Test, ExpectedException(typeof(System.InvalidOperationException)), TestCaseSource(typeof(Construction_TestCases), "cases")]
    public void Construction_AnyOfQuantityIsLessThanOne_ThrowsException(int pickUpStepQ, int maxEquippableQ, int maxQPerSlot){
        IItemTemplateConstArg arg;
        TestItemTemplate testItemTemp = CreateTestItemTemplate(pickUpStepQ, maxEquippableQ, maxQPerSlot, out arg);
    }
    public class Construction_TestCases{
        public static object[] cases = {
            new object[]{0, 1, 1},
            new object[]{1, 0, 1},
            new object[]{1, 1, 0},
        };
    }
    [Test]
    public void IsSameAs_ThisReferenceEqualsOther_ReturnsTrue(){
        IItemTemplateConstArg arg;
        TestItemTemplate testItemTemplate = CreateTestItemTemplate(1, 1, 1, out arg);
        TestItemTemplate other = testItemTemplate;
        Assert.That(Object.ReferenceEquals(testItemTemplate, other), Is.True);

        bool actualBool = testItemTemplate.IsSameAs(other);

        Assert.That(actualBool, Is.True);
    }
    [Test]
    public void IsSameAs_ThisNotReferenceEqualsOther_ReturnsFalse(){
        IItemTemplateConstArg arg;
        TestItemTemplate testItemTemplate = CreateTestItemTemplate(1, 1, 1, out arg);
        TestItemTemplate other = new TestItemTemplate(arg);
        Assert.That(Object.ReferenceEquals(testItemTemplate, other), Is.False);

        bool actualBool = testItemTemplate.IsSameAs(other);

        Assert.That(actualBool, Is.False);
    }
    public class TestItemTemplate: AbsItemTemplate{
        public TestItemTemplate(IItemTemplateConstArg arg): base(arg){}
    }
    public TestItemTemplate CreateTestItemTemplate(int pickUpStepQ, int maxEquippableQ, int maxQPerSlot, out IItemTemplateConstArg arg){
        IItemTemplateConstArg thisArg = Substitute.For<IItemTemplateConstArg>();
        thisArg.pickupStepQuantity.Returns(pickUpStepQ);
        thisArg.maxEquippableQuantity.Returns(maxEquippableQ);
        thisArg.maxQuantityPerSlot.Returns(maxQPerSlot);
        TestItemTemplate testItemTemplate = new TestItemTemplate(thisArg);

        arg = thisArg;
        return testItemTemplate;
    }
}
