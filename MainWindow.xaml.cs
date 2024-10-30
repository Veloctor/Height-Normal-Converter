using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Velctor.Utils;
using static System.Runtime.InteropServices.Marshal;
using static HeightNormalConverter.BitmapUtils;

namespace HeightNormalConverter;

public partial class MainWindow
{
	static readonly HashSet<string> supportImageFormats = [".png", ".jpg", ".jpeg", ".tif", ".tiff", ".bmp", ".raw"];
	IntPtr SourceImageBuffer;
	PixelStruct sourceImagePixelStruct;
	int sourceMapWidth;
	int sourceMapHeight;
	IntPtr MaskImageBuffer;
	PixelStruct maskImagePixelStruct;
	int maskMapWidth;
	int maskMapHeight;

	long isIterating;
	long needIterStop;

	public MainWindow()
	{
		InitializeComponent();
	}

	void ConvertButton_Click(object sender, RoutedEventArgs e)
	{
		if (Interlocked.Read(ref isIterating) != 0)
		{
			Interlocked.Exchange(ref needIterStop, 1);
			return;
		}
		if (SourceImageBuffer == IntPtr.Zero)
		{
			ButtonInfoTextBox.Text = "请先放入源贴图";
			return;
		}

		if (RadioButtonH2N.IsChecked.IsTrue())
		{
			ButtonInfoTextBox.Text = "正在执行高程转法线...";
			int channelOffset = -1;
			if (HeightReadChannelR.IsChecked.IsTrue())
				channelOffset = sourceImagePixelStruct.GetChannelOffset(0);
			else if (HeightReadChannelG.IsChecked.IsTrue())
				channelOffset = sourceImagePixelStruct.GetChannelOffset(1);
			else if (HeightReadChannelB.IsChecked.IsTrue())
				channelOffset = sourceImagePixelStruct.GetChannelOffset(2);
			else if (HeightReadChannelA.IsChecked.IsTrue())
				channelOffset = sourceImagePixelStruct.GetChannelOffset(3);
			bool isUnsigned = Unsigned16BitCheckBox.IsChecked.IsTrue();
			bool isBigEndian = BigEndianCheckBox.IsChecked.IsTrue();
			if (!float.TryParse(HeightScaleBox.Text, out float heightScale)) {
				ButtonInfoTextBox.Text = "高程缩放值文本表示的不是数字！";
				return;
			}
			bool dxGChannel = NormalGChannelModeDX.IsChecked.IsTrue();
			string filePath = Path.ChangeExtension(SourceMapNameLabel.Content.ToString(), "_Normal.dummy");
			Task.Run(() => ConvertHeightToNormal(channelOffset, heightScale, isUnsigned, isBigEndian, dxGChannel, filePath));
		}
		else {
			if (!float.TryParse(HeightScaleBox.Text, out float heightScale)) {
				ButtonInfoTextBox.Text = "高程缩放值文本表示的不是数字！";
				return;
			}
			bool dxGChannel = NormalGChannelModeDX.IsChecked.IsTrue();
			string filePath = Path.ChangeExtension(SourceMapNameLabel.Content.ToString(), "_Height.dummy");
			ConvertButton.Content = "停止迭代";
			Task.Run(() =>
			{
				Interlocked.Exchange(ref isIterating, 1);
				nint targetBufferSize = (nint)sourceMapWidth * sourceMapHeight;
				IntPtr iterateBuffer = AllocHGlobal(targetBufferSize);
				try
				{
					int iter = 0;
					while (Interlocked.Read(ref needIterStop) == 0)
					{
						IterateNormalToHeight(iterateBuffer, ++iter, heightScale, dxGChannel);
						WriteImageToFile(filePath, iterateBuffer, sourceMapWidth, sourceMapHeight, targetBufferSize, PixelFormats.Gray8);
						MessageTextBox.Dispatcher.BeginInvoke(() => MessageTextBox.Text = $"迭代次数：{iter}\n文件已经写入到：{filePath}");
					}
				}
				finally
				{
					FreeHGlobal(iterateBuffer);
					ConvertButton.Dispatcher.Invoke(() => ConvertButton.Content = "转换");
					Interlocked.Exchange(ref isIterating, 0);
					Interlocked.Exchange(ref needIterStop, 0);
				}
			});
		}

		ButtonInfoTextBox.Text = null;
	}

	void IterateNormalToHeight(IntPtr targetMapBuffer, int iter, float heightScale, bool dxGChannel)
	{
		int width = sourceMapWidth;
		int height = sourceMapHeight;
		int sourceBytesPerChannel = sourceImagePixelStruct.BytesPerChannel;
		int sourceChannelCount = sourceImagePixelStruct.ChannelCount;
		int sourceBpp = sourceImagePixelStruct.BytesPerPixel;
		int sourceStride = width * sourceBpp;

		int targetStride = width * 1; //1是8bit高程图每像素1字节，为16/32bit高程图需求预留下空间

		Parallel.For(0, height, y =>
		{
			Ptr<byte> rowAddr = targetMapBuffer + (nint)y * targetStride; //不转换nint的话,大图可能超4G值溢出
			for (int x = 0; x < width; x++)
			{
				rowAddr[x * 1] = (byte)(iter * 3);
			}
		});
	}

	void ConvertHeightToNormal(int channelOffset, float heightScale, bool isUnsigned, bool isBigEndian, bool GreenChannelFlip, string outputFile)
	{
		int width = sourceMapWidth;
		int height = sourceMapHeight;
		int sourceBytesPerChannel = sourceImagePixelStruct.BytesPerChannel;
		int sourceChannelCount = sourceImagePixelStruct.ChannelCount;
		int sourceBpp = sourceImagePixelStruct.BytesPerPixel;
		int sourceStride = width * sourceBpp;

		int targetStride = width * 3;
		nint targetBufferSize = (nint)targetStride * height;
		IntPtr targetMapBuffer = AllocHGlobal(targetBufferSize);
		try
		{
			Parallel.For(0, height, y =>
			{
				Ptr<byte> rowAddr = targetMapBuffer + (nint)y * targetStride; //不转换nint的话,大图可能超4G值溢出
				/*#region 生成星球经纬高程法向量
				const double 赤道周长m = Math.PI * 2 * 1737400;
				double 纬度rad = -(double)y / height * Math.PI + (Math.PI / 2);
				double 纬度周长m = Math.Cos(纬度rad) * 赤道周长m;
				float rev像素宽度m = (float)(width / 纬度周长m) / 8; //横向分辨率的倒数.(/8是Sobel算子的归一化系数)
				float rev像素高度m = (float)(height / (赤道周长m / 2)) / 8; //纵向分辨率的倒数.
				#endregion*/

				Span<float> f = stackalloc float[9];
				for (int x = 0; x < width; x++)
				{
					for (int j = -1; j <= 1; j++)
					for (int i = -1; i <= 1; i++)
					{
						f[ij2idx(i, j)] = H(x + i, y + j);
					}

					float dx = f[ij2idx(1, -1)] + 2 * f[ij2idx(1, 0)] + f[ij2idx(1, 1)] -
					           (f[ij2idx(-1, -1)] + 2 * f[ij2idx(-1, 0)] + f[ij2idx(-1, 1)]);
					float dy = f[ij2idx(-1, -1)] + 2 * f[ij2idx(0, -1)] + f[ij2idx(1, -1)] -
					           (f[ij2idx(-1, 1)] + 2 * f[ij2idx(0, 1)] + f[ij2idx(1, 1)]);
					if (GreenChannelFlip) dy = -dy;
					//dx *= rev像素宽度m;
					//dy *= rev像素高度m;
					float rlength = MathF.ReciprocalSqrtEstimate(dx * dx + dy * dy + 1);
					float X = -dx * rlength;
					float Y = -dy * rlength;
					float Z = 1 * rlength;
					rowAddr[x * 3 + 0] = EncodeNormal(X);
					rowAddr[x * 3 + 1] = EncodeNormal(Y);
					rowAddr[x * 3 + 2] = EncodeNormal(Z);
					continue;

					static int ij2idx(int i, int j) => j * 3 + 3 + i + 1;

					float H(int x, int y) => sourceBytesPerChannel switch
					{
						1 => GetHeightInt8(SourceImageBuffer, x, y, width, height, sourceBpp, channelOffset) * heightScale,
						2 => GetHeightInt16(SourceImageBuffer, x, y, width, height, sourceBpp, channelOffset, isUnsigned, isBigEndian) * heightScale,
						_ => GetHeightFloat32(SourceImageBuffer, x, y, width, height, sourceBpp, channelOffset) * heightScale
					};
				}
			});
			WriteImageToFile(outputFile, targetMapBuffer, width, height, targetBufferSize, PixelFormats.Rgb24);
		}
		finally
		{
			FreeHGlobal(targetMapBuffer);
		}
	}

	static void WriteImageToFile(string filepath, IntPtr pixelBuffer, int width, int height, nint byteLength, PixelFormat pixelFormat)
	{
		if (byteLength > int.MaxValue)
		{
			filepath = Path.ChangeExtension(filepath, ".raw");
			using FileStream saveFile = new(filepath, FileMode.Create, FileAccess.Write);
			nint blockCnt = byteLength / int.MaxValue;
			for (nint i = 0; i < blockCnt; i++)
			{
				Span<byte> block = (pixelBuffer + i * int.MaxValue).AsPtr<byte>().AsSpan(int.MaxValue);
				saveFile.Write(block);
			}

			nint remain = pixelBuffer - blockCnt * int.MaxValue;
			if (remain > 0)
			{
				Span<byte> block = (pixelBuffer + int.MaxValue * blockCnt).AsPtr<byte>().AsSpan((int)remain);
				saveFile.Write(block);
			}

			saveFile.Flush();
			//MessageBox.Show($"已经写入到{filepath}");
		}
		else
		{
			filepath = Path.ChangeExtension(filepath, ".png");
			BitmapEncoder encoder = GetNewEncoder(".png");
			encoder.Frames.Add(BitmapFrame.Create(BitmapSource.Create(width, height, 96, 96, pixelFormat, null, pixelBuffer, (int)byteLength, pixelFormat.BitsPerPixel / 8 * width)));
			using FileStream saveFile = new(filepath, FileMode.Create, FileAccess.Write);
			encoder.Save(saveFile);
			saveFile.Flush();
			//MessageBox.Show($"已经写入到{filepath}");
		}
	}

	static void LoadImage(
		ref IntPtr targetBuffer,
		out PixelStruct targetPixelStruct,
		out int width,
		out int height,
		Image targetImageBox,
		Label targetNameLabel,
		TextBlock targetUnderlyTextBlock,
		TextBlock targetInfoTextBlock,
		string filePath)
	{
		//string fileName = Path.GetFileName(filePath);
		string extension = Path.GetExtension(filePath);
		width = 0;
		height = 0;
		targetPixelStruct = default;
		if (!supportImageFormats.Contains(extension.ToLower()))
		{
			MessageBox.Show($"不支持的图片格式:{extension}");
			return;
		}

		targetNameLabel.Content = $"正在加载...";
		targetInfoTextBlock.Text = filePath;
		try
		{
			if (extension == ".raw")
			{
				FileInfo fileInfo = new(filePath);
				RawInfoDialog rawInfoDiag = new((nint)fileInfo.Length, Path.GetFileName(filePath));
				var rawDiagResult = rawInfoDiag.ShowDialog();
				if (!(rawDiagResult.HasValue && rawDiagResult.Value))
				{
					targetImageBox.Source = null;
					if (targetBuffer != IntPtr.Zero)
					{
						FreeHGlobal(targetBuffer);
						targetBuffer = IntPtr.Zero;
					}

					targetPixelStruct = default;
					targetNameLabel.Content = "输入图";
					targetUnderlyTextBlock.Text = "将输入图拖到此处";
					targetInfoTextBlock.Text = null;
				}
				else
				{
					width = rawInfoDiag.ImageWidth;
					height = rawInfoDiag.ImageHeight;
					targetPixelStruct.ChannelCount = rawInfoDiag.ChannelCount;
					targetPixelStruct.BytesPerChannel = rawInfoDiag.ChannelByteDepth;
					targetPixelStruct.BytesPerPixel = rawInfoDiag.ChannelCount * rawInfoDiag.ChannelByteDepth;
					using FileStream stream = File.OpenRead(filePath);
					nint dataSize = (nint)width * height * targetPixelStruct.BytesPerPixel;
					if (targetBuffer == IntPtr.Zero) targetBuffer = AllocHGlobal(dataSize);
					else ReAllocHGlobal(targetBuffer, dataSize);
					stream.Seek(rawInfoDiag.HeaderSize, SeekOrigin.Begin);
					if (rawInfoDiag.InterlacedCheckBox.IsChecked.IsTrue())
					{
						nint blocks = dataSize / int.MaxValue;
						for (nint i = 0; i < blocks; i++)
						{
							Span<byte> bufferSpan = (targetBuffer + i * int.MaxValue).AsPtr<byte>().AsSpan(int.MaxValue);
							stream.Read(bufferSpan);
						}

						int remain = (int)(dataSize - (blocks * int.MaxValue));
						if (remain > 0)
						{
							var bufferSpan = (targetBuffer + blocks * int.MaxValue).AsPtr<byte>().AsSpan(remain);
							stream.Read(bufferSpan);
						}
					}
					else
					{
						nint channelCount = rawInfoDiag.ChannelCount;
						nint oneChannelDataSize = dataSize / channelCount;
						for (nint c = 0; c < channelCount; c++)
						for (nint i = 0; i < oneChannelDataSize; i++)
						{
							byte b = (byte)stream.ReadByte();
							nint offset = i * channelCount + c;
							WriteByte(targetBuffer + offset, b);
						}
					}

					for (int i = 0; i < rawInfoDiag.ChannelCount; i++)
					{
						targetPixelStruct.WriteChannelOffset(i, rawInfoDiag.ChannelByteDepth * i);
					}

					PixelFormat f;
					if (rawInfoDiag.ChannelByteDepth == 1)
						if (rawInfoDiag.ChannelCount == 1) f = PixelFormats.Gray8;
						else f = PixelFormats.Rgb24;
					else if (rawInfoDiag.ChannelByteDepth == 2)
						if (rawInfoDiag.ChannelCount == 1) f = PixelFormats.Gray16;
						else if (rawInfoDiag.ChannelCount == 3) f = PixelFormats.Rgb48;
						else f = PixelFormats.Rgba64;
					else if (rawInfoDiag.ChannelByteDepth == 4)
						if (rawInfoDiag.ChannelCount == 1) f = PixelFormats.Gray32Float;
						else f = PixelFormats.Rgba128Float;
					else
					{
						targetImageBox.Source = null;
						targetNameLabel.Content = filePath;
						targetUnderlyTextBlock.Text = "(此格式暂不支持预览图,但可正常使用)";
						targetInfoTextBlock.Text = $"宽度:{width} 高度:{height} 通道位深:{targetPixelStruct.BytesPerChannel} 通道数量:{targetPixelStruct.ChannelCount}";
						return;
					}

					if (dataSize > int.MaxValue)
					{
						targetImageBox.Source = null;
						targetNameLabel.Content = filePath;
						targetUnderlyTextBlock.Text = "(图片过大暂不支持预览图,但可正常使用)";
						targetInfoTextBlock.Text = $"宽度:{width} 高度:{height} 通道位深:{targetPixelStruct.BytesPerChannel} 通道数量:{targetPixelStruct.ChannelCount}";
					}
					else
					{
						var source = BitmapSource.Create(width, height, 96, 96, f, null, targetBuffer, (int)dataSize, width * targetPixelStruct.BytesPerPixel);
						targetImageBox.Source = source;
						targetNameLabel.Content = filePath;
						targetUnderlyTextBlock.Text = "将输入图拖到此处";
						targetInfoTextBlock.Text = $"宽度:{width} 高度:{height} 通道位深:{targetPixelStruct.BytesPerChannel} 像素格式:{f}";
					}
				}
			}
			else
			{
				using FileStream stream = new(filePath, FileMode.Open, FileAccess.Read);
				BitmapDecoder decoder = GetNewDecoder(extension, stream);
				BitmapFrame frame = decoder.Frames[0];
				//把无压缩源图片数据存到targetBuffer里
				width = frame.PixelWidth;
				height = frame.PixelHeight;
				targetPixelStruct = GetPixelStructFromWPFFormat(frame.Format);
				int Bpp = targetPixelStruct.BytesPerPixel;
				nint bufferSize = (nint)width * height * Bpp;
				if (targetBuffer == IntPtr.Zero) targetBuffer = AllocHGlobal(bufferSize);
				else ReAllocHGlobal(targetBuffer, bufferSize);
				frame.CopyPixels(new(0, 0, width, height), targetBuffer, (int)bufferSize, width * Bpp);//Todo:bug
				targetImageBox.Source = frame;
				targetNameLabel.Content = filePath;
				targetUnderlyTextBlock.Text = "将输入图拖到此处";
				targetInfoTextBlock.Text = $"宽度:{width} 高度:{height} 通道位深:{targetPixelStruct.BytesPerChannel} 像素格式:{frame.Format}";
			}
		}
		catch (Exception fe)
		{
			targetImageBox.Source = null;
			if (targetBuffer != IntPtr.Zero)
			{
				FreeHGlobal(targetBuffer);
				targetBuffer = IntPtr.Zero;
			}

			targetPixelStruct = default;
			targetNameLabel.Content = "输入图";
			targetUnderlyTextBlock.Text = "将输入图拖到此处";
			targetInfoTextBlock.Text = null;
			MessageBox.Show($"{filePath}图片加载失败:\n{fe}");
		}
	}

	void SourceMapImageBox_Drop(object sender, DragEventArgs e) => LoadImage(
		ref SourceImageBuffer,
		out sourceImagePixelStruct,
		out sourceMapWidth,
		out sourceMapHeight,
		SourceMapImageBox,
		SourceMapNameLabel,
		SourceMapUnderImageTextBlock,
		SourceMapInfoTextBlock,
		(e.Data.GetData(DataFormats.FileDrop) as string[])[0]);

	void MaskMapImageBox_Drop(object sender, DragEventArgs e) => LoadImage(
		ref MaskImageBuffer,
		out maskImagePixelStruct,
		out maskMapWidth,
		out maskMapHeight,
		MaskMapImageBox,
		MaskMapNameLabel,
		MaskMapUnderImageTextBlock,
		MaskMapInfoTextBlock,
		(e.Data.GetData(DataFormats.FileDrop) as string[])[0]);

	static BitmapDecoder GetNewDecoder(string format,
		Stream bitmapStream,
		BitmapCreateOptions createOptions = BitmapCreateOptions.PreservePixelFormat,
		BitmapCacheOption cacheOption = BitmapCacheOption.OnLoad) => format switch
	{
		".jpeg" => new JpegBitmapDecoder(bitmapStream, createOptions, cacheOption),
		".jpg" => new JpegBitmapDecoder(bitmapStream, createOptions, cacheOption),
		".png" => new PngBitmapDecoder(bitmapStream, createOptions, cacheOption),
		".tif" => new TiffBitmapDecoder(bitmapStream, createOptions, cacheOption),
		".tiff" => new TiffBitmapDecoder(bitmapStream, createOptions, cacheOption),
		".bmp" => new BmpBitmapDecoder(bitmapStream, createOptions, cacheOption),
		_ => throw new FileFormatException("不正确的图片格式")
	};

	static BitmapEncoder GetNewEncoder(string format) => format switch
	{
		".jpeg" => new JpegBitmapEncoder(),
		".jpg" => new JpegBitmapEncoder(),
		".png" => new PngBitmapEncoder(),
		".tif" => new TiffBitmapEncoder(),
		".tiff" => new TiffBitmapEncoder(),
		".bmp" => new BmpBitmapEncoder(),
		_ => throw new FileFormatException("不正确的图片格式")
	};
}