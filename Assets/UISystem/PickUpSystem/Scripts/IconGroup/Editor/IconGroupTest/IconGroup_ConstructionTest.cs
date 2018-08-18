using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NSubstitute;
using NUnit.Framework;
using UISystem;
using UISystem.PickUpUISystem;

[TestFixture, Category("PickUpSystem"), Ignore]
public class IconGroup_ConstructionTest: IconGroupTest{
    [Test][ExpectedException(typeof(System.InvalidOperationException))]
	public void Construction_MaxSizeInvalid_ThrowsException([Values(0, -1, -100)]int maxSize){
		IIconGroupConstArg arg;
		TestIG testIG = CreateTestIG(0, maxSize, out arg);
	}
	[Test][ExpectedException(typeof(System.InvalidOperationException))][TestCaseSource(typeof(ConstructionTestCases),"cases")]
	public void Construction_MaxSizeEqualOrLesserThanMinSize_ThrowsException(int minSize, int maxSize){
		IIconGroupConstArg arg;
		TestIG testIG = CreateTestIG(minSize, maxSize, out arg);
	}
	public class ConstructionTestCases{
		public static object[] cases = {
			new object[]{ 2, 1},
			new object[]{ 101, 100},
			new object[]{1, 0}
		};
	}
}
