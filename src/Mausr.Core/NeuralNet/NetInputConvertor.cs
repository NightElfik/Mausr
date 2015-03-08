using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace Mausr.Core.NeuralNet {
	public class NetInputConvertor {

		private RawDataProcessor dataProcessor = new RawDataProcessor();


		public void Shuffle<T>(IList<T> stuff) {
			var rand = new Random();
			int count = stuff.Count;

			for (int i = 0; i < count; ++i) {
				int otherI = rand.Next(0, count - 1);
				T tempI = stuff[i];
				stuff[i] = stuff[otherI];
				stuff[otherI] = tempI;
			}
		}


		public Matrix<double> CreateInputsMatrix(IEnumerable<RawDrawing> rawDrawings, Rasterizer rasterizer, bool normalize) {
			Contract.Requires(rawDrawings.Count() > 0);
			var inputVectors = new List<Vector<double>>();

			using (var img = new Bitmap(rasterizer.ImageSize, rasterizer.ImageSize, PixelFormat.Format24bppRgb)) {
				int cols = img.Width * img.Height;
				foreach (var drawing in rawDrawings) {
					RawDrawing rd = normalize ? dataProcessor.Normalize(drawing) : drawing;
					rasterizer.Rasterize(img, rd);
					inputVectors.Add(ImageToVector(img));
				}
			}

			return DenseMatrix.OfRowVectors(inputVectors);
		}

		public int[] CreateOutIndicesFromIds(IEnumerable<int> inputIds, Net network) {
			return inputIds.Select(id => network.MapOutputToOutNeuron(id)).ToArray();
		}

		unsafe public Vector<double> ImageToVector(Bitmap img) {
			Contract.Requires(img.PixelFormat == PixelFormat.Format24bppRgb);

			int wid = img.Width;
			int hei = img.Height;

			var imgData = img.LockBits(new Rectangle(0, 0, wid, hei), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

			var resultVector = new DenseVector(wid * hei);

			byte* imgPtr = (byte*)imgData.Scan0;
			int imgStride = imgData.Stride;

			for (int y = 0; y < hei; ++y) {
				int baseVecI = y * wid;
				int baseStride = y * imgStride;

				for (int x = 0; x < wid; ++x) {
					resultVector[baseVecI + x] = 1.0 - imgPtr[baseStride + x * 3] / 255.0;
				}
			}

			img.UnlockBits(imgData);

			return resultVector;
		}

		unsafe public void VectorToImage(Vector<double> vector, Bitmap outImg) {
			Contract.Requires(outImg.PixelFormat == PixelFormat.Format24bppRgb);
			Contract.Requires(outImg.Width * outImg.Height == vector.Count);

			int wid = outImg.Width;
			int hei = outImg.Height;

			var imgData = outImg.LockBits(new Rectangle(0, 0, wid, hei), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
			
			byte* imgPtr = (byte*)imgData.Scan0;
			int imgStride = imgData.Stride;

			for (int y = 0; y < hei; ++y) {
				int baseVecI = y * wid;
				int baseStride = y * imgStride;

				for (int x = 0; x < wid; ++x) {
					double mult = 1 - vector[baseVecI + x];
					int imgI = baseStride + x * 3;
					imgPtr[imgI] = (byte)Math.Round(imgPtr[imgI] * mult);
					imgPtr[imgI + 1] = (byte)Math.Round(imgPtr[imgI + 1] * mult);
					imgPtr[imgI + 2] = (byte)Math.Round(imgPtr[imgI + 2] * mult);
				}
			}

			outImg.UnlockBits(imgData);
		}

	}
}
