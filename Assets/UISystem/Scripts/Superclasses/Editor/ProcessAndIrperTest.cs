using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using NSubstitute;
using UISystem;

public class ProcessAndIrperTest{
	[Test][TestCaseSource(typeof(ExpirationTestCase), "nonExpireCases")]
	public void WaitAndExpipreProcess_Construction_ExpireTIsSetLessThanOrEqualToZero_UpdateProcessIsSuppliedWithEnoughDeltaT_DoesNotCallProcessStateOnProcessExpire(float expireT){
		IProcessManager procMan = Substitute.For<IProcessManager>();
		IWaitAndExpireProcessState state = Substitute.For<IWaitAndExpireProcessState>();
		IWaitAndExpireProcess process = new GenericWaitAndExpireProcess(procMan, state, expireT);
		
		float timer = 0f;
		float durationOfTest = expireT > 0f? expireT: 1f;
		float pseudoDeltaT = .1f;
		for(int i = 0; timer < durationOfTest; i ++){
			process.UpdateProcess(pseudoDeltaT);
			timer += pseudoDeltaT;
		}

		state.DidNotReceive().OnProcessExpire();
	}
	[Test][TestCaseSource(typeof(ExpirationTestCase), "expireCases")]
	public void WaitAndExpipreProcess_Construction_ExpireTIsSetGreaterThanZero_UpdateProcessIsSuppliedWithEnoughDeltaT_CallProcStateOnExpire(float expireT){
		IProcessManager procMan = Substitute.For<IProcessManager>();
		IWaitAndExpireProcessState state = Substitute.For<IWaitAndExpireProcessState>();
		IWaitAndExpireProcess process = new GenericWaitAndExpireProcess(procMan, state, expireT);
		
		float timer = 0f;
		float durationOfTest = expireT;
		float pseudoDeltaT = .1f;
		for(int i = 0; timer < durationOfTest; i ++){
			process.UpdateProcess(pseudoDeltaT);
			timer += pseudoDeltaT;
		}

		state.Received(1).OnProcessExpire();
	}
		class ExpirationTestCase{
			static object[] nonExpireCases = {
				new object[]{0f},
				new object[]{-1f},
			};
			static object[] expireCases = {
				new object[]{.1f},
				new object[]{1f},
				new object[]{2f},
				new object[]{.01f}
			};
		}
	[Test]
	public void WaitAndExpipreProcess_UpdateProcess_WhenCalled_CallsProcStateOnProcessUpdate(){
		IProcessManager procMan = Substitute.For<IProcessManager>();
		IWaitAndExpireProcessState state = Substitute.For<IWaitAndExpireProcessState>();
		IWaitAndExpireProcess process = new GenericWaitAndExpireProcess(procMan, state, 0f);
		float deltaT = .1f;

		process.UpdateProcess(deltaT);

		state.Received(1).OnProcessUpdate(deltaT);
	}
}
