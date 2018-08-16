using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using NSubstitute;
using UISystem;
using DKUtility;

[TestFixture, Category("UISystem")]
public class ProximateParentScrollerCalculatorTest {
	[Test]
	public void Calculate_NoParentUIE_ReturnsNull(){
		IUIElement uie = Substitute.For<IUIElement>();
		IUIElement parent = null;
		uie.GetParentUIE().Returns(parent);

		IProximateParentScrollerCalculator calculator = new ProximateParentScrollerCalculator(uie);

		Assert.That(calculator.Calculate(), Is.Null);
	}
	[Test]
	public void Calculate_HasNonScrollerParent_ReturnsNull(){
		IUIElement uie = Substitute.For<IUIElement>();
		IUIElement parent = Substitute.For<IUIElement>();
		IUIElement nullParent = null;

		uie.GetParentUIE().Returns(parent);
		parent.GetParentUIE().Returns(nullParent);

		IProximateParentScrollerCalculator calculator = new ProximateParentScrollerCalculator(uie);

		Assert.That(calculator.Calculate(), Is.Null);
	}
	[Test]
	public void Calculate_HasScrollerParentDirectlyAbove_ReturnsIt(){
		IUIElement uie = Substitute.For<IUIElement>();
		IScroller scrollerParent = Substitute.For<IScroller>();
		IUIElement nullParent = null;

		uie.GetParentUIE().Returns(scrollerParent);
		scrollerParent.GetParentUIE().Returns(nullParent);

		IProximateParentScrollerCalculator calculator = new ProximateParentScrollerCalculator(uie);

		Assert.That(calculator.Calculate(), Is.SameAs(scrollerParent));
	}
	[Test]
	public void Calculate_HasScrollerParentNotDirecltyAbove_ReturnsIt(){
		IUIElement uie = Substitute.For<IUIElement>();
		IUIElement nonScrollerParent = Substitute.For<IUIElement>();
		IScroller scrollerParent = Substitute.For<IScroller>();
		IUIElement nullParent = null;

		uie.GetParentUIE().Returns(nonScrollerParent);
		nonScrollerParent.GetParentUIE().Returns(scrollerParent);
		scrollerParent.GetParentUIE().Returns(nullParent);

		IProximateParentScrollerCalculator calculator = new ProximateParentScrollerCalculator(uie);

		Assert.That(calculator.Calculate(), Is.SameAs(scrollerParent));
	}
	[Test]
	public void Calculate_HasMultipleScrollerParentAbove_ReturnsTheColosest(){
		IUIElement uie = Substitute.For<IUIElement>();
		IScroller closeScrollerParent = Substitute.For<IScroller>();
		IScroller farScrollerParent = Substitute.For<IScroller>();
		IUIElement nullParent = null;

		uie.GetParentUIE().Returns(closeScrollerParent);
		closeScrollerParent.GetParentUIE().Returns(farScrollerParent);
		farScrollerParent.GetParentUIE().Returns(nullParent);

		IProximateParentScrollerCalculator calculator = new ProximateParentScrollerCalculator(uie);

		Assert.That(calculator.Calculate(), Is.SameAs(closeScrollerParent));
	}
}
