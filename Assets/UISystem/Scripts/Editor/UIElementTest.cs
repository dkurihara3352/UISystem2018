using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using NSubstitute;
using UISystem;

public class UIElementTest {

	[Test]
	public void UIElement_WhenCreated_IsPassedUIMAndUIAdaptor(){
		IUIManager uim = Substitute.For<IUIManager>();
		IUIAdaptor uia = Substitute.For<IUIAdaptor>();
		
		UIElement uie = new UIElement(uim, uia);

		Assert.That(uie.GetUIM(), Is.SameAs(uim));
		Assert.That(uie.GetUIAdaptor(), Is.SameAs(uia));
	}
	[Test]
	public void UIElement_GetParentUIE_Returns_UIAReturnValue(){
		IUIAdaptor uia = Substitute.For<IUIAdaptor>();
		IUIManager uim = Substitute.For<IUIManager>();
		UIElement uie = new UIElement(uim, uia);
		IUIElement expected = Substitute.For<IUIElement>();
		uia.GetParentUIE().Returns(expected);

		Assert.That(uie.GetParentUIE(), Is.SameAs(expected));
	}
	[Test]
	public void UIElement_GetChildUIEs_Returns_UIAReturnValue(){
		IUIManager uim = Substitute.For<IUIManager>();
		IUIAdaptor uia = Substitute.For<IUIAdaptor>();
		UIElement uie = new UIElement(uim, uia);
		List<IUIElement> expected = new List<IUIElement>();
		uia.GetChildUIEs().Returns(expected);

		Assert.That(uie.GetChildUIEs(), Is.SameAs(expected));
	}
}
