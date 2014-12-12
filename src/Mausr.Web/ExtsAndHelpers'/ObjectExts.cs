using System.Linq;
using System.Diagnostics.Contracts;
using System.Reflection;

namespace Mausr.Web {
	public static class ObjectExts {

		public static B ShallowCloneAs<B>(this object instance) where B : new() {
			//Contract.Requires(typeof(B).IsAssignableFrom(instance.GetType()));

			B newInstance = new B();
			var bf = BindingFlags.Public | BindingFlags.Instance;
			var destProps = typeof(B).GetProperties(bf).Where(pi => pi.CanWrite);
			var srcProps = instance.GetType().GetProperties(bf).Where(pi => pi.CanRead);

			var props = destProps.Join(srcProps, pi => pi.Name, pi => pi.Name, (dest, src) => new { DestPi = dest, SrcPi = src });

			foreach (var pi in props) {
				pi.DestPi.SetValue(newInstance, pi.SrcPi.GetValue(instance));
			}

			return newInstance;
		}

	}
}