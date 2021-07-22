using System;
using System.Threading.Tasks;
using Net.Kniaz.Optimization;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using DotNetMatrix;

namespace HeightNormalConverter
{
	class NormalEnergyFunction : IGradientFunction
	{
		BitmapData NormalMap;
		int width,height,bytePerPixel;

		public NormalEnergyFunction(BitmapData NormalMapData)
		{
			NormalMap = NormalMapData;
			width = NormalMap.Width;
			height = NormalMap.Height;
			bytePerPixel = NormalMap.Stride / NormalMap.Width;
		}
		#region IGradientFunction Members
		public int Dimension { get { return NormalMap.Width*NormalMap.Height; } }

		public static float Square(float v) => v * v;

		public static float HeightAtArray(ref float[] array, ref int width,ref int height, int x, int y) 
			=> array[Clamp(y,0,height-1) * height + Clamp(x,0,width-1)];

		public static float DX(ref float[] array, ref int width, ref int height, ref int x, ref int y)
			=> HeightAtArray(ref array, ref width, ref height, x + 1, y) - HeightAtArray(ref array, ref width, ref height, x - 1, y);

		public static float DY(ref float[] array, ref int width, ref int height, ref int x, ref int y)
			=> HeightAtArray(ref array, ref width, ref height, x, y + 1) - HeightAtArray(ref array, ref width, ref height, x, y - 1);

		public static Vector3 TangentU(ref float[] array,ref int width,ref int height, ref int x, ref int y)
			=> new Vector3(1, 0, DX(ref array, ref width,ref height,ref x,ref y));

		public static Vector3 TangentV(ref float[] array,ref int width,ref int height, ref int x, ref int y)
			=> new Vector3(0, 1, DY(ref array, ref width, ref height,ref x,ref y));

		public float GetVal(float[] HeightArray)
		{
			float E = 0;
			Parallel.For(0, height, j =>
			{
				for (int i = 0; i < width; i++)
					E += Square(NX(i, j) + DX(ref HeightArray, ref width, ref height, ref i, ref j) * NZ(i, j))
							+ Square(NY(i, j) + DY(ref HeightArray, ref width, ref height, ref i, ref j) * NZ(i, j));
			});
			return E;
		}

		public float[] GetEnergyMap(float[] HeightArray)
		{
			float[] E = new float[HeightArray.Length];
			Parallel.For(0, height, j =>
			{
				for (int i = 0; i < width; i++)
					E[j * width + i] = NX(i, j) + DX(ref HeightArray, ref width, ref height, ref i, ref j) * NZ(i, j)
											+ NY(i, j) + DY(ref HeightArray, ref width, ref height, ref i, ref j) * NZ(i, j);
			});
			return E;
		}
		public float[] GetGradient(float[] HeightArray)
		{
			float[] GradientArray = new float[HeightArray.Length];
			float[] GradientArray2 = new float[HeightArray.Length];
			Parallel.For(0, height, j =>
			{
				for (int i = 0; i < width; i++)
					GradientArray[j * width + i] = 2f * (NX(i, j) + DX(ref HeightArray, ref width, ref height, ref i, ref j) * NZ(i, j)) * NZ(i, j);
			});
			Parallel.For(0, height, j =>
			{
				for (int i = 0; i < width; i++)
					GradientArray[j * width + i] -= GradientArray[j * width + Math.Min(i + 1, width - 1)];
					//GradientArray[j * width + i] = GradientArray[j*width+i] - GradientArray[Math.Min(j+1,height-1)*width+i];
			});
			Parallel.For(0, height, j =>
			{
				for (int i = 0; i < width; i++)
					GradientArray2[j * width + i] = -2f * (NY(i, j) - DY(ref HeightArray, ref width, ref height, ref i, ref j) * NZ(i, j)) * NZ(i, j);
			});
			Parallel.For(0, height, j =>
			{
				for (int i = 0; i < width; i++)
					GradientArray2[j * width + i] -= GradientArray2[Math.Min(j + 1, height - 1) * width + i];
					//GradientArray2[j * width + i] = GradientArray2[j*width+i] - GradientArray2[j*width+Math.Min(i+1,width-1)];
			});
			Parallel.For(0, height, j =>
			{
				for (int i = 0; i < width; i++)
					GradientArray[j * width + i] += GradientArray2[j * width + i];
			});
			return GradientArray;
		}
		#endregion

		float NX(int x, int y)
		{
			IntPtr MemLocation = NormalMap.Scan0 + (y * NormalMap.Stride) + x* bytePerPixel;
			return  Marshal.ReadByte(MemLocation + 2) / 127.5f - 1f;
		}
		float NY(int x, int y)
		{
			IntPtr MemLocation = NormalMap.Scan0 + (y * NormalMap.Stride) + x* bytePerPixel;
			return Marshal.ReadByte(MemLocation + 1) / 127.5f - 1f;
		}
		float NZ(int x, int y)
		{
			IntPtr MemLocation = NormalMap.Scan0 + (y * NormalMap.Stride) + x* bytePerPixel;
			return Marshal.ReadByte(MemLocation) / 127.5f - 1f;
		}
		static int Clamp(int value, int min, int max) => Math.Min(Math.Max(value, min), max);
	}
}
