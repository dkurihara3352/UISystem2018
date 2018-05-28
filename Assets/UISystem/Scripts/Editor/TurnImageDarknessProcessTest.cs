using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using NSubstitute;
using UISystem;

public class TurnImageDarknessProcessTest{
    [Test][TestCaseSource(typeof(Run_VariousDiffTestCase), "validCases")]
    public void Run_WhenDiffIsBigEnough_ProcMngReceivesAddRunningProcessThis(float initDarkness, float tarDarkness){
        TurnImgDrkProcConstArg arg;
        TurnImageDarknessProcess process = CreateTurnImgDrkProcess(out arg, initDarkness, tarDarkness);

        process.Run();

        IProcessManager procManager = arg.procManager;
        procManager.Received().AddRunningProcess(process);
        
    }
    [Test][TestCaseSource(typeof(Run_VariousDiffTestCase), "invalidCases")]
    public void Run_WhenDiffIsNotBigEnough_ProcMngDidNotReceiveAddRunningProcessThis(float initDarkness, float tarDarkness){
        TurnImgDrkProcConstArg arg;
        TurnImageDarknessProcess process = CreateTurnImgDrkProcess(out arg, initDarkness, tarDarkness);

        process.Run();

        IProcessManager procManager = arg.procManager;
        procManager.DidNotReceive().AddRunningProcess(process);
    }
        class Run_VariousDiffTestCase{
            static object[] validCases = {
                new object[]{ .5f, .56f}// .06
                ,new object[]{ .5f, 100f}// 99.5 => clamped to 0.5
                ,new object[]{ .5f, .555f}// .55
                ,new object[]{ .5f, .5500000000001f}// .55
                ,new object[]{ .5f, .44f}// -0.06
            };
            static object[] invalidCases = {
                new object[]{ .5f, .5f}// same
                , new object[]{ .5f, .54f}// 0.04
                , new object[]{ .5f, .54999f}// 0.04999
                , new object[]{ .5f, .46f}// -0.04
            };
        }
    [Test][TestCaseSource(typeof(UpdateProcessExpirationTestCase), "validCases")]
    public void UpdateProcess_WhenCalledAfterRunAndFedWithSufficientDeltaT_CallsProcManagerRemoveRunningProcessThis(float initD, float tarD, float expiringT){
        TurnImgDrkProcConstArg arg;
        TurnImageDarknessProcess process = CreateTurnImgDrkProcess(out arg, initD, tarD);
        IProcessManager procManager = arg.procManager;
        process.Run();
        float i = 0f;
        process.UpdateProcess(0f);
        for(i = 0f; i < expiringT - 0.1f; i += 0.1f){
            process.UpdateProcess(0.1f);
            procManager.DidNotReceive().RemoveRunningProcess(process);
        }
        i += 0.1f;
        process.UpdateProcess(0.1f);
        procManager.Received().RemoveRunningProcess(process);
        // Debug.Log("time is up: t = " + i.ToString());
    }
        class UpdateProcessExpirationTestCase{
            static object[] validCases = {
                new object[]{0f, 1f, 1f},
                new object[]{.5f, 1f, 0.5f},
                new object[]{1f, .2f, 0.8f},
                new object[]{1f, 0f, 1f},
                new object[]{-0.2f, .5f, .5f},
                new object[]{0f, 300f, 1f}
            };
        }
    /* Test Support Methods and Classes */
    class TurnImgDrkProcConstArg{
        public TurnImgDrkProcConstArg(IProcessManager procManager, IUIImage image){
            this.procManager = procManager;
            this.image = image;
        }
        public IProcessManager procManager;
        public IUIImage image;
    }
    TurnImageDarknessProcess CreateTurnImgDrkProcess(out TurnImgDrkProcConstArg arg, float initDarkness, float targetDarkness){
        IUIImage image = Substitute.For<IUIImage>();
            image.GetCurrentDarkness().Returns(initDarkness);
        IProcessManager procManager = Substitute.For<IProcessManager>();
        
        TurnImageDarknessProcess process = new TurnImageDarknessProcess(procManager, image, targetDarkness);
        arg = new TurnImgDrkProcConstArg(procManager, image);

        return process;
    }
}
