using System;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;

namespace Mausr.Core {
	public static class SerializationHelper {

		public static void SerializeInt(int value, byte[] data, ref int index) {
			Contract.Requires(index + 4 < data.Length);

			var intData = BitConverter.GetBytes(value);
			for (int i = 0; i < 4; ++i) {
				data[index++] = intData[i];
			}
		}

		public static int DeserializeInt(byte[] data, ref int index) {
			Contract.Requires(index + 4 < data.Length);

			int value = BitConverter.ToInt32(data, index);
			index += 4;
			return value;
		}

		public static void SerializeArray<T>(T[] array, byte[] data, ref int index) {
			Contract.Requires(index + array.Length * Marshal.SizeOf<T>() < data.Length);

			int bytesCount = Marshal.SizeOf<T>() * array.Length;
			var pinnedHandle = GCHandle.Alloc(array, GCHandleType.Pinned);
			Marshal.Copy(pinnedHandle.AddrOfPinnedObject(), data, index, bytesCount);
			pinnedHandle.Free();

			index += bytesCount;
		}

		public static void DeserializeArray<T>(T[] array, byte[] data, ref int index) {
			Contract.Requires(index + array.Length * Marshal.SizeOf<T>() < data.Length);

			int bytesCount = Marshal.SizeOf<T>() * array.Length;
			var pinnedHandle = GCHandle.Alloc(array, GCHandleType.Pinned);
			Marshal.Copy(data, index, pinnedHandle.AddrOfPinnedObject(), bytesCount);
			pinnedHandle.Free();

			index += bytesCount;
		}

	}
}
