using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Media;

namespace HeightNormalConverter;

static class BitmapUtils
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float GetHeightInt8(
        IntPtr target,
        int x,
        int y,
        int pixelWidth,
        int pixelHeight,
        int Bpp,
        int channelOffset)//小于0代表是灰度
    {
        nint wrapx = Mod(x, pixelWidth);
        nint wrapy = Mod(y, pixelHeight);
        nint MemAddress = target + wrapy * Bpp * pixelWidth + wrapx * Bpp;
        if (channelOffset < 0)
        {
            byte R = Marshal.ReadByte(MemAddress);
            byte G = Marshal.ReadByte(++MemAddress);
            byte B = Marshal.ReadByte(++MemAddress);
            float src = (R * 19595 + G * 38469 + B * 7472) >> 16;
            return 1 / 255f * src;
        }
        else
        {
            float src = Marshal.ReadByte(MemAddress + channelOffset);
            return 1 / 255f * src;
        }
    }
    /// <param name="x">目标像素的x</param>
    /// <param name="y">目标像素的y</param>
    /// <param name="Address">目标点阵图的头指针</param>
    /// <param name="pixelWidth">目标图片的像素宽度</param>
    /// <param name="pixelHeight">目标图片的像素高度</param>
    /// <param name="Bpp">每像素字节数</param>
    /// <param name="channelOffset">目标通道在像素中的第几个字节位置,小于0代表要计算灰度</param>
    /// <param name="unsigned">是无符号16位整数</param>
    /// <param name="isBigEndinan">目标数据是大端存储的,false则为小端</param>
    /// <returns>高程图的高度</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float GetHeightInt16(
        IntPtr Address,
        int x,
        int y,
        int pixelWidth,
        int pixelHeight,
        int Bpp,
        int channelOffset,
        bool unsigned,
        bool isBigEndinan)
    {
        nint wrapx = Mod(x, pixelWidth);
        nint wrapy = Mod(y, pixelHeight);
        nint MemAddress = Address + wrapy * Bpp * pixelWidth + wrapx * Bpp;
        if (channelOffset < 0)
        {
            ushort R = ReadInt16WithEndian(MemAddress + 0, isBigEndinan);
            ushort G = ReadInt16WithEndian(MemAddress + 2, isBigEndinan);
            ushort B = ReadInt16WithEndian(MemAddress + 4, isBigEndinan);
            if (unsigned) return R * 0.299f + G * 0.587f + B * 0.114f;
            else return Unsafe.As<ushort, short>(ref R) * 0.299f +
                        Unsafe.As<ushort, short>(ref G) * 0.587f +
                        Unsafe.As<ushort, short>(ref B) * 0.114f;
        }
        else
        {
            ushort result = ReadInt16WithEndian(MemAddress + channelOffset, isBigEndinan);
            if (unsigned) return result;
            else return Unsafe.As<ushort, short>(ref result);
        }

        static ushort ReadInt16WithEndian(IntPtr Address, bool isBigEndinan)
        {
            byte b1 = Marshal.ReadByte(Address);
            byte b2 = Marshal.ReadByte(Address + 1);
            if (isBigEndinan) return (ushort)(b1 << 8 | b2);
            return (ushort)(b2 << 8 | b1);
        }
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float GetHeightFloat32(IntPtr Address, int x, int y, int pixelWidth, int pixelHeight, int Bpp, int channelOffset)
    {
        x = Mod(x, pixelWidth);
        y = Mod(y, pixelHeight);
        nint MemAddress = Address + y * Bpp * pixelWidth + x * Bpp;
        if (channelOffset < 0)
        {
            int RfloatBits = Marshal.ReadInt32(MemAddress);
            int GfloatBits = Marshal.ReadInt32(MemAddress + 4);
            int BfloatBits = Marshal.ReadInt32(MemAddress + 8);
            return Unsafe.As<int, float>(ref RfloatBits) * 0.299f +
                   Unsafe.As<int, float>(ref GfloatBits) * 0.587f +
                   Unsafe.As<int, float>(ref BfloatBits) * 0.114f;
        }
        else
        {
            int floatBits = Marshal.ReadInt32(MemAddress + channelOffset);
            return Unsafe.As<int, float>(ref floatBits);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Mod(int x, int y) => (x % y) + (x < 0 ? y : 0);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Remap01(float value, float min, float max) => (value - min) / (max - min);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Clamp(int value, int min, int max) => Math.Min(Math.Max(value, min), max);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Saturate(float value) => Math.Min(Math.Max(value, 0), 0.99999f);

    public static PixelStruct GetPixelStructFromWPFFormat(PixelFormat format)
    {
        int BytesPerPixel = format.BitsPerPixel >> 3;
        int ChannelCount = format.Masks.Count;
        int BytesPerChannel = BytesPerPixel / ChannelCount;
        int Roffset = 0;
        int Goffset = 0;
        int Boffset = 0;
        int Aoffset = 0;
        if (format == PixelFormats.Bgr32) { Roffset = 2; Goffset = 1; Boffset = 0; Aoffset = 0; }
        else if (format == PixelFormats.Bgra32) { Roffset = 2; Goffset = 1; Boffset = 0; Aoffset = 4; }
        else if (format == PixelFormats.Bgr24) { Roffset = 2; Goffset = 1; Boffset = 0; Aoffset = 0; }
        else if (format == PixelFormats.Rgb24) { Roffset = 0; Goffset = 1; Boffset = 2; Aoffset = 0; }
        else if (format == PixelFormats.Rgb48) { Roffset = 0; Goffset = 2; Boffset = 4; Aoffset = 0; }
        else if (format == PixelFormats.Rgba128Float) { Roffset = 0; Goffset = 4; Boffset = 8; Aoffset = 12; }
        else if (format == PixelFormats.Rgba64) { Roffset = 0; Goffset = 2; Boffset = 4; Aoffset = 6; }
        else if (format == PixelFormats.Rgb128Float) { Roffset = 0; Goffset = 4; Boffset = 8; Aoffset = 0; }
        return new()
        {
            Roffset = Roffset,
            Goffset = Goffset,
            Boffset = Boffset,
            Aoffset = Aoffset,
            BytesPerPixel = BytesPerPixel,
            ChannelCount = ChannelCount,
            BytesPerChannel = BytesPerChannel,
        };
    }
}
struct PixelStruct
{
    public int Roffset;
    public int Goffset;
    public int Boffset;
    public int Aoffset;
    public int BytesPerPixel;
    public int BytesPerChannel;
    public int ChannelCount;

    public void WriteChannelOffset(int RGBA, int offset)
    {
        switch (RGBA)
        {
            case 0: Roffset = offset; break;
            case 1: Goffset = offset; break;
            case 2: Boffset = offset; break;
            case 3: Aoffset = offset; break;
        }
    }
    public int GetChannelOffset(int RGBA) => RGBA switch
    {
        0 => Roffset,
        1 => Goffset,
        2 => Boffset,
        3 => Aoffset,
        _ => throw new ArgumentException($"Channel {RGBA} is not exist."),
    };
}
