using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using NSubstitute;
using UISystem;
using UISystem.PickUpUISystem;

[TestFixture, Category("PickUpSystem")]
public class IconGroupTest{
	/* Test Classes */
	public class TestIG: AbsIconGroup{
		public TestIG(IIconGroupConstArg arg): base(arg){}
		public override bool HasItemSpace(IUIItem item){
			return false;
		}
		public void SetItemIcons(List<IItemIcon> itemIcons){
			this.thisItemIcons = itemIcons;
		}
	}
	public TestIG CreateTestIG(int minSize, int maxSize, out IIconGroupConstArg arg){
		IUIManager uim = Substitute.For<IUIManager>();
		IPickUpSystemProcessFactory pickUpSystemProcessFactory = Substitute.For<IPickUpSystemProcessFactory>();
		IPickUpSystemUIElementFactory pickUpSystemUIElementFactory = Substitute.For<IPickUpSystemUIElementFactory>();
		IPickUpSystemUIA pickUpSystemUIA = Substitute.For<IPickUpSystemUIA>();
		IUIImage image = Substitute.For<IUIImage>();
		IUITool tool = Substitute.For<IUITool>();
		IItemIconTransactionManager iiTAM = Substitute.For<IItemIconTransactionManager>();
		IHoverPadsManager hoverPadsManager = Substitute.For<IHoverPadsManager>();
		List<IItemIcon> iis = new List<IItemIcon>();

		IIconGroupConstArg thisArg = new IconGroupConstArg(uim, pickUpSystemProcessFactory, pickUpSystemUIElementFactory, pickUpSystemUIA, image, tool, iiTAM, minSize, maxSize, hoverPadsManager, iis);
		TestIG testIG = new TestIG(thisArg);
		arg = thisArg;
		return testIG;
	}
}
