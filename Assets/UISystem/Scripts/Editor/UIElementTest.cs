using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using NSubstitute;
using UISystem;

// public class UIElementTest {

// 	[Test]
// 	public void UIElement_WhenCreated_IsPassedUIMAndUIAdaptor(){
// 		TestUIElementConstArg arg;
// 		TestUIElement uie = CreateTestUIElement(out arg);

// 		Assert.That(uie.GetUIM(), Is.SameAs(arg.uim));
// 		Assert.That(uie.GetUIAdaptor(), Is.SameAs(arg.uia));
// 	}
// 	[Test]
// 	public void UIElement_GetParentUIE_Returns_UIAReturnValue(){
// 		TestUIElementConstArg arg;
// 		TestUIElement uie = CreateTestUIElement(out arg);
// 		IUIAdaptor uia = arg.uia;
// 		IUIElement expected = Substitute.For<IUIElement>();
// 		uia.GetParentUIE().Returns(expected);

// 		Assert.That(uie.GetParentUIE(), Is.SameAs(expected));
// 	}
// 	[Test]
// 	public void UIElement_GetChildUIEs_Returns_UIAReturnValue(){
// 		TestUIElementConstArg arg;
// 		TestUIElement uie = CreateTestUIElement(out arg);
// 		IUIAdaptor uia = arg.uia;
// 		List<IUIElement> expected = new List<IUIElement>();
// 		uia.GetChildUIEs().Returns(expected);

// 		Assert.That(uie.GetChildUIEs(), Is.SameAs(expected));
// 	}
// 	/* Test Support Classes */
// 		class TestUIElement: AbsUIElement{
// 			public TestUIElement(IUIManager uim, IUIAdaptor uia) :base(uim, uia){
// 			}
// 			public override IUIImage CreateUIImage(){
// 				return new TestUIImage();
// 			}
// 		}
// 		class TestUIImage: IUIImage{
// 			public float GetCurrentDarkness(){return .5f;}
// 			public float GetDefaultDarkness(){return 1f;}
// 			public float GetDarkenedDarkness(){return .1f;}
// 			public void SetDarkness(float darkness){return;}
// 		}
// 		TestUIElement CreateTestUIElement(out TestUIElementConstArg arg){
// 			IUIManager uim = Substitute.For<IUIManager>();
// 				IProcessFactory procFac = Substitute.For<IProcessFactory>();
// 					ITurnImageDarknessProcess turnImageDarknessProcess = Substitute.For<ITurnImageDarknessProcess>();
// 					procFac.CreateTurnImageDarknessProcess(Arg.Any<IUIImage>(), Arg.Any<float>()).Returns(turnImageDarknessProcess);
// 				uim.GetProcessFactory().Returns(procFac);
// 			IUIAdaptor uia = Substitute.For<IUIAdaptor>();
// 			TestUIElementConstArg thisArg = new TestUIElementConstArg(uim, uia, procFac);

// 			TestUIElement uie = new TestUIElement(uim, uia);
// 			arg = thisArg;
// 			return uie;
// 		}
// 		class TestUIElementConstArg{
// 			public IUIManager uim;
// 			public IUIAdaptor uia;
// 			public IProcessFactory procFac;
// 			public TestUIElementConstArg(IUIManager uim, IUIAdaptor uia, IProcessFactory procFac){
// 				this.uim = uim;
// 				this.uia = uia;
// 				this.procFac = procFac;
// 			}
// 		}
// }
