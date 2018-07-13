using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using NSubstitute;
using UISystem;
using DKUtility;

[TestFixture]
public class TurnImageDarknessProcessTest {
	[Test]
	public void Run_CallsImageSetDarknessWithSourceValue(){
		IUIImage image;
		const float sourceDarkness = .5f;
		TurnImageDarknessProcess process = CreateTurnImageDarknessProcess(out image, sourceDarkness, 1f);
		process.Run();

		image.Received(1).SetDarkness(sourceDarkness);
	}
	[Test][TestCaseSource(typeof(UpdateProcess_TestCases), "cases")]
	public void UpdateProcess_CallsImageSetDarkness(float sourceDarkness, float targetDarkness, float expectedEpireT){
		IUIImage image;
		TurnImageDarknessProcess process = CreateTurnImageDarknessProcess(out image, sourceDarkness, targetDarkness);

		process.Run();
		image.Received(1).SetDarkness(sourceDarkness);

		float deltaT = .1f;
		for(float f = deltaT; f < expectedEpireT; f += deltaT){
			float elapsedT = f;
			process.UpdateProcess(deltaT);
			float normalizedT = f/expectedEpireT;
			float expectedNewDarkness = Mathf.Lerp(sourceDarkness, targetDarkness, normalizedT);
			image.Received(1).SetDarkness(expectedNewDarkness);
			image.DidNotReceive().SetDarkness(targetDarkness);
		}

		process.UpdateProcess(deltaT);
		image.Received(1).SetDarkness(targetDarkness);
	}
	public class UpdateProcess_TestCases{
		public static object[] cases = {
			new object[]{0f, 1f, 1f},
			new object[]{0f, .5f, .5f},
			new object[]{1f, .5f, .5f},
			new object[]{1f, 0f, 1f}
		};
	}

	public TurnImageDarknessProcess CreateTurnImageDarknessProcess(out IUIImage image, float curDarkness, float targetDarkness){
		IProcessManager processManager = Substitute.For<IProcessManager>();
		IUIImage img = Substitute.For<IUIImage>();
		img.GetCurrentDarkness().Returns(curDarkness);
		TurnImageDarknessProcess process = new TurnImageDarknessProcess(processManager, ProcessConstraint.rateOfChange, 1f, .05f, img, targetDarkness, false);
		image = img;
		return process;
	}
}
