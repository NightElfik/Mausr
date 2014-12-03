using System.Diagnostics.Contracts;
using System.Reflection;

namespace Mausr.Web {
	public static class ObjectExts {

		public static B ShallowCloneAs<B>(this object instance) where B : new() {
			Contract.Requires(typeof(B).IsAssignableFrom(instance.GetType()));

			B newInstance = new B();
			var fields = typeof(B).GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

			foreach (var fi in fields) {
				fi.SetValue(newInstance, fi.GetValue(instance));
			}

			return newInstance;
		}

	}
}