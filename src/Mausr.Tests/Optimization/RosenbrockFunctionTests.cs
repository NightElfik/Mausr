using System.Linq;
using Mausr.Core.Optimization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Mausr.Tests.Optimization {
	[TestClass]
	public class RosenbrockFunctionTests {
		[TestMethod]
		public void RosenbrockDerivativeTest() {
			FunctionDerivativeTester.PerformDerivativeTest(new RosenbrockFunction(),
				Enumerable.Range(-100, 200).SelectMany(x =>
					Enumerable.Range(-100, 200).Select(y =>
						new double[] { x / 70.0, y / 70.0 }
					).ToArray()
				).ToArray()
			);
		}
	}
}
