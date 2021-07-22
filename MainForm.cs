using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;

namespace HeightNormalConverter
{
	enum HeightMapChannelMode
	{
		R, G, B, A, Gradient
	}
	public partial class MainForm : Form
	{
		private Bitmap heightMap, normalMap;
		private BitmapData heightMapData, normalMapData;
		byte[] rgbValues;
		float[] heightValuesf;
		private string normalMapName, heightMapName, path;
		public int TaskRemain, iwidth, iheight, bytesPerPixel;
		public float savedHeightMin = 0, savedHeightMax = 255, rHeightMin = 0, rHeightMax = 3;
		HeightMapChannelMode HCMode = HeightMapChannelMode.Gradient;

		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainForm());
		}

		public MainForm()
		{
			InitializeComponent();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			normalMapBox.AllowDrop = true;
			heightMapBox.AllowDrop = true;
		}
		private void ConvertButton_Click(object sender, EventArgs e)
		{
			TaskRemain = 0;
			string file;
			if (convertModeN2H.Checked) //法线转高程
			{
				if (normalMap == null)
				{
					MessageBox.Show("法线贴图未填入");
					return;
				}
				convertButton.Enabled = false;
				InfoBox.Text = "正在初始化...";
				heightMapName = "HeightMapOf" + normalMapName;
				file = Path.Combine(path, heightMapName);
				iwidth = normalMap.Width;
				iheight = normalMap.Height;
				Rectangle Range = new Rectangle(0, 0, iwidth, iheight);//锁定像素的范围
				normalMapData = normalMap.LockBits(Range, ImageLockMode.ReadOnly, normalMap.PixelFormat);
				int heightPixelCount = iwidth * iheight;
				heightValuesf = new float[heightPixelCount];
				bytesPerPixel = normalMapData.Stride / normalMapData.Width;
				//计算
				InfoBox.Text = "正在计算...";
				heightValuesf = VectorSubtractEquals(heightValuesf, GetEnergyMap(heightValuesf));
				normalMap.UnlockBits(normalMapData);
				//Remap0-1
				InfoBox.Text = "正在重映射高程值...";
				float min = 0f, max = 0f;
				Parallel.ForEach(heightValuesf, f =>
				{
					min = Math.Min(min, f);
					max = Math.Max(max, f);
				});
				Parallel.For(0, heightValuesf.Length, a =>
				{
					heightValuesf[a] = Remap01(heightValuesf[a], min, max);
				});
				//保存图片
				InfoBox.Text = "正在保存图片...";
				PngBitmapEncoder encoder = new PngBitmapEncoder();
				encoder.Frames.Add(BitmapFrame.Create(
					BitmapSource.Create(iwidth, iheight, normalMap.HorizontalResolution, normalMap.VerticalResolution, PixelFormats.Gray32Float, null, heightValuesf, iwidth * 4)));
				FileStream heightmapfile = new FileStream(file + ".png", FileMode.Create);
				encoder.Save(heightmapfile);
				heightmapfile.Flush();
				heightmapfile.Close();
				//创建一个GDI+位图,用于在窗口中显示预览
				InfoBox.Text = "正在创建预览图....";
				int k;
				byte[] heightValuesb = new byte[heightPixelCount * 3];
				Parallel.For(0, heightPixelCount, i =>
				{
					k = i * 3;
					heightValuesb[k] = heightValuesb[k + 1] = heightValuesb[k + 2] = (byte)(256 * Saturate(heightValuesf[i]));
				});
				heightMap = new Bitmap(iwidth, iheight, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
				heightMapData = heightMap.LockBits(Range, ImageLockMode.WriteOnly, heightMap.PixelFormat);
				Marshal.Copy(heightValuesb, 0, heightMapData.Scan0, heightPixelCount * 3); //将算得RGB值存入bitmapdata
				heightMap.UnlockBits(heightMapData);
				heightMapBox.Image = heightMap;
				heightNameLabel.Text = heightMapName + "\n" + iwidth + "×" + iheight;
				convertButton.Enabled = true;
				MessageBox.Show("高程图已保存为" + file);
				InfoBox.Text = "提示:不理解的参数可以保持默认,用法可参考作者的B站专栏";
			}
			else    //高程转法线
			{
				if (heightMap == null)
				{
					MessageBox.Show("高程贴图未填入");
					return;
				}
				convertButton.Enabled = false;
				InfoBox.Text = "正在初始化...";
				normalMapName = "NormalMapOf" + heightMapName;
				file = Path.Combine(path, normalMapName);
				normalMap = new Bitmap(heightMap);
				iwidth = normalMap.Width;
				iheight = normalMap.Height;
				Rectangle Range = new Rectangle(0, 0, iwidth, iheight);//锁定像素的范围
				heightMapData = normalMap.LockBits(Range, ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
				int bytes = Math.Abs(heightMapData.Stride) * heightMap.Height;//确认RGB存储数组的数量(stride为一行像素*像素通道数)
				bytesPerPixel = heightMapData.Stride / heightMapData.Width;
				rgbValues = new byte[bytes];
				InfoBox.Text = "正在计算...";
				Parallel.For(0, iheight, y => Heiget2NormalRow(y)); //并行计算图片每一行
				InfoBox.Text = "正在保存图片...";
				Marshal.Copy(rgbValues, 0, heightMapData.Scan0, bytes); //将算得RGB值存入bitmapdata
				normalMap.UnlockBits(heightMapData);
				normalMapBox.Image = normalMap;
				normalNameLabel.Text = normalMapName + "\n" + iwidth + "×" + iheight;
				normalMap.Save(file, heightMap.RawFormat);
				InfoBox.Text = "提示:不理解的参数可以保持默认,用法可参考作者的B站专栏";
				convertButton.Enabled = true;
				MessageBox.Show("法线贴图已保存为" + file);
			}
		}
		public float[] GetEnergyMap(float[] HeightArray)
		{
			float[] EX = new float[HeightArray.Length];
			//+x
			for (int j = 0; j < iheight; j++)
			{
				int i = 0;
				float e = NY(i, j) + DY(ref HeightArray, ref iwidth, ref iheight, ref i, ref j) * NZ(i, j);
				EX[j * iwidth + i] = EX[Math.Max(j - 1, 0) * iwidth + i] - e;
			}
			Parallel.For(0, iheight, j =>
			{
				for (int i = 0; i <= iwidth / 2; i++)
				{
					float e = NX(i, j) + DX(ref HeightArray, ref iwidth, ref iheight, ref i, ref j) * NZ(i, j);
					EX[j * iwidth + i] = EX[j * iwidth + Math.Max(i - 1, 0)] + e;
				}
			});
			//-x
			for (int j = 0; j < iheight; j++)
			{
				int i = iwidth - 1;
				float e = NY(i, j) + DY(ref HeightArray, ref iwidth, ref iheight, ref i, ref j) * NZ(i, j);
				EX[j * iwidth + i] = EX[Math.Max(j - 1, 0) * iwidth + i] - e;
			}
			Parallel.For(0, iheight, j =>
			{
				for (int i = iwidth - 1; i >= iwidth / 2; i--)
				{
					float e = NX(i, j) + DX(ref HeightArray, ref iwidth, ref iheight, ref i, ref j) * NZ(i, j);
					EX[j * iwidth + i] = EX[j * iwidth + Math.Min(i + 1, iwidth - 1)] - e;
				}
			});
			float[] EY = new float[HeightArray.Length];
			//+y
			for (int i = 0; i < iwidth; i++)
			{
				int j = 0;
				float e = NX(i, j) + DX(ref HeightArray, ref iwidth, ref iheight, ref i, ref j) * NZ(i, j);
				EX[j * iwidth + i] = EX[j * iwidth + Math.Max(i - 1, 0)] + e;
			}
			Parallel.For(0, iwidth, i =>
			{
				for (int j = 0; j <= iheight / 2; j++)
				{
					float e = NY(i, j) + DY(ref HeightArray, ref iwidth, ref iheight, ref i, ref j) * NZ(i, j);
					EY[j * iwidth + i] = EY[Math.Max(j - 1, 0) * iwidth + i] - e;
				}
			});
			//-y
			for (int i = 0; i < iwidth; i++)
			{
				int j = iheight - 1;
				float e = NX(i, j) + DX(ref HeightArray, ref iwidth, ref iheight, ref i, ref j) * NZ(i, j);
				EX[j * iwidth + i] = EX[j * iwidth + Math.Max(i - 1, 0)] + e;
			}
			Parallel.For(0, iwidth, i =>
			{
				for (int j = iheight - 1; j >= iheight / 2; j--)
				{
					float e = NY(i, j) + DY(ref HeightArray, ref iwidth, ref iheight, ref i, ref j) * NZ(i, j);
					EY[j * iwidth + i] = EY[Math.Min(j + 1, iheight - 1) * iwidth + i] + e;
				}
			});
			return VectorAddEquals(EX, EY);
		}

		float NX(int x, int y)
			=> Marshal.ReadByte(normalMapData.Scan0 + (y * normalMapData.Stride) + x * bytesPerPixel + 2) / 128f - 1f;

		float NY(int x, int y)
			=> Marshal.ReadByte(normalMapData.Scan0 + (y * normalMapData.Stride) + x * bytesPerPixel + 1) / 128f - 1f;

		float NZ(int x, int y)
			=> Marshal.ReadByte(normalMapData.Scan0 + (y * normalMapData.Stride) + x * bytesPerPixel) / 128f - 1f;

		public static float DX(ref float[] array, ref int width, ref int height, ref int x, ref int y)
			=> HeightAtArray(ref array, ref width, ref height, x + 1, y) - HeightAtArray(ref array, ref width, ref height, x - 1, y);

		public static float DY(ref float[] array, ref int width, ref int height, ref int x, ref int y)
			=> HeightAtArray(ref array, ref width, ref height, x, y + 1) - HeightAtArray(ref array, ref width, ref height, x, y - 1);

		public static float HeightAtArray(ref float[] array, ref int width, ref int height, int x, int y)
			=> array[Clamp(y, 0, height - 1) * height + Clamp(x, 0, width - 1)];

		private void Heiget2NormalRow(int row)
		{
			for (int i = 0; i < iwidth; i++)
			{
				float X, Y, Z;
				Heiget2NormalPixel(ref i, ref row, out X, out Y, out Z);
				rgbValues[row * heightMapData.Stride + i * bytesPerPixel] = (byte)(Saturate(Z + 0.5f) * 255);
				rgbValues[row * heightMapData.Stride + i * bytesPerPixel + 1] = (byte)(Saturate(Y + 0.5f) * 255);
				rgbValues[row * heightMapData.Stride + i * bytesPerPixel + 2] = (byte)(Saturate(X + 0.5f) * 255);
			}
		}

		void Heiget2NormalPixel(ref int x, ref int y, out float X, out float Y, out float Z)
		{
			float dx = H(x + 1, y - 1) + (2 * H(x + 1, y)) + H(x + 1, y + 1) - (H(x - 1, y - 1) + (2 * H(x - 1, y)) + H(x - 1, y + 1));
			float dy = H(x - 1, y - 1) + (2 * H(x, y - 1)) + H(x + 1, y - 1) - (H(x - 1, y + 1) + (2 * H(x, y + 1)) + H(x + 1, y + 1));
			if (NormalGChannelModeDX.Checked) dy = -dy;
			//Normalize
			float length = (float)Math.Sqrt(dx * dx + dy * dy + 64f) * 2f;
			X = -dx / length;
			Y = -dy / length;
			Z = 8f / length;
		}

		float H(int x, int y)//Get height value of pixel at x,y
		{
			x = Clamp(x, 0, iwidth - 1);
			y = Clamp(y, 0, iheight - 1);
			float src;
			IntPtr MemLocation = heightMapData.Scan0 + (y * heightMapData.Stride) + x * bytesPerPixel;
			switch (HCMode)
			{
				case HeightMapChannelMode.A: src = Marshal.ReadByte(MemLocation + 3); break;
				case HeightMapChannelMode.R: src = Marshal.ReadByte(MemLocation + 2); break;
				case HeightMapChannelMode.G: src = Marshal.ReadByte(MemLocation + 1); break;
				case HeightMapChannelMode.B: src = Marshal.ReadByte(MemLocation); break;
				default:
					byte R = Marshal.ReadByte(MemLocation + 2);
					byte G = Marshal.ReadByte(MemLocation + 1);
					byte B = Marshal.ReadByte(MemLocation);
					src = (R * 19595 + G * 38469 + B * 7472) >> 16;
					break;
			}
			return Remap01(src, savedHeightMin, savedHeightMax) * (rHeightMax - rHeightMin);
		}

		static float Remap(float value, float fmin, float fmax, float tmin, float tmax) => (value - fmin) / (fmax - fmin) * (tmax - tmin) + tmin;

		static float Remap01(float value, float min, float max) => (value - min) / (max - min);

		static int Clamp(int value, int min, int max) => Math.Min(Math.Max(value, min), max);

		static float Saturate(float value) => Math.Min(Math.Max(value, 0), 0.99999f);

		float[] VectorSubtractEquals(float[] VectorA, float[] VectorB)//A-=B
		{
			if (VectorA.Length == VectorB.Length)
			{
				Parallel.For(0, VectorA.Length, i =>
				{
					VectorA[i] -= VectorB[i];
				});
				return VectorA;
			}
			else throw new ArgumentException("两个数组长度必须相同.");
		}

		float[] VectorAddEquals(float[] VectorA, float[] VectorB)//A+=B
		{
			if (VectorA.Length == VectorB.Length)
			{
				Parallel.For(0, VectorA.Length, i =>
				{
					VectorA[i] += VectorB[i];
				});
				return VectorA;
			}
			else throw new ArgumentException("两个数组长度必须相同.");
		}

		private void TextBox_KeyPress(object sender, KeyPressEventArgs e)
		{
			TextBox s = sender as TextBox;
			if (e.KeyChar == '.' && (s.Text.Contains(".") || s.Text == ""))
			{
				e.Handled = true;
			}
			if (s.Text == "")
			{
				if (e.KeyChar <= '0' || e.KeyChar > '9') e.Handled = true;
			}
			else if ((e.KeyChar < '0' || e.KeyChar > '9') && e.KeyChar != 8 && e.KeyChar != '.')
			{
				e.Handled = true;
			}
		}

		private void realHeightMax_TextChanged(object sender, EventArgs e) =>
			rHeightMax = Convert.ToSingle(realHeightMax.Text);

		private void realHeightMin_TextChanged(object sender, EventArgs e) =>
			rHeightMin = Convert.ToSingle(realHeightMin.Text);

		private void textureHeightMax_TextChanged(object sender, EventArgs e) =>
			savedHeightMax = Convert.ToSingle(textureHeightMax.Text);

		private void textureHeightMin_TextChanged(object sender, EventArgs e) =>
			savedHeightMin = Convert.ToSingle(textureHeightMin.Text);

		private void ConvertModeN2H_CheckedChanged(object sender, EventArgs e) =>
			ChangeConvertMode();

		private void ConvertModeH2N_CheckedChanged(object sender, EventArgs e) =>
			ChangeConvertMode();

		private void ChannelModeR_CheckedChanged(object sender, EventArgs e)
		{
			if (ChannelModeR.Checked)
				HCMode = HeightMapChannelMode.R;
		}

		private void ChannelModeG_CheckedChanged(object sender, EventArgs e)
		{
			if (ChannelModeG.Checked)
				HCMode = HeightMapChannelMode.G;
		}

		private void ChannelModeB_CheckedChanged(object sender, EventArgs e)
		{
			if (ChannelModeB.Checked)
				HCMode = HeightMapChannelMode.B;
		}

		private void ChannelModeA_CheckedChanged(object sender, EventArgs e)
		{
			if (ChannelModeA.Checked)
				HCMode = HeightMapChannelMode.A;
		}

		private void ChannelModeGradient_CheckedChanged(object sender, EventArgs e)
		{
			if (ChannelModeGradient.Checked)
				HCMode = HeightMapChannelMode.Gradient;
		}

		private void ChangeConvertMode()
		{
			bool @checked = convertModeN2H.Checked;
			normalMapBoxLabel.Enabled = @checked;
			normalMapBox.Enabled = @checked;
			normalNameLabel.Enabled = @checked;
			heightMapBoxLabel.Enabled = !@checked;
			heightMapBox.Enabled = !@checked;
			heightNameLabel.Enabled = !@checked;
		}
		private void NormalBox_DragDrop(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				string[] array = (string[])e.Data.GetData(DataFormats.FileDrop);
				try
				{
					normalMap = new Bitmap(Image.FromFile(array[0]));
					normalMapName = Path.GetFileName(array[0]);
					path = Path.GetDirectoryName(array[0]);
					normalMapBox.Image = normalMap;
					iwidth = normalMap.Width;
					iheight = normalMap.Height;
					normalNameLabel.Text = normalMapName + "\n" + iwidth + "×" + iheight;
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message);
				}
			}
		}

		private void HeightBox_DragDrop(object sender, DragEventArgs e)
		{
			bool dataPresent = e.Data.GetDataPresent(DataFormats.FileDrop);
			if (dataPresent)
			{
				string[] array = (string[])e.Data.GetData(DataFormats.FileDrop);
				try
				{
					heightMap = new Bitmap(Image.FromFile(array[0]));
					heightMapName = Path.GetFileName(array[0]);
					path = Path.GetDirectoryName(array[0]);
					heightMapBox.Image = heightMap;
					iwidth = heightMap.Width;
					iheight = heightMap.Height;
					heightNameLabel.Text = heightMapName + "\n" + iwidth + "×" + iheight;
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message);
				}
			}
		}

		private void NormalBox_DragEnter(object sender, DragEventArgs e)
		{
			bool flag = e.Data.GetDataPresent(DataFormats.Bitmap) || e.Data.GetDataPresent(DataFormats.FileDrop);
			if (flag) e.Effect = DragDropEffects.Copy;
			else e.Effect = DragDropEffects.None;
		}

		private void HeightBox_DragEnter(object sender, DragEventArgs e)
		{
			bool flag = e.Data.GetDataPresent(DataFormats.Bitmap) || e.Data.GetDataPresent(DataFormats.FileDrop);
			if (flag) e.Effect = DragDropEffects.Copy;
			else e.Effect = DragDropEffects.None;
		}
	}
}