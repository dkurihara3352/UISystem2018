using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using NSubstitute;
using UISystem;
using DKUtility;

[TestFixture, Category("UISystem")]
public class AbsPointerDownInputStateTest {
    [Test]
    public void OnEnter_ClearsVelocityStack(){
        IPointerDownInputStateConstArg arg = CreateMockArg();
        TestAbsPointerDownInputState state = new TestAbsPointerDownInputState(arg);
        state.AddVelocityToStack_Test(new Vector2(10f, 10f), 0);
        state.AddVelocityToStack_Test(new Vector2(10f, 10f), 1);
        state.AddVelocityToStack_Test(new Vector2(10f, 10f), 2);

        state.OnEnter();
        Assert.That(state.GetVelocityStack_Test(), Is.EqualTo(new Vector2[arg.velocityStackSize]));
    }
    [Test]
    public void OnPointerDown_ThrowsException(){
        IPointerDownInputStateConstArg arg = CreateMockArg();
        TestAbsPointerDownInputState state = new TestAbsPointerDownInputState(arg);
        
        Assert.Throws(
            Is.TypeOf(typeof(System.InvalidOperationException)).
            And.Message.EqualTo("OnPointerDown should not be called while pointer is already held down"),
            () => {
                state.OnPointerDown(Substitute.For<ICustomEventData>());
            }
        );
    }
    [Test, TestCaseSource(typeof(VelocityIsOverSwipeThreshold_TestCase), "cases")]
    public void VelocityIsOverSwipeThreshold_VelocityNotLessThanThreshold_ReturnsTrue(
        Vector2 velocity, 
        float threshold,
        bool expected
    ){
        IPointerDownInputStateConstArg arg = CreateMockArg();
        arg.engine.GetSwipeVelocityThreshold().Returns(threshold);
        TestAbsPointerDownInputState state = new TestAbsPointerDownInputState(arg);

        bool actual = state.VelocityIsOverSwipeThreshold_Test(velocity);

        Assert.That(actual, Is.EqualTo(expected));
    }
    public class VelocityIsOverSwipeThreshold_TestCase{
        public static object[] cases = {
            new object[]{
                new Vector2(3f, 4f),
                5f,
                true
            },
            new object[]{
                new Vector2(-3f, 4f),
                5f,
                true
            },
            new object[]{
                new Vector2(3f, -4f),
                5f,
                true
            },
            new object[]{
                new Vector2(-3f, -4f),
                5f,
                true
            },
            new object[]{
                new Vector2(3f, 4f),
                5.001f,
                false
            },
        };
    }
    [Test, TestCaseSource(typeof(PushVelocityStack_TestCase), "cases")]
    public void PushVelocityStack_UpdatesVelocityStack(Vector2[] addedVelocity, Vector2[] expectedStack){
        IPointerDownInputStateConstArg arg = CreateMockArg();
        TestAbsPointerDownInputState state = new TestAbsPointerDownInputState(arg);

        foreach(Vector2 velocity in addedVelocity)
            state.PushVelocityStack_Test(velocity);

        Assert.That(state.GetVelocityStack_Test(), Is.EqualTo(expectedStack));
    }
    public class PushVelocityStack_TestCase{
        public static object[] cases = {
            new object[]{
                new Vector2[]{
                    new Vector2(1f, 1f)
                },
                new Vector2[]{
                    new Vector2(0f, 0f),
                    new Vector2(0f, 0f),
                    new Vector2(1f, 1f),
                }
            },
            new object[]{
                new Vector2[]{
                    new Vector2(1f, 1f),
                    new Vector2(2f, 2f),
                },
                new Vector2[]{
                    new Vector2(0f, 0f),
                    new Vector2(1f, 1f),
                    new Vector2(2f, 2f),
                }
            },
            new object[]{
                new Vector2[]{
                    new Vector2(1f, 1f),
                    new Vector2(2f, 2f),
                    new Vector2(3f, 3f),
                },
                new Vector2[]{
                    new Vector2(1f, 1f),
                    new Vector2(2f, 2f),
                    new Vector2(3f, 3f),
                }
            },
            new object[]{
                new Vector2[]{
                    new Vector2(1f, 1f),
                    new Vector2(2f, 2f),
                    new Vector2(3f, 3f),
                    new Vector2(4f, 4f),
                },
                new Vector2[]{
                    new Vector2(2f, 2f),
                    new Vector2(3f, 3f),
                    new Vector2(4f, 4f),
                }
            },
        };
    }
    [Test, TestCaseSource(typeof(GetAverageVelocity_TestCase), "cases")]
    public void GetAverageVelocity_Various(Vector2[] velocityStack, Vector2 expectedVelocity){
        IPointerDownInputStateConstArg arg = CreateMockArg();
        TestAbsPointerDownInputState state = new TestAbsPointerDownInputState(arg);
        state.SetVelocityStack_Test(velocityStack);

        Assert.That(state.GetAvarageVelocity_Test(), Is.EqualTo(expectedVelocity));
    }
    public class GetAverageVelocity_TestCase{
        public static object[] cases = {
            new object[]{
                new Vector2[]{
                    new Vector2(1f, 1f),
                    new Vector2(0f, 0f),
                    new Vector2(0f, 0f),
                },
                new Vector2(1f, 1f)
            },
            new object[]{
                new Vector2[]{
                    new Vector2(1f, 1f),
                    new Vector2(2f, 2f),
                    new Vector2(0f, 0f),
                },
                new Vector2(1.5f, 1.5f)
            },
            new object[]{
                new Vector2[]{
                    new Vector2(1f, 1f),
                    new Vector2(2f, 2f),
                    new Vector2(-3f, -3f),
                },
                new Vector2(0f, 0f)
            },
        };
    }
    

    class TestAbsPointerDownInputState: AbsPointerDownInputState{
        public TestAbsPointerDownInputState(IPointerDownInputStateConstArg arg): base(arg){
        }
        public override void OnExit(){}
        public override void OnPointerUp(ICustomEventData eventData){}
        public override void OnPointerEnter(ICustomEventData eventData){}
        public override void OnPointerExit(ICustomEventData eventData){}
        public void AddVelocityToStack_Test(Vector2 velocity, int index){
            thisVelocityStack[index] = velocity;
        }
        public Vector2[] GetVelocityStack_Test(){
            return thisVelocityStack;
        }
        public bool VelocityIsOverSwipeThreshold_Test(Vector2 velocity){
            return this.VelocityIsOverSwipeThreshold(velocity);
        }
        public void PushVelocityStack_Test(Vector2 velocity){
            this.PushVelocityStack(velocity);
        }
        public Vector2 GetAvarageVelocity_Test(){
            return this.GetAverageVelocity();
        }
        public void SetVelocityStack_Test(Vector2[] stack){
            thisVelocityStack = stack;
        }
    }
	IPointerDownInputStateConstArg CreateMockArg(){
        IPointerDownInputStateConstArg arg = Substitute.For<IPointerDownInputStateConstArg>();
        arg.engine.Returns(Substitute.For<IUIAdaptorInputStateEngine>());
        arg.uiManager.Returns(Substitute.For<IUIManager>());
        arg.velocityStackSize.Returns(3);

        return arg;
    }
}
