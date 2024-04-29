using System;
using System.Runtime.CompilerServices;

namespace Velctor.Utils
{
	/// <summary>
	/// 对原生指针的封装。便于与.Net、Unity的五花八门的指针表示互转
	/// </summary>
	public struct Ptr<T> where T : unmanaged
	{
		private unsafe T* addr;
		public readonly unsafe ref T Target => ref *addr;

		public readonly unsafe bool IsNull => addr == null;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe Ptr(void* ptr) => addr = (T*)ptr;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe Ptr(IntPtr ptr) => addr = (T*)ptr;

		/// <summary> 获取指向当前非托管值类型的指针。不要对引用类型的对象的值字段使用，因为其内存地址不固定 </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe Ptr(ref T targetObj) 
		{
			fixed (T* p = &targetObj) {
				addr = p;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static unsafe implicit operator Ptr<T>(T* nativePtr) => new(nativePtr);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static unsafe implicit operator Ptr<T>(IntPtr nativePtr) => new(nativePtr);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static unsafe implicit operator T*(Ptr<T> nativePtr) => nativePtr.addr;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static unsafe explicit operator nint(Ptr<T> v) => (nint)v.addr;

		public unsafe ref T this[nint indx] => ref addr[indx];

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly unsafe Ptr<U> CastAs<U>() where U : unmanaged => new(addr);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly unsafe Span<T> AsSpan(int elemCount = 1) => new(addr, elemCount);

		public override readonly unsafe string ToString() => $"0x{(UIntPtr)addr:X16}";

		public static unsafe Ptr<T> operator +(Ptr<T> a, nint b) => a.addr + b;
		public static unsafe Ptr<T> operator ++(Ptr<T> a) => a.addr++;
		public static unsafe Ptr<T> operator --(Ptr<T> a) => a.addr--;
		public static unsafe bool operator ==(Ptr<T> a, Ptr<T> b) => a.addr == b.addr;
		public static unsafe bool operator !=(Ptr<T> a, Ptr<T> b) => a.addr != b.addr;
		public static unsafe bool operator >(Ptr<T> a, Ptr<T> b) => a.addr > b.addr;
		public static unsafe bool operator <(Ptr<T> a, Ptr<T> b) => a.addr < b.addr;

		public override readonly unsafe bool Equals(object obj) => addr == ((Ptr<T>)obj).addr;

		public override readonly unsafe int GetHashCode() => ((nint)addr).GetHashCode();
	}

	public static class Ptr
	{
		/// <summary> 获取指向当前非托管值类型的指针。不要对引用类型的对象的值字段使用，因为其内存地址不固定 </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Ptr<T> GetUnsafPtr<T>(ref this T target) where T : unmanaged => new(ref target);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static unsafe void* AsPtr(this IntPtr intPtr) => (void*)(intPtr);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Ptr<T> AsPtr<T>(this IntPtr intPtr) where T : unmanaged => new(intPtr);
	}
}
