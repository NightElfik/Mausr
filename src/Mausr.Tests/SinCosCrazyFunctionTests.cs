using System.Linq;
using Mausr.Core.Optimization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Mausr.Tests {
	[TestClass]
	public class SinCosCrazyFunctionTests {
		[TestMethod]
		public void SinCosCrazyDerivativeTest() {
			FunctionDerivativeTester.PerformDerivativeTest(new SinCosCrazyFunction(),
				Enumerable.Range(-100, 200).SelectMany(x =>
					Enumerable.Range(-100, 200).Select(y =>
						new double[] { x / 50.0, y / 50.0 }
					).ToArray()
				).ToArray()
			);
		}
	}
}
