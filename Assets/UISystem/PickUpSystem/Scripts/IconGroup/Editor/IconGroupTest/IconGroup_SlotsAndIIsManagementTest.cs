using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using NSubstitute;
using UISystem;
using UISystem.PickUpUISystem;

[TestFixture, Category("PickUpSystem"), Ignore]
public class IconGroup_SlotsAndIIsManagementTest : IconGroupTest {
	[Test, TestCaseSource(typeof(HasSlotSpaceTestCases), "validCases")]
	public void HasSlotSpace_CurSizeLessThanMaxSize_ReturnsTrue(int iisSize, int maxSize){
		IIconGroupConstArg arg;
		TestIG testIG = CreateTestIG(iisSize, maxSize, out arg);
		List<IItemIcon> iis = CreateStubItemIcons(iisSize);
		testIG.SetItemIcons(iis);

		bool actualBool = testIG.HasSlotSpace();
		
		Assert.That(actualBool, Is.True);
	}
	List<IItemIcon> CreateStubItemIcons(int count){
		List<IItemIcon> result = new List<IItemIcon>();
		for(int i = 0; i < count; i ++)
			result.Add(Substitute.For<IItemIcon>());
		return result;
	}
	[Test][TestCaseSource(typeof(HasSlotSpaceTestCases), "validCasesV2")]
	public void HasSlotSpace_CurSizeEqualToMaxSize_SomeEmptyIIsPresent_ReturnsTrue(int iisCount, int[] emptyAt){
		IIconGroupConstArg arg;
		TestIG testIG = CreateTestIG(iisCount, iisCount, out arg);
		List<IItemIcon> iis = CreateStubItemIcons(iisCount, emptyAt);
		testIG.SetItemIcons(iis);

		bool actualBool = testIG.HasSlotSpace();

		Assert.That(actualBool, Is.True);
	}
	[Test, TestCaseSource(typeof(HasSlotSpaceTestCases), "invalidCases")]
	public void HasSlotSpace_CurSizeEqualToMasSize_NoEmtpyIIs_ReturnsFalse(int iisCount, int[] emptyAt){
		IIconGroupConstArg arg;
		TestIG testIG = CreateTestIG(iisCount, iisCount, out arg);
		List<IItemIcon> iis = CreateStubItemIcons(iisCount, emptyAt);
		testIG.SetItemIcons(iis);

		bool actualBool = testIG.HasSlotSpace();

		Assert.That(actualBool, Is.False);
	}
	public List<IItemIcon> CreateStubItemIcons(int iisCount, int[] emptyAt){
		List<IItemIcon> result = new List<IItemIcon>();
		for(int i = 0; i < iisCount; i ++){
			IItemIcon ii = Substitute.For<IItemIcon>();
			foreach(int j in emptyAt)
				if( j == i )
					ii.IsEmpty().Returns(true);
				else
					ii.IsEmpty().Returns(false);
			result.Add(ii);
		}
		return result;
	}
	public class HasSlotSpaceTestCases{
		public static object[] validCases = {
			new object[]{1, 2},
			new object[]{20, 50},
			new object[]{0, 1}
		};
		public static object[] validCasesV2 = {
			new object[]{1, new int[]{0}},
			new object[]{2, new int[]{1}},
			new object[]{5, new int[]{1, 3}}
		};
		public static object[] invalidCases = {
			new object[]{1, new int[]{}},
			new object[]{2, new int[]{}},
			new object[]{5, new int[]{}}
		};
	}
	[Test, TestCaseSource(typeof(GetItemQuantityTestCases), "cases")]
	public void GetItemQuantity_WhenCalled_ReturnsSumOfAllSameItemQuantity(int[] quantities, int[] sameAt, int expectedQuantity){
		IIconGroupConstArg arg;
		TestIG testIG = CreateTestIG(quantities.Length, quantities.Length, out arg);
		IUIItem sourceItem = Substitute.For<IUIItem>();
		List<IItemIcon> iis = CreateStubIIsWithItemsMatchingAt(quantities, sourceItem, sameAt);
		testIG.SetItemIcons(iis);

		int actualQuantity = testIG.GetItemQuantity(sourceItem);

		Assert.That(actualQuantity, Is.EqualTo(expectedQuantity));
	}
	public class GetItemQuantityTestCases{
		public static object[] cases = {
			new object[]{new int[]{10}, new int[]{}, 0},
			new object[]{new int[]{0, 5, 2}, new int[]{1}, 5},
			new object[]{new int[]{1, 1, 5, 1, 0, 0}, new int[]{0, 1, 3}, 3},
			new object[]{new int[]{1, 1, 5, 1, 0, 0}, new int[]{2}, 5},
			new object[]{new int[]{1, 1, 5, 1, 100, 0}, new int[]{2, 4}, 105}
		};
	}
	[Test][TestCaseSource(typeof(CreateStubIIsWithItemsMatchingAtTestCases), "cases")]
	public void CreateStubIIsWithItemsMatchingAt_WorksFine(int[] quantities, int[] sameAt){
		IUIItem sourceItem = Substitute.For<IUIItem>();
		List<IItemIcon> iis = CreateStubIIsWithItemsMatchingAt(quantities, sourceItem, sameAt);
		 
		Assert.That(iis.Count, Is.EqualTo(quantities.Length));
		foreach(IItemIcon ii in iis){
			if(quantities[iis.IndexOf(ii)] == -1){
				Assert.That(ii.IsEmpty(), Is.True);
				Assert.That(ii.GetUIItem(), Is.Null);
			}else{
				Assert.That(ii.IsEmpty(), Is.False);
				Assert.That(ii.GetUIItem().GetQuantity(), Is.EqualTo(quantities[iis.IndexOf(ii)]));
				bool contained = false;
				foreach(int j in sameAt){
					if(iis.IndexOf(ii) == j){
						contained = true;
					}
				}
				if(contained)
					Assert.That(ii.GetUIItem().IsSameAs(sourceItem), Is.True);
				else
					Assert.That(ii.GetUIItem().IsSameAs(sourceItem), Is.False);
			}
		}
	}
	public class CreateStubIIsWithItemsMatchingAtTestCases{
		public static object[] cases = {
			new object[]{new int[]{0, 1, 2, 3}, new int[]{1}},
			new object[]{new int[]{0, 1, 2, 3}, new int[]{}},
			new object[]{new int[]{0, 1, 2, 3}, new int[]{0, 1, 2, 3}},
			new object[]{new int[]{0, -1, 3, 1, 1, 1, -1, 5, 0}, new int[]{2, 7}},
		};
	}
	public List<IItemIcon> CreateStubIIsWithItemsMatchingAt(int[] quantities, IUIItem sourceItem, int[] sameAt){
		List<IItemIcon> result = new List<IItemIcon>();
		int index = -1;
		foreach(int i in quantities){
			index++;
			IItemIcon ii = Substitute.For<IItemIcon>();
			IUIItem tarItem = Substitute.For<IUIItem>();
			if(i == -1){
				ii.IsEmpty().Returns(true);
				ii.GetUIItem().Returns((IUIItem)null);
			}
			else{
				ii.IsEmpty().Returns(false);
				tarItem.GetQuantity().Returns(i);
				bool contained = false;
				foreach(int j in sameAt){
					if(j == index)
						contained = true;
				}
				if(contained)
					tarItem.IsSameAs(sourceItem).Returns(true);
				else
					tarItem.IsSameAs(sourceItem).Returns(false);
				ii.GetUIItem().Returns(tarItem);
			}
			result.Add(ii);
		}
		return result;
	}
	[Test,TestCaseSource(typeof(GetItemIconFromItemTestCases), "noMatchCases")]
	public void GetItemIconFromItem_WhenNoneMatches_ReturnsNull(int[] quantities, int[] sameAt){
		IIconGroupConstArg arg;
		TestIG testIG = CreateTestIG(quantities.Length, quantities.Length, out arg);
		IUIItem sourceItem = Substitute.For<IUIItem>();
		List<IItemIcon> iis = CreateStubIIsWithItemsMatchingAt(quantities, sourceItem, sameAt);
		testIG.SetItemIcons(iis);

		IItemIcon actualII = testIG.GetItemIconFromItem(sourceItem);

		Assert.That(actualII, Is.Null);
	}
	[Test, TestCaseSource(typeof(GetItemIconFromItemTestCases), "oneMatchCases")]
	public void GetItemIconFromItem_WhenOneMatches_ReturnsIt(int[] quantities, int[] sameAt){
		IIconGroupConstArg arg;
		TestIG testIG = CreateTestIG(quantities.Length, quantities.Length, out arg);
		IUIItem sourceItem = Substitute.For<IUIItem>();
		List<IItemIcon> iis = CreateStubIIsWithItemsMatchingAt(quantities, sourceItem, sameAt);
		testIG.SetItemIcons(iis);

		IItemIcon actualII = testIG.GetItemIconFromItem(sourceItem);

		Assert.That(actualII, Is.EqualTo(iis[sameAt[0]]));
	}
	[Test, TestCaseSource(typeof(GetItemIconFromItemTestCases), "multipleMatchesCases")]
	public void GetItemIconFromItem_MoreThanOneMatch_ReturnsFirstMatch(int[] quantities, int[] sameAt){
		IIconGroupConstArg arg;
		TestIG testIG = CreateTestIG(quantities.Length, quantities.Length, out arg);
		IUIItem sourceItem = Substitute.For<IUIItem>();
		List<IItemIcon> iis = CreateStubIIsWithItemsMatchingAt(quantities, sourceItem, sameAt);
		testIG.SetItemIcons(iis);

		IItemIcon actualII = testIG.GetItemIconFromItem(sourceItem);

		Assert.That(actualII, Is.EqualTo(iis[sameAt[0]]));
	}
	public class GetItemIconFromItemTestCases{
		public static object[] noMatchCases = {
			new object[]{
				new int[]{-1, 2, 1, 1, 1},
				new int[]{}
			},
			new object[]{
				new int[]{1},
				new int[]{}
			},
			new object[]{
				new int[]{0},
				new int[]{}
			},
		};
		public static object[] oneMatchCases = {
			new object[]{
				new int[]{-1, 0, 0, 1, 2, 1, 100},
				new int[]{2}
			},
			new object[]{
				new int[]{-1, 0, 0, 1, 2, 1, 100},
				new int[]{6}
			}
		};
		public static object[] multipleMatchesCases = {
			new object[]{
				new int[]{100, -1, 0, 0, 0, 1, 1, -1, 2},
				new int[]{0, 2, 5}
			},
			new object[]{
				new int[]{100, -1, 0, 0, 0, 1, 1, -1, 2},
				new int[]{5, 6, 8}
			},
		};
	}
	[Test, TestCaseSource(typeof(GetAllItemIconWithItemTemplateTestCases), "noMatchCases")]
	public void GetAllItemIconWithItemTemplate_NoMatch_ReturnsEmptyList(int[] quantities, int[] sameAt){
		IIconGroupConstArg arg;
		TestIG testIG = CreateTestIG(quantities.Length, quantities.Length, out arg);
		IItemTemplate sourceItemTemp = Substitute.For<IItemTemplate>();
		List<IItemIcon> iis = CreateStubItemIconsWithItemTempsMatchingAt(quantities, sameAt, sourceItemTemp);
		testIG.SetItemIcons(iis);

		List<IItemIcon> actualIIs = testIG.GetAllItemIconWithItemTemplate(sourceItemTemp);

		Assert.That(actualIIs, Is.Empty);
	}
	[Test, TestCaseSource(typeof(GetAllItemIconWithItemTemplateTestCases), "matchCases")]
	public void GetAllItemIconWithItemTemplate_Matches_ReturnsAllMatches(int[] quantities, int[] sameAt){
		IIconGroupConstArg arg;
		TestIG testIG = CreateTestIG(quantities.Length, quantities.Length, out arg);
		IItemTemplate sourceItemTemp = Substitute.For<IItemTemplate>();
		List<IItemIcon> iis = CreateStubItemIconsWithItemTempsMatchingAt(quantities, sameAt, sourceItemTemp);
		testIG.SetItemIcons(iis);

		List<IItemIcon> actualIIs = testIG.GetAllItemIconWithItemTemplate(sourceItemTemp);
		List<IItemIcon> expectedIIs = new List<IItemIcon>();
		foreach(int i in sameAt){
			expectedIIs.Add(iis[i]);
		}

		Assert.That(actualIIs, Is.EquivalentTo(expectedIIs));
	}
	public List<IItemIcon> CreateStubItemIconsWithItemTempsMatchingAt(int[] quantities, int[] sameAt, IItemTemplate sourceItemTemp){
		List<IItemIcon> result = new List<IItemIcon>();
		int index = -1;
		foreach(int i in quantities){
			index++;
			IItemIcon ii = Substitute.For<IItemIcon>();
			IUIItem tarItem = Substitute.For<IUIItem>();
			if(i == -1){
				ii.IsEmpty().Returns(true);
				ii.GetUIItem().Returns((IUIItem)null);
				ii.GetItemTemplate().Returns((IItemTemplate)null);
			}
			else{
				ii.IsEmpty().Returns(false);
				tarItem.GetQuantity().Returns(i);
				IItemTemplate tarItemTemp = Substitute.For<IItemTemplate>();
				tarItem.GetItemTemplate().Returns(tarItemTemp);
				bool contained = false;
				foreach(int j in sameAt){
					if(j == index)
						contained = true;
				}
				if(contained)
					tarItemTemp.IsSameAs(sourceItemTemp).Returns(true);
				else
					tarItemTemp.IsSameAs(sourceItemTemp).Returns(false);
				ii.GetUIItem().Returns(tarItem);
				ii.GetItemTemplate().Returns(tarItemTemp);
			}
			result.Add(ii);
		}
		return result;
	}
	public class GetAllItemIconWithItemTemplateTestCases{
		public static object[] noMatchCases = {
			new object[]{
				new int[]{1, 1, 0, -1, 0, 1, 1, 10, 1},
				new int[]{}
			},
			new object[]{
				new int[]{1},
				new int[]{}
			},
			new object[]{
				new int[]{0},
				new int[]{}
			},
		};
		public static object[] matchCases = {
			new object[]{
				new int[]{1, 1, 1, -1, 2, 0, 0, 1, 10, 0},
				new int[]{0, 1, 2}
			},
			new object[]{
				new int[]{1, 1, 1, -1, 2, 0, 0, 1, 10, 0},
				new int[]{4}
			},
			new object[]{
				new int[]{1, 1, 1, -1, 2, 0, 0, 1, 10, 0},
				new int[]{5, 6, 9}
			},
		};
	}
}
