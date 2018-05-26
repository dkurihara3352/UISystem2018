using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using NSubstitute;
using UISystem;

public class UIElementTest {

	[Test]
	public void UIElement_Instantiation(){
		IUIElement uie = new UIElement();
		Assert.That(uie, Is.Not.Null);
	}
	[Test]
	public void UIElement_SayHi_WhenCalled_PrintHi(){
		IUIElement uie = new UIElement();
		string result = uie.SayHi();
		Assert.That(result, Is.Null);
	}
}
