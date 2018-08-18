using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using NSubstitute;
using UISystem;
using UISystem.PickUpUISystem;

[TestFixture, Category("PickUpSystem"), Ignore]
public class EquipToolIconGroupTest{
    [Test]
    public void EquipToolPoolIG_HasItemSpace_ValidTypeIsPassed_ReturnsTrue(){
        IIconGroupConstArg arg;
        EqpToolPoolIG eqpToolPoolIG = CreateEqpToolIG(2, 10, out arg);
        IEquippableUIItem eqpItem = Substitute.For<IEquippableUIItem>();

        bool actualBool = eqpToolPoolIG.HasItemSpace(eqpItem);

        Assert.That(actualBool, Is.True);
    }
    [Test, TestCaseSource(typeof(EqpToolIGHasItemSpaceTestCases), "bowCases")]
    public void EqpToolEqpBowIG_HasItemSpace_Various_ReturnsAccordingly(System.Type tempType, bool isEquipped, bool expectedBool){
        IIconGroupConstArg arg;
        EqpToolEqpBowIG eqpBowIG = CreateEqpToolEqpBowIG(out arg);
        IEquippableUIItem item = CreateStubItemFromTempTypeAndEquippedness(tempType, isEquipped);
        bool actualBool = eqpBowIG.HasItemSpace(item);
        
        Assert.That(actualBool, Is.EqualTo(expectedBool));
    }
    [Test, TestCaseSource(typeof(EqpToolIGHasItemSpaceTestCases), "wearCases")]
    public void EqpToolEqpWearIG_HasItemSpace_Various_ReturnsAccordingly(System.Type tempType, bool isEquipped, bool expectedBool){
        IIconGroupConstArg arg;
        EqpToolEqpWearIG eqpWearIG = CreateEqpToolEqpWearIG(out arg);
        IEquippableUIItem item = CreateStubItemFromTempTypeAndEquippedness(tempType, isEquipped);
        bool actualBool = eqpWearIG.HasItemSpace(item);

        Assert.That(actualBool, Is.EqualTo(expectedBool));
    }
    [Test][TestCaseSource(typeof(EqpToolIGHasItemSpaceTestCases), "cgInvalidCases")]
    public void EqpToolEqpCarriedGearIG_HasItemSpace_WithItemSpace_ItemTempNotCG_RetunsFalse(int quantityInIG, int maxEqpQ, System.Type type){
        IIconGroupConstArg arg;
        TestEqpToolEqpCGIG ig = CreateEqpToolEqpCarriedGearIG(0, 4, out arg);
        IEquippableUIItem item = CreateItemWithMaxEqpQAndTemplate(maxEqpQ, type);
        List<IItemIcon> iis = CreateStubIIsWithItemQuantity(size: 4, quantInIG: quantityInIG, item:item);
        ig.SetItemIcons(iis);

        bool actualBool = ig.HasItemSpace(item);

        Assert.That(actualBool, Is.False);
    }
    [Test, TestCaseSource(typeof(EqpToolIGHasItemSpaceTestCases), "cgValidCases")]
    public void EqpToolEqpCarriedGearIG_HasItemSpace_WithItemSpace_ItemTempTypeIsCG_ReturnsTrue(int quantInIG, int maxEqpQ){
        IIconGroupConstArg arg;
        TestEqpToolEqpCGIG ig = CreateEqpToolEqpCarriedGearIG(0, 4, out arg);
        IEquippableUIItem item = CreateItemWithMaxEqpQAndTemplate(maxEqpQ, typeof(ICarriedGearTemplate));
        List<IItemIcon> iis = CreateStubIIsWithItemQuantity(size: 4, quantInIG: quantInIG, item:item);
        ig.SetItemIcons(iis);

        bool actualBool = ig.HasItemSpace(item);

        Assert.That(actualBool, Is.True);
    }
    IEquippableUIItem CreateItemWithMaxEqpQAndTemplate(int maxEqpQ, System.Type type){
        IEquippableUIItem eqpItem = Substitute.For<IEquippableUIItem>();
        eqpItem.GetMaxEquippableQuantity().Returns(maxEqpQ);
        if(type == typeof(IBowTemplate))
            eqpItem.GetItemTemplate().Returns(Substitute.For<IBowTemplate>());
        else if(type == typeof(IWearTemplate))
            eqpItem.GetItemTemplate().Returns(Substitute.For<IWearTemplate>());
        else
            eqpItem.GetItemTemplate().Returns(Substitute.For<ICarriedGearTemplate>());
        return eqpItem;
    }
    List<IItemIcon> CreateStubIIsWithItemQuantity(int size, int quantInIG, IUIItem item){
        List<IItemIcon> result = new List<IItemIcon>();
        for(int i = 0; i < size; i++){
            result.Add(Substitute.For<IItemIcon>());
        }
        if(quantInIG != 0){
            IItemIcon firstOne = result[0];
            IUIItem sameItem = Substitute.For<IUIItem>();
            sameItem.IsSameAs(item).Returns(true);
            sameItem.GetQuantity().Returns(quantInIG);
            firstOne.GetUIItem().Returns(sameItem);
        }
        return result;
    }
    IEquippableUIItem CreateStubItemFromTempTypeAndEquippedness(System.Type tempType, bool isEquipped){
        IEquippableUIItem item = Substitute.For<IEquippableUIItem>();
        if(tempType == typeof(IBowTemplate)){
            IBowTemplate bowTemp = Substitute.For<IBowTemplate>();
            item.GetItemTemplate().Returns(bowTemp);
        }else if(tempType == typeof(IWearTemplate)){
            IWearTemplate wearTemp = Substitute.For<IWearTemplate>();
            item.GetItemTemplate().Returns(wearTemp);
        }else {
            ICarriedGearTemplate carriedGearTemp = Substitute.For<ICarriedGearTemplate>();
            item.GetItemTemplate().Returns(carriedGearTemp);
        }
        if(isEquipped)
            item.IsEquipped().Returns(true);
        else
            item.IsEquipped().Returns(false);
        return item;
    }
    public class EqpToolIGHasItemSpaceTestCases{
        public static object[] bowCases = {
            new object[]{
                typeof(IBowTemplate), false, true
            },
            new object[]{
                typeof(IBowTemplate), true, false
            },
            new object[]{
                typeof(IWearTemplate), false, false
            },
            new object[]{
                typeof(IWearTemplate), true, false
            },
            new object[]{
                typeof(ICarriedGearTemplate), false, false
            },
            new object[]{
                typeof(ICarriedGearTemplate), true, false
            }
        };
        public static object[] wearCases = {
            new object[]{
                typeof(IBowTemplate), false, false
            },
            new object[]{
                typeof(IBowTemplate), true, false
            },
            new object[]{
                typeof(IWearTemplate), false, true
            },
            new object[]{
                typeof(IWearTemplate), true, false
            },
            new object[]{
                typeof(ICarriedGearTemplate), false, false
            },
            new object[]{
                typeof(ICarriedGearTemplate), true, false
            }
        };
        public static object[] cgInvalidCases = {
            new object[]{
                0, 1, typeof(IBowTemplate)
            },
            new object[]{
                1, 2, typeof(IBowTemplate)
            },
            new object[]{
                0, 1, typeof(IWearTemplate)
            },
            new object[]{
                1, 2, typeof(IWearTemplate)
            }
        };
        public static object[] cgValidCases = {
            new object[]{0, 1},
            new object[]{1, 2},
            new object[]{3, 10},
        };
    }
    [Test, TestCaseSource(typeof(EqpToolEqpCarriedGearIG_GetDefaultTATargetEqpII_TestCases), "cases")]
    public void EqpToolEqpCarriedGearIG_GetDefaultTATargetEqpII_HasSameItem_ReturnsIt(int[] sameAt){
        IIconGroupConstArg arg;
        TestEqpToolEqpCGIG ig = CreateEqpToolEqpCarriedGearIG(0, 4, out arg);
        IEquippableItemIcon pickedEqpII = Substitute.For<IEquippableItemIcon>();
        IEquippableUIItem pickedEqpItem = Substitute.For<IEquippableUIItem>();
        pickedEqpII.GetUIItem().Returns(pickedEqpItem);
        List<IItemIcon> iisContainingSameItem = CreateStubEqpIIsWithSameItemAt(4 ,pickedEqpItem, sameAt);
        ig.SetItemIcons(iisContainingSameItem);

        IEquippableItemIcon actualEqpII = ig.GetDefaultTATargetEqpII(pickedEqpII);
        IItemIcon expectedII = iisContainingSameItem[sameAt[0]];

        Assert.That(actualEqpII, Is.SameAs(expectedII));
    }
    public class EqpToolEqpCarriedGearIG_GetDefaultTATargetEqpII_TestCases{
        public static object[] cases = {
            new object[]{new int[]{0}},
            new object[]{new int[]{1}},
            new object[]{new int[]{2}},
            new object[]{new int[]{3}}
        };
        public static object[] cases2 = {
            new object[]{
                new int[]{0}
            },
            new object[]{
                new int[]{3}
            },
            new object[]{
                new int[]{1,2}
            },
            new object[]{
                new int[]{0, 1, 2, 3}
            }
        };
    }
    List<IItemIcon> CreateStubEqpIIsWithSameItemAt(int size, IUIItem sourceItem ,int[] sameAt){
        List<IItemIcon> result = new List<IItemIcon>();
        for(int i =0; i < size; i ++){

            IEquippableItemIcon tarII = Substitute.For<IEquippableItemIcon>();
            IUIItem tarItem = Substitute.For<IUIItem>();
            tarII.GetUIItem().Returns(tarItem);

            bool contained = false;
            foreach(int j in sameAt){
                if(j == i)
                    contained = true;
            }
            if(contained){
                tarItem.IsSameAs(sourceItem).Returns(true);
            }else{
                tarItem.IsSameAs(sourceItem).Returns(false);
            }

            result.Add(tarII);
        }
        return result;
    }
    [Test, TestCaseSource(typeof(EqpToolEqpCarriedGearIG_GetDefaultTATargetEqpII_TestCases), "cases2")]
    public void EqpToolEqpCarriedGearIG_GetDefaultTATargetEqpII_HasNoSameItem_HasEmpty_ReturnsFirstEmpty(int[] emptyAt){
        IIconGroupConstArg arg;
        TestEqpToolEqpCGIG ig = CreateEqpToolEqpCarriedGearIG(0, 4, out arg);
        IEquippableItemIcon pickedEqpII = Substitute.For<IEquippableItemIcon>();
        IEquippableUIItem pickedEqpItem = Substitute.For<IEquippableUIItem>();
        pickedEqpII.GetUIItem().Returns(pickedEqpItem);
        List<IItemIcon> iis = CreateStubIIsWithNoMatchAndEmptyAt(4, pickedEqpItem, emptyAt);
        ig.SetItemIcons(iis);

        IEquippableItemIcon actualEqpII = ig.GetDefaultTATargetEqpII(pickedEqpII);
        IItemIcon expectedEqpII = iis[emptyAt[0]];

        Assert.That(actualEqpII, Is.SameAs(expectedEqpII));
    }
    List<IItemIcon> CreateStubIIsWithNoMatchAndEmptyAt(int size, IEquippableUIItem item, int[] emptyAt){
        List<IItemIcon> result = new List<IItemIcon>();
        for(int i = 0; i < size; i ++){
            IEquippableItemIcon tarII = Substitute.For<IEquippableItemIcon>();
            bool contained = false;
            foreach(int j in emptyAt){
                if(j == i)
                    contained = true;
            }
            if(contained){
                tarII.GetUIItem().Returns((IUIItem)null);
                tarII.IsEmpty().Returns(true);
            }else{
                IUIItem tarItem = Substitute.For<IUIItem>();
                tarII.GetUIItem().Returns(tarItem);
                tarII.IsEmpty().Returns(false);
            }
            result.Add(tarII);
        }
        return result;
    }
    /*  */
    public EqpToolPoolIG CreateEqpToolIG(int minSize, int maxSize, out IIconGroupConstArg arg){
        IIconGroupConstArg thisArg = CreateStubIconGroupConstArg(minSize, maxSize);
        EqpToolPoolIG ig = new EqpToolPoolIG(thisArg);
        arg = thisArg;
        return ig;
    }
    public EqpToolEqpBowIG CreateEqpToolEqpBowIG(out IIconGroupConstArg arg){
        IIconGroupConstArg thisArg = CreateStubIconGroupConstArg(1, 1);
        EqpToolEqpBowIG ig = new EqpToolEqpBowIG(thisArg);
        arg = thisArg;
        return ig;
    }
    public EqpToolEqpWearIG CreateEqpToolEqpWearIG(out IIconGroupConstArg arg){
        IIconGroupConstArg thisArg = CreateStubIconGroupConstArg(1, 1);
        EqpToolEqpWearIG ig = new EqpToolEqpWearIG(thisArg);
        arg = thisArg;
        return ig;
    }
    public TestEqpToolEqpCGIG CreateEqpToolEqpCarriedGearIG(int minSize, int maxSize, out IIconGroupConstArg arg){
        IIconGroupConstArg thisArg = CreateStubIconGroupConstArg(minSize, maxSize);
        TestEqpToolEqpCGIG ig = new TestEqpToolEqpCGIG(thisArg);
        arg = thisArg;
        return ig;
    }
    public class TestEqpToolEqpCGIG: EqpToolEqpCarriedGearsIG{
        public TestEqpToolEqpCGIG(IIconGroupConstArg arg): base(arg){}
        public void SetItemIcons(List<IItemIcon> iis){
            this.thisItemIcons = iis;
        }
    }
    IIconGroupConstArg CreateStubIconGroupConstArg(int minSize, int maxSize){
        IIconGroupConstArg thisArg = Substitute.For<IIconGroupConstArg>();
        IUIManager uim = Substitute.For<IUIManager>();
        IUIAdaptor uia = Substitute.For<IUIAdaptor>();
        IUIImage image = Substitute.For<IUIImage>();
        thisArg.uim.Returns(uim);
        thisArg.uia.Returns(uia);
        thisArg.image.Returns(image);
        thisArg.minSize.Returns(minSize);
        thisArg.maxSize.Returns(maxSize);

        return thisArg;
    }
}
