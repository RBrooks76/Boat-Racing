using System;
using Unity.Collections;

namespace NWH.Common.Utility
{
    public static class ArrayExtensions
    {
        public static void Fill<T>(this T[] destinationArray, params T[] value)
        {
            if (destinationArray.Length == 0)
            {
                return;
            }
            
            // set the initial array value
            Array.Copy(value, destinationArray, value.Length);

            int arrayToFillHalfLength = destinationArray.Length / 2;
            int copyLength;

            for (copyLength = value.Length; copyLength < arrayToFillHalfLength; copyLength <<= 1)
            {
                Array.Copy(destinationArray, 0, destinationArray, copyLength, copyLength);
            }

            Array.Copy(destinationArray, 0, destinationArray, copyLength, destinationArray.Length - copyLength);
        }


        public static void FastCopyFrom<T>(this NativeArray<T> destination, T[] source) where T : struct
        {
            // The unsafe way is slightly faster (~3 percent effect on overall performance) but 
            // can cause issues on some devices and requires unsafe option to be enabled.
            /*
            unsafe
            {
                int byteLength = source.Length * UnsafeUtility.SizeOf(typeof(T));
                void* sourceManagedBuffer = UnsafeUtility.AddressOf(ref source[0]);
                void* destinationNativeBuffer = destination.GetUnsafePtr();
                UnsafeUtility.MemCpy(destinationNativeBuffer, sourceManagedBuffer, byteLength);
            }
            */
            destination.CopyFrom(source);
        }


        public static void FastCopyTo<T>(this NativeArray<T> source, T[] destination) where T : struct
        {
            /*
            unsafe
            {
                int byteLength = source.Length * UnsafeUtility.SizeOf(typeof(T));
                void* destinationManagedBuffer = UnsafeUtility.AddressOf(ref destination[0]);
                void* sourceNativeBuffer = source.GetUnsafePtr();
                UnsafeUtility.MemCpy(destinationManagedBuffer, sourceNativeBuffer, byteLength);
            }
            */
            source.CopyTo(destination);
        }
    }
}