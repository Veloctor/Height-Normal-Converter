﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Velctor.Utils;
using static System.Runtime.InteropServices.Marshal;
using static HeightNormalConverter.BitmapUtils;

namespace HeightNormalConverter;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
	private static readonly HashSet<string> supportImageFormats = [".png", ".jpg", ".jpeg", ".tif", ".tiff", ".bmp", ".raw"];
	private IntPtr SourceImageBuffer;
	PixelStruct sourceImagePixelStruct = default;
	int sourceMapWidth;
	int sourceMapHeight;
	IntPtr MaskImageBuffer;
	PixelStruct maskImagePixelStruct = default;
	int maskMapWidth;
	int maskMapHeight;

	public MainWindow()
	{
		InitializeComponent();
	}

	private void ConvertButton_Click(object sender, RoutedEventArgs e)
	{
		if (SourceImageBuffer == IntPtr.Zero) {
			ButtonInfoTextBox.Text = "请先放入源贴图";
			return;
		}
		ButtonInfoTextBox.Text = "正在转换...";
		if ((bool)RadioButtonH2N.IsChecked) {
			int width = sourceMapWidth;
			int height = sourceMapHeight;
			int sourceBytesPerChannel = sourceImagePixelStruct.BytesPerChannel;
			int sourceChannelCount = sourceImagePixelStruct.ChannelCount;
			int sourceBpp = sourceImagePixelStruct.BytesPerPixel;
			int sourceStride = width * sourceBpp;
			int channelOffset = -1;
			if (HeightReadChannelR.IsChecked.HasValue && HeightReadChannelR.IsChecked.Value)
				channelOffset = sourceImagePixelStruct.GetChannelOffset(0);
			else if (HeightReadChannelG.IsChecked.HasValue && HeightReadChannelG.IsChecked.Value)
				channelOffset = sourceImagePixelStruct.GetChannelOffset(1);
			else if (HeightReadChannelB.IsChecked.HasValue && HeightReadChannelB.IsChecked.Value)
				channelOffset = sourceImagePixelStruct.GetChannelOffset(2);
			else if (HeightReadChannelA.IsChecked.HasValue && HeightReadChannelA.IsChecked.Value)
				channelOffset = sourceImagePixelStruct.GetChannelOffset(3);
			bool isUnsigned = (bool)Unsigned16BitCheckBox.IsChecked;
			bool isBigEndian = (bool)BigEndianCheckBox.IsChecked;
			float heightScale = float.Parse(RealHeightMaxBox.Text) - float.Parse(RealHeightMinBox.Text);
			int targetStride = width * 3;
			nint targetBufferSize = (nint)targetStride * height;
			IntPtr targetMapBuffer = AllocHGlobal(targetBufferSize);
			bool dxGChannel = (bool)NormalGChannelModeDX.IsChecked;
			Parallel.For(0, height, y => {
				Ptr<byte> rowAddr = targetMapBuffer + (nint)y * targetStride;//不转换nint的话,大图可能超4G值溢出
				/*
				#region 生成星球经纬高程法向量
				const double 赤道周长m = Math.PI * 2 * 1737100;
				double 纬度rad = -(double)y / height * Math.PI + (Math.PI / 2);
				double 纬度周长m = Math.Cos(纬度rad) * 赤道周长m;
				float rev像素宽度m = (float)(width / 纬度周长m);//横向分辨率的倒数
				float rev像素高度m = (float)(height / (赤道周长m / 2));//纵向分辨率的倒数
				#endregion
				*/
				for (int x = 0; x < width; x++) {
					float dx = H(x + 1, y - 1) + (2 * H(x + 1, y)) + H(x + 1, y + 1) - (H(x - 1, y - 1) + (2 * H(x - 1, y)) + H(x - 1, y + 1));
					float dy = H(x - 1, y - 1) + (2 * H(x, y - 1)) + H(x + 1, y - 1) - (H(x - 1, y + 1) + (2 * H(x, y + 1)) + H(x + 1, y + 1));
					if (dxGChannel) dy = -dy;
					dx *= 1f / 8;
					dy *= 1f / 8;
					float rlength = MathF.ReciprocalSqrtEstimate(dx * dx + dy * dy + 1);
					float X = -dx * rlength;
					float Y = -dy * rlength;
					float Z = 1 * rlength;
					#region 生成星球经纬高程法向量
					//dx *= rev像素宽度m;
					//dy *= rev像素高度m;
					#endregion
					//用于经纬空间法向
					rowAddr[x * 3 + 0] = EncodeNormal(X);
					rowAddr[x * 3 + 1] = EncodeNormal(Y);
					rowAddr[x * 3 + 2] = EncodeNormal(Z);

					float H(int x, int y) => sourceBytesPerChannel switch {
						1 => GetHeightInt8(SourceImageBuffer, x, y, width, height, sourceBpp, channelOffset) * heightScale,
						2 => GetHeightInt16(SourceImageBuffer, x, y, width, height, sourceBpp, channelOffset, isUnsigned, isBigEndian) * heightScale,
						_ => GetHeightFloat32(SourceImageBuffer, x, y, width, height, sourceBpp, channelOffset) * heightScale
					};
				}
			});

			if (targetBufferSize > int.MaxValue) {
				string name = SourceMapNameLabel.Content.ToString();
				name = Path.ChangeExtension(name, "_Normal.raw");
				using FileStream saveFile = new(name, FileMode.Create, FileAccess.Write);
				nint blockCnt = targetBufferSize / int.MaxValue;
				for (nint i = 0; i < blockCnt; i++) {
					Span<byte> block = (targetMapBuffer + i * int.MaxValue).AsPtr<byte>().AsSpan(int.MaxValue);
					saveFile.Write(block);
				}
				nint remain = targetBufferSize - blockCnt * int.MaxValue;
				if (remain > 0) {
					Span<byte> block = (targetMapBuffer + int.MaxValue * blockCnt).AsPtr<byte>().AsSpan((int)remain);
					saveFile.Write(block);
				}
				saveFile.Flush();
				MessageBox.Show($"已经写入到{name}");
			}
			else {
				BitmapEncoder encoder = GetNewEncoder(".png");
				encoder.Frames.Add(BitmapFrame.Create(BitmapSource.Create(width, height, 96, 96, PixelFormats.Rgb24, null, targetMapBuffer, (int)targetBufferSize, targetStride)));
				string name = SourceMapNameLabel.Content.ToString();
				name = Path.ChangeExtension(name, "_Normal.png");
				using FileStream saveFile = new(name, FileMode.Create, FileAccess.Write);
				encoder.Save(saveFile);
				saveFile.Flush();
				MessageBox.Show($"已经写入到{name}");
			}
			FreeHGlobal(targetMapBuffer);
		}
		else {
			ButtonInfoTextBox.Text = "暂未实现";
			return;
		}
		ButtonInfoTextBox.Text = null;
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
		if (!supportImageFormats.Contains(extension.ToLower())) {
			MessageBox.Show($"不支持的图片格式:{extension}");
			return;
		}
		targetNameLabel.Content = $"正在加载...";
		targetInfoTextBlock.Text = filePath;
		try {
			if (extension == ".raw") {
				FileInfo fileInfo = new(filePath);
				RawInfoDialog rawInfoDiag = new((nint)fileInfo.Length, Path.GetFileName(filePath));
				if (rawInfoDiag.ShowDialog() == true) {
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
					if ((bool)rawInfoDiag.InterlacedCheckBox.IsChecked) {
						for (nint i = 0; i < dataSize; i++) {
							WriteByte(targetBuffer + i, (byte)stream.ReadByte());
						}
					}
					else {
						nint channelCount = rawInfoDiag.ChannelCount;
						nint oneChannelDataSize = dataSize / channelCount;
						for (nint c = 0; c < channelCount; c++)
							for (nint i = 0; i < oneChannelDataSize; i++) {
								byte b = (byte)stream.ReadByte();
								nint offset = i * channelCount + c;
								WriteByte(targetBuffer + offset, b);
							}
					}
					for (int i = 0; i < rawInfoDiag.ChannelCount; i++) {
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
					else {
						targetImageBox.Source = null;
						targetNameLabel.Content = filePath;
						targetUnderlyTextBlock.Text = "(此格式暂不支持预览图,但可正常使用)";
						targetInfoTextBlock.Text = $"宽度:{width} 高度:{height} 通道位深:{targetPixelStruct.BytesPerChannel} 通道数量:{targetPixelStruct.ChannelCount}";
						return;
					}
					if (dataSize > int.MaxValue) {
						targetImageBox.Source = null;
						targetNameLabel.Content = filePath;
						targetUnderlyTextBlock.Text = "(图片过大暂不支持预览图,但可正常使用)";
						targetInfoTextBlock.Text = $"宽度:{width} 高度:{height} 通道位深:{targetPixelStruct.BytesPerChannel} 通道数量:{targetPixelStruct.ChannelCount}";
					}
					else {
						var source = BitmapSource.Create(width, height, 96, 96, f, null, targetBuffer, (int)dataSize, width * targetPixelStruct.BytesPerPixel);
						targetImageBox.Source = source;
						targetNameLabel.Content = filePath;
						targetUnderlyTextBlock.Text = "将输入图拖到此处";
						targetInfoTextBlock.Text = $"宽度:{width} 高度:{height} 通道位深:{targetPixelStruct.BytesPerChannel} 像素格式:{f}";
					}
				}
				else {
					targetImageBox.Source = null;
					if (targetBuffer != IntPtr.Zero) {
						FreeHGlobal(targetBuffer);
						targetBuffer = IntPtr.Zero;
					}
					targetPixelStruct = default;
					targetNameLabel.Content = "输入图";
					targetUnderlyTextBlock.Text = "将输入图拖到此处";
					targetInfoTextBlock.Text = null;
				}
			}
			else {
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
				frame.CopyPixels(new Int32Rect(0, 0, width, height), targetBuffer, (int)bufferSize, width * Bpp);
				targetImageBox.Source = frame;
				targetNameLabel.Content = filePath;
				targetUnderlyTextBlock.Text = "将输入图拖到此处";
				targetInfoTextBlock.Text = $"宽度:{width} 高度:{height} 通道位深:{targetPixelStruct.BytesPerChannel} 像素格式:{frame.Format}";
			}
		}
		catch (Exception fe) {
			targetImageBox.Source = null;
			if (targetBuffer != IntPtr.Zero) {
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
									   BitmapCacheOption cacheOption = BitmapCacheOption.OnLoad) => format switch {
										   ".jpeg" => new JpegBitmapDecoder(bitmapStream, createOptions, cacheOption),
										   ".jpg" => new JpegBitmapDecoder(bitmapStream, createOptions, cacheOption),
										   ".png" => new PngBitmapDecoder(bitmapStream, createOptions, cacheOption),
										   ".tif" => new TiffBitmapDecoder(bitmapStream, createOptions, cacheOption),
										   ".tiff" => new TiffBitmapDecoder(bitmapStream, createOptions, cacheOption),
										   ".bmp" => new BmpBitmapDecoder(bitmapStream, createOptions, cacheOption),
										   _ => throw new FileFormatException("不正确的图片格式")
									   };
	static BitmapEncoder GetNewEncoder(string format) => format switch {
		".jpeg" => new JpegBitmapEncoder(),
		".jpg" => new JpegBitmapEncoder(),
		".png" => new PngBitmapEncoder(),
		".tif" => new TiffBitmapEncoder(),
		".tiff" => new TiffBitmapEncoder(),
		".bmp" => new BmpBitmapEncoder(),
		_ => throw new FileFormatException("不正确的图片格式")
	};
}
