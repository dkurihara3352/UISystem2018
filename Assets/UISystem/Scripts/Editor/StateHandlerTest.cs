using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using NSubstitute;
using UISystem;
public class StateHandlerTest {
    [Test]
    public void SelectabilityStateEngine_WhenCreated_IsSetWithStates(){
        TestSelStateEngineConstArg arg;
        TestSelectabilityStateEngine engine = CreateTestSelectabilityStateEngine(out arg);

        Assert.That(engine.StatesAreAllSet(), Is.True);
    }
    /* Test Support Classes */
        class TestSelectabilityStateEngine: SelectabilityStateEngine{
            public TestSelectabilityStateEngine(IUIElement uie, IProcessFactory procFac): base(uie, procFac){}
            public bool StatesAreAllSet(){
                return this.selectableState != null && this.unselectableState != null && this.selectedState != null;
            }
        }
        TestSelectabilityStateEngine CreateTestSelectabilityStateEngine(out TestSelStateEngineConstArg arg){
            IUIElement uie = Substitute.For<IUIElement>();
                IUIImage image = Substitute.For<IUIImage>();
                uie.GetUIImage().Returns(image);
            IProcessFactory procFac = Substitute.For<IProcessFactory>();
                ITurnImageDarknessProcess turnImageDarknessProcess = Substitute.For<ITurnImageDarknessProcess>();
                procFac.CreateTurnImageDarknessProcess(image, Arg.Any<float>()).Returns(turnImageDarknessProcess);
            TestSelectabilityStateEngine engine = new TestSelectabilityStateEngine(uie, procFac);
            TestSelStateEngineConstArg thisArg = new TestSelStateEngineConstArg();
            arg = thisArg;
            return engine;
        }
        class TestSelStateEngineConstArg{
            public TestSelStateEngineConstArg(){}
        }
}
