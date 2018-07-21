using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using NSubstitute;
using UISystem;

[TestFixture, Category("UISystem")]
public class ItemTest{
	[Test]
	public void IsSameAs_ThisItemTempIsStackable_ThisItemTempRefEqualsToOtherItemTemp_ReturnsTrue(){
		IUIItemConstArg arg;
		TestItem item = CreateTestItem(1, 0, out arg);
		IItemTemplate thisItemTemp = arg.itemTemp;
		thisItemTemp.IsStackable().Returns(true);
		IUIItem otherItem = Substitute.For<IUIItem>();
		otherItem.GetItemTemplate().Returns(thisItemTemp);

		bool actualBool = item.IsSameAs(otherItem);
		
		Assert.That(actualBool, Is.True);
	}
	[Test]
	public void IsSameAs_ThisItemTempIsStackable_ThisItemTempNotRefEqualsToOtherItemTemp_ReturnsFalse(){
		IUIItemConstArg arg;
		TestItem item = CreateTestItem(1, 0, out arg);
		IItemTemplate thisItemTemp = arg.itemTemp;
		thisItemTemp.IsStackable().Returns(true);
		IUIItem otherItem = Substitute.For<IUIItem>();
		otherItem.GetItemTemplate().Returns(Substitute.For<IItemTemplate>());

		bool actualBool = item.IsSameAs(otherItem);
		
		Assert.That(actualBool, Is.False);
	}
	[Test]
	public void IsSameAs_ThisItemTempIsNotStackable_ThisItemIDEqualsToOtherItemID_ReturnsTrue(){
		IUIItemConstArg arg;
		const int itemID = 0;
		TestItem item = CreateTestItem(1, itemID, out arg);
		IItemTemplate thisItemTemp = arg.itemTemp;
		thisItemTemp.IsStackable().Returns(false);
		IUIItem otherItem = Substitute.For<IUIItem>();
		otherItem.GetItemID().Returns(itemID);

		bool actualBool = item.IsSameAs(otherItem);
		
		Assert.That(actualBool, Is.True);
	}
	[Test]
	public void IsSameAs_ThisItemTempIsNotStackable_ThisItemIDNotEqualsToOtherItemID_ReturnsFalse(){
		IUIItemConstArg arg;
		const int itemID = 0;
		const int otherID = 1;
		TestItem item = CreateTestItem(1, itemID, out arg);
		IItemTemplate thisItemTemp = arg.itemTemp;
		thisItemTemp.IsStackable().Returns(false);
		IUIItem otherItem = Substitute.For<IUIItem>();
		otherItem.GetItemID().Returns(otherID);

		bool actualBool = item.IsSameAs(otherItem);
		
		Assert.That(actualBool, Is.False);
	}
	public class TestItem: AbsUIItem{
		public TestItem(IUIItemConstArg arg): base(arg){}
	}
	public TestItem CreateTestItem(int quantity, int itemID, out IUIItemConstArg arg){
		IUIItemConstArg thisArg = Substitute.For<IUIItemConstArg>();
		IItemTemplate itemTemp = Substitute.For<IItemTemplate>();
		thisArg.itemTemp.Returns(itemTemp);
		thisArg.quantity.Returns(quantity);
		thisArg.itemID.Returns(itemID);
		TestItem testItem = new TestItem(thisArg);

		arg = thisArg;
		return testItem;
	}
}
