using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
namespace StandCats
{
    public static class BinaryUtil
    {
        public unsafe static void ToBytesFloat(float v,ref byte[] dst, ref int offset)
        {
            fixed (byte* b = &dst[offset])
            {
                *((float*)b) = v;//offset分ズラした位置へ書き込み
                offset += 4;
            }
        }
        public unsafe static void ToBytesDouble(double v, ref byte[] dst, ref int offset)
        {
            fixed (byte* b = &dst[offset])
            {
                *((double*)b) = v;//offset分ズラした位置へ書き込み
                offset += 8;
            }
        }
        public unsafe static float ByBytesFloat(ref byte[] source,ref int offset)
        {
            float result;
             fixed (byte * b = &source[offset])
            {
                result = *(float*)b;
                offset += 4;
            }
            return result;
        }
        public unsafe static double ByBytesDouble(ref byte[] source, ref int offset)
        {
            double result;
            fixed (byte* b = &source[offset])
            {
                result = *(double*)b;
                offset += 8;
            }
            return result;
        }
        public unsafe static void ToBytesULong(ulong v, ref byte[] dst, ref int offset)
        {
            fixed (byte* b = &dst[offset])
            {
                *((ulong*)b) = v;//offset分ズラした位置へ書き込み
                offset += 8;
            }
        }
        public unsafe static void ToBytesLong(long v, ref byte[] dst, ref int offset)
        {
            fixed (byte* b = &dst[offset])
            {
                *((long*)b) = v;//offset分ズラした位置へ書き込み
                offset += 8;
            }
        }

        public unsafe static ulong ByBytesULong(ref byte[] source,ref int offset)
        {
            ulong result;
            fixed (byte* b = &source[offset])
            {
                result = *(ulong*)b;
                offset += 8;
            }
            return result;
        }
        public unsafe static long ByBytesLong(ref byte[] source, ref int offset)
        {
            long result;
            fixed (byte* b = &source[offset])
            {
                result = *(long*)b;
                offset += 8;
            }
            return result;
        }


        public unsafe static void ToByteUInt(uint v,ref byte[] dst,ref int offset)
        {
            fixed (byte* b = &dst[offset])
            {
                *((uint*)b) = v;
                offset += 4;
            }
        }
        public unsafe static uint ByByteUInt(ref byte[] source, ref int offset)
        {
            uint result;
            fixed (byte* b = &source[offset])
            {
                result = *(uint*)b;
                offset += 4;
            }
            return result;
        }

        public unsafe static void ToByteInt(int v, ref byte[] dst, ref int offset)
        {
            fixed (byte* b = &dst[offset])
            {
                *((int*)b) = v;
                offset += 4;
            }
        }
        public unsafe static int ByByteInt(ref byte[] source,ref int offset)
        {
            int result;
            fixed(byte* b = &source[offset])
            {
                result = *(int*)b;
                offset += 4;
            }
            return result;
        }

        public unsafe static void ToByteShort(short v, ref byte[] dst, ref int offset)
        {

            fixed (byte* b = &dst[offset])
            {
                *((short*)b) = v;//offset分ズラした位置へ書き込み
                offset += 2;
            }
        }
        public unsafe static void ToBytesUShort(ushort v, ref byte[] dst, ref int offset)
        {

            fixed (byte* b = &dst[offset])
            {
                *((ushort*)b) = v;//offset分ズラした位置へ書き込み
                offset += 2;
            }
        }

        public unsafe static short ByByteShort(ref byte[] source, ref int offset)
        {
            short result;
            fixed (byte* b = &source[offset])
            {
                result = *(short*)b;
                offset += 2;
            }
            return result;
        }

        public unsafe static ushort ByBytesUShort(ref byte[] source, ref int offset)
        {
            ushort result;
            fixed (byte* b = &source[offset])
            {
                result = *(ushort*)b;
                offset += 2;
            }
            return result;
        }

        public unsafe static void ToByteByte(byte v, ref byte[] dst, ref int offset)
        {
            dst[offset] = v;//offset分ズラした位置へ書き込み        
            offset += 1;
        }
        public unsafe static byte ByByteByte(ref byte[] source,ref int offset)
        {
            offset += 1;//offset を進めて元オフセットを読み込む
            return source[offset - 1];
        }
        public static void ToBytesBytes(ref byte[] v,ref byte[] dst,ref int offset,int len)
        {
            ToByteInt(len, ref dst, ref offset);
            Buffer.BlockCopy(v, 0, dst, offset, len);
            offset += len;
        }
        public static void ByBytesBytes(ref byte[] source,ref int sourceoffset,ref byte[] dst,int dstoffset)
        {
            var len = ByByteInt(ref source, ref sourceoffset);
            Buffer.BlockCopy(source, sourceoffset, dst, dstoffset, len);
            sourceoffset += len;
            dstoffset += len;
        }

        /// <summary>
        /// 配列の長さは255まで
        /// ゲーム用途であれば十二分
        /// </summary>
        /// <param name="source"></param>
        /// <param name="sourceoffset"></param>
        /// <param name="dst"></param>
        /// <param name="dstoffset"></param>
        /// <param name="sourcelen"></param>
        public static void ToBytesFloatArray(ref float[] source,ref byte[] dst,ref int dstoffset)
        {
            ToByteByte((byte)source.Length, ref dst, ref dstoffset);
            Buffer.BlockCopy(source, 0 , dst, dstoffset,source.Length * 4);
            dstoffset += source.Length * 4;
        }
        public static void ByBytesFloatArray(ref byte[] source,ref int sourceoffset,ref float[] dst)
        {
            var len = ByByteByte(ref source, ref sourceoffset);
            //境界チェック
           // if(dst.Length < len) { throw new IOException("Over dst Array["+dst.Length+"] readlength:"+len); }
            if(len*4 > source.Length) { throw new IOException("Over SourceArray[" + source.Length + "] readlength:" + len); }
            dst = new float[len];
            Buffer.BlockCopy(source, sourceoffset, dst, 0 , len * 4);
            sourceoffset += len * 4;
        }
    }
}
