using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TestTesting{
	public interface ITestClass{
		int Doubles(int q);
	}

	public class TestClass: ITestClass{
		public int Doubles(int q){
			return q *2;
		}
	}
}
