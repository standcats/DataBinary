using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Linq.Expressions;
namespace StandCats
{
    public class ByteBufferPool
    {

    }
    /// <summary>
    /// シリアライザー
    /// デシリアライザーの保存先
    /// </summary>
    public abstract class DataBinaryBase
    {
        public static Hashtable Cache;

        public Dictionary<byte, int> CacheLength;//Codeと長さ
        public Dictionary<byte, byte[]> CacheData;//Codeとデータ
        public Dictionary<byte, int> CacheIndex;
        public bool IsParent(Type type,Type parent)
        {
            if(type.BaseType == null)
            {
                return false;
            }else
            if(type.BaseType == parent)
            {
                return true;
            }else
            {
                return IsParent(type.BaseType,parent);
            }
        }

        public abstract bool InnerToBinary(ref byte[] dst,ref int offset);
        public abstract bool InnerByBinary(ref byte[] source,ref int offset);
        public bool ToBinary(ref byte[] dst,ref int offset)
        {
            InnerBufferPool.StackReset();
            return InnerToBinary(ref dst, ref offset);
        }
        public bool ByBinary(ref byte[] source,ref int offset)
        {
            InnerBufferPool.StackReset();
            return InnerByBinary(ref source, ref offset);
        }
    }

   /// <summary>
   /// Zipクラスに詰め込めるDataBinary
   /// 
   /// </summary>
   /// <typeparam name="T"></typeparam>
    public abstract class ZippedDataBinary<T> : DataBinary<T> where T : ZippedDataBinary<T>,new()
    {
        public abstract byte ZipCode { get; }
    }
    //https://stackoverflow.com/questions/8827649/fastest-way-to-convert-int-to-4-bytes-in-c-sharp
    //Unsafeを使って高速化
    //BitConverterもほぼ同等の操作

    //https://ufcpp.wordpress.com/2015/02/11/c-7%E3%81%AB%E5%90%91%E3%81%91%E3%81%A69-method-contracts/ 契約プログラミング





    public class DataBinary<T> : DataBinaryBase where T : DataBinary<T> , new()
    {
        static DataBinary()
        {
            //クラス名から自身のインデックスを作成
            //var name = nameof(T);//C#6でないと使えないのでオミット
            //var type = typeof(T);
            ////シリアライザー作成

            //foreach (var properties in type.GetProperties())
            //{


            //}
        }

        /// <summary>
        /// |headerlength(1~2byte)|headerdata(0~ 65,535byte)|
        /// ヘッダーデータ
        /// |code(1byte)|index(4byte)| 
        /// code:識別ID
        /// index:このバイナリ上の位置
        /// 
        /// 
        /// </summary>
        /// <param name="dst"></param>
        /// <returns></returns>
        public override bool InnerToBinary(ref byte[] dst,ref int offset)
        {
            CacheLength = InstancePool<Dictionary<byte, int>>.Rent();
            CacheData =   InstancePool<Dictionary<byte, byte[]>>.Rent();
            CacheLength.Clear();
            CacheData.Clear();
            var type = typeof(T);
            HashSet<byte> Codes = new HashSet<byte>();
            var Exprlist = new List<Action<DataBinary<T>>>(); 
            foreach(var properties in type.GetProperties())
            {
                var attr = properties.GetCustomAttribute<StandCatsCodeAttribute>();
                if(attr != null)
                {
                    var code = attr.Code;
                    var proptype = properties.PropertyType;
                    if(proptype == typeof(short))
                    {
                        var method = typeof(T).GetMethod(nameof(SetShort), new Type[] { typeof(byte).MakeByRefType(), typeof(short).MakeByRefType() });
                        var target = Expression.Parameter(typeof(DataBinary<T>), "target");
                        var codelamda = Expression.Constant(code);
                        var propcall = Expression.Property(Expression.Convert(target, typeof(T)), properties.Name);//子クラスにキャストしてPropertyからゲット
                        var callmethod = Expression.Call(target, method, codelamda, propcall);
                        var tmplamda = Expression.Lambda<Action<DataBinary<T>>>(callmethod, target);
                        Exprlist.Add(tmplamda.Compile());
                    }else
                    if(proptype == typeof(ushort))
                    {
                        var method = typeof(T).GetMethod(nameof(SetUShort), new Type[] { typeof(byte).MakeByRefType(), typeof(ushort).MakeByRefType() });
                        var target = Expression.Parameter(typeof(DataBinary<T>), "target");
                        var codelamda = Expression.Constant(code);
                        var propcall = Expression.Property(Expression.Convert(target, typeof(T)), properties.Name);//子クラスにキャストしてPropertyからゲット
                        var callmethod = Expression.Call(target, method, codelamda, propcall);
                        var tmplamda = Expression.Lambda<Action<DataBinary<T>>>(callmethod, target);
                        Exprlist.Add(tmplamda.Compile());
                    }
                    else
                    if(proptype == typeof(int))
                    {
                        var method = typeof(T).GetMethod(nameof(SetInt),new Type[] {typeof(byte).MakeByRefType(),typeof(int).MakeByRefType() });
                        var target = Expression.Parameter(typeof(DataBinary<T>), "target");
                        var codelamda = Expression.Constant(code);
                        var propcall = Expression.Property(Expression.Convert(target,typeof(T)), properties.Name);//子クラスにキャストしてPropertyからゲット
                        var callmethod = Expression.Call(target, method, codelamda, propcall);
                        var tmplamda = Expression.Lambda<Action<DataBinary<T>>>(callmethod, target);                                                                       
                        Exprlist.Add(tmplamda.Compile()); 

                    }else
                    if(proptype == typeof(uint))
                    {
                        var method = typeof(T).GetMethod(nameof(SetUInt), new Type[] { typeof(byte).MakeByRefType(), typeof(uint).MakeByRefType() });
                        var target = Expression.Parameter(typeof(DataBinary<T>), "target");
                        var codelamda = Expression.Constant(code);
                        var propcall = Expression.Property(Expression.Convert(target, typeof(T)), properties.Name);//子クラスにキャストしてPropertyからゲット
                        var callmethod = Expression.Call(target, method, codelamda, propcall);
                        var tmplamda = Expression.Lambda<Action<DataBinary<T>>>(callmethod, target);
                        Exprlist.Add(tmplamda.Compile());
                    }else
                    if(proptype == typeof(long))
                    {
                        var method = typeof(T).GetMethod(nameof(SetLong), new Type[] { typeof(byte).MakeByRefType(), typeof(long).MakeByRefType() });
                        var target = Expression.Parameter(typeof(DataBinary<T>), "target");
                        var codelamda = Expression.Constant(code);
                        var propcall = Expression.Property(Expression.Convert(target, typeof(T)), properties.Name);//子クラスにキャストしてPropertyからゲット
                        var callmethod = Expression.Call(target, method, codelamda, propcall);
                        var tmplamda = Expression.Lambda<Action<DataBinary<T>>>(callmethod, target);
                        Exprlist.Add(tmplamda.Compile());
                    }else
                    if (proptype == typeof(ulong))
                    {
                        var method = typeof(T).GetMethod(nameof(SetULong), new Type[] { typeof(byte).MakeByRefType(), typeof(ulong).MakeByRefType() });
                        var target = Expression.Parameter(typeof(DataBinary<T>), "target");
                        var codelamda = Expression.Constant(code);
                        var propcall = Expression.Property(Expression.Convert(target, typeof(T)), properties.Name);//子クラスにキャストしてPropertyからゲット
                        var callmethod = Expression.Call(target, method, codelamda, propcall);
                        var tmplamda = Expression.Lambda<Action<DataBinary<T>>>(callmethod, target);
                        Exprlist.Add(tmplamda.Compile());
                    }else
                    if(proptype == typeof(float))
                    {
                        var method = typeof(T).GetMethod(nameof(SetFloat), new Type[] { typeof(byte).MakeByRefType(), typeof(float).MakeByRefType() });
                        var target = Expression.Parameter(typeof(DataBinary<T>), "target");
                        var codelamda = Expression.Constant(code);
                        var propcall = Expression.Property(Expression.Convert(target, typeof(T)), properties.Name);//子クラスにキャストしてPropertyからゲット
                        var callmethod = Expression.Call(target, method, codelamda, propcall);
                        var tmplamda = Expression.Lambda<Action<DataBinary<T>>>(callmethod, target);
                        Exprlist.Add(tmplamda.Compile());
                    }else
                    if(proptype == typeof(double))
                    {
                        var method = typeof(T).GetMethod(nameof(SetDouble), new Type[] { typeof(byte).MakeByRefType(), typeof(double).MakeByRefType() });
                        var target = Expression.Parameter(typeof(DataBinary<T>), "target");
                        var codelamda = Expression.Constant(code);
                        var propcall = Expression.Property(Expression.Convert(target, typeof(T)), properties.Name);//子クラスにキャストしてPropertyからゲット
                        var callmethod = Expression.Call(target, method, codelamda, propcall);
                        var tmplamda = Expression.Lambda<Action<DataBinary<T>>>(callmethod, target);
                        Exprlist.Add(tmplamda.Compile());
                    }else
                    if(proptype == typeof(float[]))
                    {
                        var method = typeof(T).GetMethod(nameof(SetFloatArray), new Type[] { typeof(byte).MakeByRefType(), typeof(float[]).MakeByRefType() });
                        var target = Expression.Parameter(typeof(DataBinary<T>), "target");
                        var codelamda = Expression.Constant(code);
                        var propcall = Expression.Property(Expression.Convert(target, typeof(T)), properties.Name);//子クラスにキャストしてPropertyからゲット
                        var callmethod = Expression.Call(target, method, codelamda, propcall);
                        var tmplamda = Expression.Lambda<Action<DataBinary<T>>>(callmethod, target);
                        Exprlist.Add(tmplamda.Compile());
                    }else
                    if(IsParent(proptype,typeof(DataBinaryBase)))
                    {             
                        //対象のクラスのToBinary          
                        var method = typeof(T).GetMethod(nameof(SetDataBinary), new Type[] { typeof(byte).MakeByRefType(), typeof(DataBinaryBase).MakeByRefType() });
                        var target = Expression.Parameter(typeof(DataBinary<T>), "target");
                        var codelamda = Expression.Constant(code);
                        var propcall = Expression.Property(Expression.Convert(target, typeof(T)), properties.Name);//子クラスにキャストしてPropertyからゲット
                        var callmethod = Expression.Call(target, method, codelamda, propcall);
                        var tmplamda = Expression.Lambda<Action<DataBinary<T>>>(callmethod, target);
                        Exprlist.Add(tmplamda.Compile());
                    }else
                    if(proptype.IsArray && IsParent(proptype.GetElementType(),typeof(DataBinaryBase)))
                    {
                        var method = typeof(T).GetMethod(nameof(SetDataBinaryArray), new Type[] { typeof(byte).MakeByRefType(), typeof(DataBinaryBase[]).MakeByRefType() });
                        var target = Expression.Parameter(typeof(DataBinary<T>), "target");
                        var codelamda = Expression.Constant(code);
                        var propcall = Expression.Property(Expression.Convert(target, typeof(T)), properties.Name);//子クラスにキャストしてPropertyからゲット
                        var callmethod = Expression.Call(target, method, codelamda, propcall);
                        var tmplamda = Expression.Lambda<Action<DataBinary<T>>>(callmethod, target);
                        Exprlist.Add(tmplamda.Compile());
                    }else
                    if(proptype.IsGenericType
                        && proptype.GetGenericTypeDefinition() == typeof(List<>)
                        && IsParent(proptype.GetGenericArguments()[0],typeof(DataBinaryBase))
                        )
                    {
                        var method = typeof(T).GetMethod(nameof(SetDataBinaryList), new Type[] { typeof(byte).MakeByRefType(), typeof(List<T>).MakeByRefType() });
                        var target = Expression.Parameter(typeof(DataBinary<T>), "target");
                        var codelamda = Expression.Constant(code);
                        var propcall = Expression.Property(Expression.Convert(target, typeof(T)), properties.Name);//子クラスにキャストしてPropertyからゲット
                        var callmethod = Expression.Call(target, method, codelamda, propcall);
                        var tmplamda = Expression.Lambda<Action<DataBinary<T>>>(callmethod, target);
                        Exprlist.Add(tmplamda.Compile());
                    }else
                    if(proptype.IsGenericType
                        && proptype.GetGenericTypeDefinition() == typeof(Dictionary<,>)
                        && IsParent(proptype.GetGenericArguments()[1],typeof(DataBinaryBase))
                        && proptype.GetGenericArguments()[0] == typeof(byte)
                        )
                    {//Dictionary<byte,DataBinary>
                        var method = typeof(T).GetMethod(nameof(SetDataBinaryByteDic), new Type[] { typeof(byte).MakeByRefType(), typeof(Dictionary<byte,T>).MakeByRefType() });
                        var target = Expression.Parameter(typeof(DataBinary<T>), "target");
                        var codelamda = Expression.Constant(code);
                        var propcall = Expression.Property(Expression.Convert(target, typeof(T)), properties.Name);//子クラスにキャストしてPropertyからゲット
                        var callmethod = Expression.Call(target, method, codelamda, propcall);
                        var tmplamda = Expression.Lambda<Action<DataBinary<T>>>(callmethod, target);
                        Exprlist.Add(tmplamda.Compile());



                    }else
                    if (proptype.IsGenericType
                        && proptype.GetGenericTypeDefinition() == typeof(Dictionary<,>)
                        && IsParent(proptype.GetGenericArguments()[1], typeof(DataBinaryBase))
                        && proptype.GetGenericArguments()[0] == typeof(int)
                        )
                    {
                        var method = typeof(T).GetMethod(nameof(SetDataBinaryIntDic), new Type[] { typeof(byte).MakeByRefType(), typeof(Dictionary<int, T>).MakeByRefType() });
                        var target = Expression.Parameter(typeof(DataBinary<T>), "target");
                        var codelamda = Expression.Constant(code);
                        var propcall = Expression.Property(Expression.Convert(target, typeof(T)), properties.Name);//子クラスにキャストしてPropertyからゲット
                        var callmethod = Expression.Call(target, method, codelamda, propcall);
                        var tmplamda = Expression.Lambda<Action<DataBinary<T>>>(callmethod, target);
                        Exprlist.Add(tmplamda.Compile());
                    }




                }
            }
            Action<DataBinary<T>> del = (d) =>
            {
                for (int i = 0; i < Exprlist.Count; i++)
                {
                    Exprlist[i](d);
                }
            };


            del(this);
            //WriteHeader
            //calcheader size
            var count = CacheLength.Count;//最大でbyte型
            //header に今回要素数を書き出し
            BinaryUtil.ToByteByte((byte)count, ref dst,ref offset);
            var dataoffset = offset+count * 5;//|code(1byte)|index(4byte)| データが始まる位置
            var initdataoffset = dataoffset;
            foreach (var d in CacheLength)
            {
                BinaryUtil.ToByteByte(d.Key, ref dst, ref offset);
                BinaryUtil.ToByteInt(dataoffset, ref dst, ref offset);//To CacheIndex 
                //BinaryUtil.ToByteInt(d.Value, ref dst, ref offset);
                dataoffset += d.Value;
            }
            if(initdataoffset != offset)
            {
                throw new Exception("Miss");
            }
            foreach(var d in CacheData)
            {
                var v = d.Value;
                var len = CacheLength[d.Key];
                //BinaryUtil.ToBytesBytes(ref v, ref dst, ref offset,len);
                Buffer.BlockCopy(v, 0, dst, offset, len);
                BufferPool.Default.Return(v);
                offset += len;
            }
            InstancePool<Dictionary<byte, int>>.Free(CacheLength);
            InstancePool<Dictionary<byte, byte[]>>.Free(CacheData);
            return true;
        }

        public void SetInt(ref byte code,ref int v)
        {
            var buff = BufferPool.Default.Rent();
            int offset = 0;
            BinaryUtil.ToByteInt(v, ref buff,ref offset);
            CacheLength.Add(code, offset);
            CacheData.Add(code, buff);
        }
        public void SetUInt(ref byte code,ref uint v)
        {
            var buff = BufferPool.Default.Rent();
            int offset = 0;
            BinaryUtil.ToByteUInt(v, ref buff, ref offset);
            CacheLength.Add(code, offset);
            CacheData.Add(code, buff);
        }
        public void SetShort(ref byte code, ref short v)
        {
            var buff = BufferPool.Default.Rent();
            int offset = 0;
            BinaryUtil.ToByteShort(v, ref buff, ref offset);
            CacheLength.Add(code, offset);
            CacheData.Add(code, buff);
        }

        public void SetUShort(ref byte code, ref ushort v)
        {
            var buff = BufferPool.Default.Rent();
            int offset = 0;
            BinaryUtil.ToBytesUShort(v, ref buff, ref offset);
            CacheLength.Add(code, offset);
            CacheData.Add(code, buff);
        }
        public void SetLong(ref byte code,ref long v)
        {
            var buff = BufferPool.Default.Rent();
            int offset = 0;
            BinaryUtil.ToBytesLong(v, ref buff, ref offset);
            CacheLength.Add(code, offset);
            CacheData.Add(code, buff);
        }
        public void SetULong(ref byte code,ref ulong v)
        {
            var buff = BufferPool.Default.Rent();
            int offset = 0;
            BinaryUtil.ToBytesULong(v, ref buff, ref offset);
            CacheLength.Add(code, offset);
            CacheData.Add(code, buff);
        }
        public void SetFloat(ref byte code,ref float v)
        {
            var buff = BufferPool.Default.Rent();
            int offset = 0;
            BinaryUtil.ToBytesFloat(v, ref buff, ref offset);
            CacheLength.Add(code, offset);
            CacheData.Add(code, buff);
        }
        public void SetDouble(ref byte code,ref double v)
        {
            var buff = BufferPool.Default.Rent();
            int offset = 0;
            BinaryUtil.ToBytesDouble(v, ref buff, ref offset);
            CacheLength.Add(code, offset);
            CacheData.Add(code, buff);
        }
        public void SetFloatArray(ref byte code,ref float[] v)
        {
            if (v != null)
            {
                var buff = BufferPool.Default.Rent();
                int offset = 0;
                BinaryUtil.ToBytesFloatArray(ref v, ref buff, ref offset);
                CacheLength.Add(code, offset);
                CacheData.Add(code, buff);
            }
        }

        public void SetDataBinary(ref byte code,ref DataBinaryBase v)
        {
            if (v != null)
            {
                var buff = BufferPool.Default.Rent();
                var dstbuff = BufferPool.Default.Rent();
                int offset = 0;
                v.InnerToBinary(ref buff, ref offset);
                int dstoffset = 0;
                BinaryUtil.ToBytesBytes(ref buff, ref dstbuff, ref dstoffset, offset);
                BufferPool.Default.Return(buff);
                CacheLength.Add(code, offset);
                CacheData.Add(code, dstbuff);
            }
        }
        public void SetDataBinaryArray(ref byte code, ref DataBinaryBase[] v)
        {
            if(v != null && v.Length > 0)
            {
                var buff = BufferPool.Default.Rent();
                int offset = 0;
                ToBytesDataBinaryArray(ref v, ref buff, ref offset);
                CacheLength.Add(code, offset);
                CacheData.Add(code, buff);
            }
        }
        public static void ToBytesDataBinaryArray(ref DataBinaryBase[] source, ref byte[] dst, ref int dstoffset)
        {
            BinaryUtil.ToByteByte((byte)source.Length, ref dst, ref dstoffset);
            for (int i = 0; i < source.Length; i++)
            {
                var buff = BufferPool.Default.Rent();
                int inneroffset = 0;
                source[i].InnerToBinary(ref buff, ref inneroffset);
                BinaryUtil.ToBytesBytes(ref buff, ref dst, ref dstoffset, inneroffset);
                BufferPool.Default.Return(buff);//dstへコピーしたので返す
            }
        }
        public void SetDataBinaryList(ref byte code, ref List<T> v)
        {
            if (v != null && v.Count > 0)
            {
                var buff = BufferPool.Default.Rent();
                int offset = 0;
                ToBytesDataBinaryList(ref v, ref buff, ref offset);
                CacheLength.Add(code, offset);
                CacheData.Add(code, buff);
            }
        }
        public void SetDataBinaryByteDic(ref byte code,ref Dictionary<byte,T> v)
        {
            if(v != null && v.Count > 0)
            {
                var buff = BufferPool.Default.Rent();
                int offset = 0;
                ToBytesDataBinaryByteDic(ref v, ref buff, ref offset);
                CacheLength.Add(code, offset);
                CacheData.Add(code, buff);
            }
        }
        public static void ToBytesDataBinaryByteDic(ref Dictionary<byte,T> dic ,ref byte[] dst, ref int dstoffset)
        {
            BinaryUtil.ToByteByte((byte)dic.Count, ref dst, ref dstoffset);
            foreach(var pair in dic)
            {
                var buff = BufferPool.Default.Rent();
                int inneroffset = 0;
                BinaryUtil.ToByteByte(pair.Key, ref buff, ref inneroffset);
                pair.Value.InnerToBinary(ref buff, ref inneroffset);
                BinaryUtil.ToBytesBytes(ref buff, ref dst, ref dstoffset, inneroffset);
                BufferPool.Default.Return(buff);
            }
        }
        public void SetDataBinaryIntDic(ref byte code, ref Dictionary<int, T> v)
        {
            if (v != null && v.Count > 0)
            {
                var buff = BufferPool.Default.Rent();
                int offset = 0;
                ToBytesDataBinaryIntDic(ref v, ref buff, ref offset);
                CacheLength.Add(code, offset);
                CacheData.Add(code, buff);
            }
        }
        public static void ToBytesDataBinaryIntDic(ref Dictionary<int, T> dic, ref byte[] dst, ref int dstoffset)
        {
            BinaryUtil.ToByteByte((byte)dic.Count, ref dst, ref dstoffset);
            foreach (var pair in dic)
            {
                var buff = BufferPool.Default.Rent();
                int inneroffset = 0;
                BinaryUtil.ToByteInt(pair.Key, ref buff, ref inneroffset);
                pair.Value.InnerToBinary(ref buff, ref inneroffset);
                BinaryUtil.ToBytesBytes(ref buff, ref dst, ref dstoffset, inneroffset);
                BufferPool.Default.Return(buff);
            }
        }

        public static void ToBytesDataBinaryList(ref List<T> source, ref byte[] dst, ref int dstoffset)
        {
            BinaryUtil.ToByteByte((byte)source.Count, ref dst, ref dstoffset);
            for (int i = 0; i < source.Count; i++)
            {
                var buff = BufferPool.Default.Rent();
                int inneroffset = 0;
                source[i].InnerToBinary(ref buff, ref inneroffset);
                BinaryUtil.ToBytesBytes(ref buff, ref dst, ref dstoffset, inneroffset);
                BufferPool.Default.Return(buff);
            }
        }



        public short ReadShort(ref byte code, ref byte[] source)
        {
            int index;
            if (CacheIndex.TryGetValue(code, out index))
            {

                return BinaryUtil.ByByteShort(ref source, ref index);
            }
            return 0;
        }
        public ushort ReadUShort(ref byte code, ref byte[] source)
        {
            int index;
            if (CacheIndex.TryGetValue(code, out index))
            {
                return BinaryUtil.ByBytesUShort(ref source, ref index);
            }
            return 0;
        }

        /// <summary>
        /// ヘッダーからソースインデックスの位置を読み込んでリターン
        /// </summary>
        /// <param name="code"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public int ReadInt(ref byte code, ref byte[] source)
        {
            int index;
            if (CacheIndex.TryGetValue(code, out index))
            {

                return BinaryUtil.ByByteInt(ref source, ref index);
            }
            return 0;
        }
        public uint ReadUInt(ref byte code,ref byte[] source)
        {
            int index;
            if (CacheIndex.TryGetValue(code, out index))
            {
                return BinaryUtil.ByByteUInt(ref source, ref index);
            }
            return 0;
        }
        public long ReadLong(ref byte code,ref byte[] source)
        {
            int index;
            if(CacheIndex.TryGetValue(code,out index))
            {
                return BinaryUtil.ByBytesLong(ref source, ref index);
            }
            return 0;
        }
        public ulong ReadULong(ref byte code,ref byte[] source)
        {
            int index;
            if (CacheIndex.TryGetValue(code, out index))
            {
                return BinaryUtil.ByBytesULong(ref source, ref index);
            }
            return 0;
        }
        public float ReadFloat(ref byte code,ref byte[] source)
        {
            int index;
            if (CacheIndex.TryGetValue(code, out index))
            {
                return BinaryUtil.ByBytesFloat(ref source, ref index);
            }
            return 0;
        }

        public double ReadDouble(ref byte code, ref byte[] source)
        {
            int index;
            if (CacheIndex.TryGetValue(code, out index))
            {
                return BinaryUtil.ByBytesDouble(ref source, ref index);
            }
            return 0;
        }
        public float[] ReadFloatArray(ref byte code, ref byte[] source)
        {
            float[] dst = null;
            int index;
            if (CacheIndex.TryGetValue(code, out index))
            {
                BinaryUtil.ByBytesFloatArray(ref source, ref index,ref dst);
            }
            return dst;
        }
        public static T ReadDataBinary(ref DataBinaryBase holder, ref byte code,ref byte[] source)
        {
            T dst = null;
            int index;
            if (holder.CacheIndex.TryGetValue(code, out index))
            {
                dst = new T();
                var tmpbuff = BufferPool.Default.Rent();
                int offset = index;
                BinaryUtil.ByBytesBytes(ref source, ref offset, ref tmpbuff, 0);
                int inneroffset = 0;
                dst.InnerByBinary(ref tmpbuff, ref inneroffset);
                BufferPool.Default.Return(tmpbuff);
            }
            return dst;
        }
        public static T[] ReadDataBinaryArray(ref DataBinaryBase holder,ref byte code, ref byte[] source)
        {
            T[] dst = null;
            int index;
            if(holder.CacheIndex.TryGetValue(code,out index))
            {
                var count = BinaryUtil.ByByteByte(ref source, ref index);
                dst = new T[count];
                for(int i = 0; i < count;i++)
                {
                    dst[i] = new T();
                    var buff = BufferPool.Default.Rent();
                    BinaryUtil.ByBytesBytes(ref source, ref index, ref buff, 0);
                    int tmpindex = 0;
                    dst[i].InnerByBinary(ref buff, ref tmpindex);
                    BufferPool.Default.Return(buff);
                }
            }
            return dst;
        }
        public static List<T> ReadDataBinaryList(ref DataBinaryBase holder, ref byte code, ref byte[] source)
        {
            List<T> dst = null;
            int index;
            if (holder.CacheIndex.TryGetValue(code, out index))
            {
                var count = BinaryUtil.ByByteByte(ref source, ref index);
                dst = new List<T>(count);
                for (int i = 0; i < count; i++)
                {
                    dst.Add(new T());
                    var buff = BufferPool.Default.Rent();
                    BinaryUtil.ByBytesBytes(ref source, ref index, ref buff, 0);
                    int tmpindex = 0;
                    dst[i].InnerByBinary(ref buff, ref tmpindex);
                    BufferPool.Default.Return(buff);
                }
            }
            return dst;
        }
        public static Dictionary<byte,T> ReadDataBinaryByteDic(ref DataBinaryBase holder, ref byte code, ref byte[] source)
        {
            Dictionary<byte,T> dst = null;
            int index;
            if (holder.CacheIndex.TryGetValue(code, out index))
            {
                var count = BinaryUtil.ByByteByte(ref source, ref index);
                dst = new Dictionary<byte,T>(count);
                for (int i = 0; i < count; i++)
                {
                    var buff = BufferPool.Default.Rent();
                    BinaryUtil.ByBytesBytes(ref source, ref index, ref buff, 0);
                    int inneroffset = 0;
                    var key = BinaryUtil.ByByteByte(ref buff,ref inneroffset);
                    var tmpins = new T();
                    if(tmpins.InnerByBinary(ref buff, ref inneroffset) == false)
                    {
                        throw new Exception("Ellegal Binary");
                    }
                    dst.Add(key, tmpins);
                    BufferPool.Default.Return(buff);
                }
            }
            return dst;
        }
        public static Dictionary<int, T> ReadDataBinaryIntDic(ref DataBinaryBase holder, ref byte code, ref byte[] source)
        {
            Dictionary<int, T> dst = null;
            int index;
            if (holder.CacheIndex.TryGetValue(code, out index))
            {
                var count = BinaryUtil.ByByteByte(ref source, ref index);
                dst = new Dictionary<int, T>(count);
                for (int i = 0; i < count; i++)
                {
                    var buff = BufferPool.Default.Rent();
                    BinaryUtil.ByBytesBytes(ref source, ref index, ref buff, 0);
                    int inneroffset = 0;
                    var key = BinaryUtil.ByByteInt(ref buff, ref inneroffset);
                    var tmpins = new T();
                    if (tmpins.InnerByBinary(ref buff, ref inneroffset) == false)
                    {
                        throw new Exception("Ellegal Binary");
                    }
                    dst.Add(key, tmpins);
                    BufferPool.Default.Return(buff);
                }
            }
            return dst;
        }
        delegate void ReadByBins(DataBinary<T> t, ref byte[] source);
        public override bool InnerByBinary(ref byte[] source,ref int offset)
        {
            CacheIndex = InstancePool<Dictionary<byte, int>>.Rent();
            CacheIndex.Clear();
            //Readhead 
            var count = BinaryUtil.ByByteByte(ref source, ref offset);
            for(byte i = 0; i < count;i++)
            {
                var code = BinaryUtil.ByByteByte(ref source, ref offset);
                var index = BinaryUtil.ByByteInt(ref source, ref offset);
                CacheIndex.Add(code, index);
            }

            var Exprlist = new List<ReadByBins>();
            foreach (var properties in typeof(T).GetProperties())
            {
                var attr = properties.GetCustomAttribute<StandCatsCodeAttribute>();
                if (attr != null)
                {
                    var code = attr.Code;
                    var proptype = properties.PropertyType;
                    if (proptype == typeof(short))
                    {
                        var method = typeof(T).GetMethod(nameof(ReadShort), new Type[] { typeof(byte).MakeByRefType(), typeof(byte[]).MakeByRefType() });
                        var target = Expression.Parameter(typeof(DataBinary<T>), "target");
                        var codelamda = Expression.Constant(code);
                        var binlamda = Expression.Parameter(typeof(byte[]).MakeByRefType(), "source");
                        var readint = Expression.Call(target, method, codelamda, binlamda);
                        var propcall = Expression.Property(Expression.Convert(target, typeof(T)), properties.Name);//子クラスにキャストしてPropertyからゲット
                        var assing = Expression.Assign(propcall, readint);//target.prop = readint(code,ref source)
                        var lamda = Expression.Lambda<ReadByBins>(assing, target, binlamda);
                        Exprlist.Add(lamda.Compile());
                    }
                    else
                    if (proptype == typeof(ushort))
                    {
                        var method = typeof(T).GetMethod(nameof(ReadUShort), new Type[] { typeof(byte).MakeByRefType(), typeof(byte[]).MakeByRefType() });
                        var target = Expression.Parameter(typeof(DataBinary<T>), "target");
                        var codelamda = Expression.Constant(code);
                        var binlamda = Expression.Parameter(typeof(byte[]).MakeByRefType(), "source");
                        var readint = Expression.Call(target, method, codelamda, binlamda);
                        var propcall = Expression.Property(Expression.Convert(target, typeof(T)), properties.Name);//子クラスにキャストしてPropertyからゲット
                        var assing = Expression.Assign(propcall, readint);//target.prop = readint(code,ref source)
                        var lamda = Expression.Lambda<ReadByBins>(assing, target, binlamda);
                        Exprlist.Add(lamda.Compile());
                    }
                    else
                    if (proptype == typeof(int))
                    {
                        var method = typeof(T).GetMethod(nameof(ReadInt), new Type[] { typeof(byte).MakeByRefType(), typeof(byte[]).MakeByRefType() });
                        var target = Expression.Parameter(typeof(DataBinary<T>), "target");
                        var codelamda = Expression.Constant(code);
                        var binlamda = Expression.Parameter(typeof(byte[]).MakeByRefType(), "source");
                        var readint = Expression.Call(target, method, codelamda, binlamda);
                        var propcall = Expression.Property(Expression.Convert(target, typeof(T)), properties.Name);//子クラスにキャストしてPropertyからゲット
                        var assing = Expression.Assign(propcall, readint);//target.prop = readint(code,ref source)
                        var lamda = Expression.Lambda<ReadByBins>(assing, target, binlamda);
                        Exprlist.Add(lamda.Compile());
                    }
                    else
                    if (proptype == typeof(uint))
                    {
                        var method = typeof(T).GetMethod(nameof(ReadUInt), new Type[] { typeof(byte).MakeByRefType(), typeof(byte[]).MakeByRefType() });
                        var target = Expression.Parameter(typeof(DataBinary<T>), "target");
                        var codelamda = Expression.Constant(code);
                        var binlamda = Expression.Parameter(typeof(byte[]).MakeByRefType(), "source");
                        var readint = Expression.Call(target, method, codelamda, binlamda);
                        var propcall = Expression.Property(Expression.Convert(target, typeof(T)), properties.Name);//子クラスにキャストしてPropertyからゲット
                        var assing = Expression.Assign(propcall, readint);//target.prop = readint(code,ref source)
                        var lamda = Expression.Lambda<ReadByBins>(assing, target, binlamda);
                        Exprlist.Add(lamda.Compile());
                    }
                    else
                    if (proptype == typeof(long))
                    {
                        var method = typeof(T).GetMethod(nameof(ReadLong), new Type[] { typeof(byte).MakeByRefType(), typeof(byte[]).MakeByRefType() });
                        var target = Expression.Parameter(typeof(DataBinary<T>), "target");
                        var codelamda = Expression.Constant(code);
                        var binlamda = Expression.Parameter(typeof(byte[]).MakeByRefType(), "source");
                        var readint = Expression.Call(target, method, codelamda, binlamda);
                        var propcall = Expression.Property(Expression.Convert(target, typeof(T)), properties.Name);//子クラスにキャストしてPropertyからゲット
                        var assing = Expression.Assign(propcall, readint);//target.prop = readint(code,ref source)
                        var lamda = Expression.Lambda<ReadByBins>(assing, target, binlamda);
                        Exprlist.Add(lamda.Compile());
                    }
                    else
                    if (proptype == typeof(ulong))
                    {
                        var method = typeof(T).GetMethod(nameof(ReadULong), new Type[] { typeof(byte).MakeByRefType(), typeof(byte[]).MakeByRefType() });
                        var target = Expression.Parameter(typeof(DataBinary<T>), "target");
                        var codelamda = Expression.Constant(code);
                        var binlamda = Expression.Parameter(typeof(byte[]).MakeByRefType(), "source");
                        var readint = Expression.Call(target, method, codelamda, binlamda);
                        var propcall = Expression.Property(Expression.Convert(target, typeof(T)), properties.Name);//子クラスにキャストしてPropertyからゲット
                        var assing = Expression.Assign(propcall, readint);//target.prop = readint(code,ref source)
                        var lamda = Expression.Lambda<ReadByBins>(assing, target, binlamda);
                        Exprlist.Add(lamda.Compile());
                    }
                    else
                    if (proptype == typeof(float))
                    {
                        var method = typeof(T).GetMethod(nameof(ReadFloat), new Type[] { typeof(byte).MakeByRefType(), typeof(byte[]).MakeByRefType() });
                        var target = Expression.Parameter(typeof(DataBinary<T>), "target");
                        var codelamda = Expression.Constant(code);
                        var binlamda = Expression.Parameter(typeof(byte[]).MakeByRefType(), "source");
                        var readint = Expression.Call(target, method, codelamda, binlamda);
                        var propcall = Expression.Property(Expression.Convert(target, typeof(T)), properties.Name);//子クラスにキャストしてPropertyからゲット
                        var assing = Expression.Assign(propcall, readint);//target.prop = readint(code,ref source)
                        var lamda = Expression.Lambda<ReadByBins>(assing, target, binlamda);
                        Exprlist.Add(lamda.Compile());
                    }
                    else
                    if (proptype == typeof(double))
                    {
                        var method = typeof(T).GetMethod(nameof(ReadDouble), new Type[] { typeof(byte).MakeByRefType(), typeof(byte[]).MakeByRefType() });
                        var target = Expression.Parameter(typeof(DataBinary<T>), "target");
                        var codelamda = Expression.Constant(code);
                        var binlamda = Expression.Parameter(typeof(byte[]).MakeByRefType(), "source");
                        var readint = Expression.Call(target, method, codelamda, binlamda);
                        var propcall = Expression.Property(Expression.Convert(target, typeof(T)), properties.Name);//子クラスにキャストしてPropertyからゲット
                        var assing = Expression.Assign(propcall, readint);//target.prop = readint(code,ref source)
                        var lamda = Expression.Lambda<ReadByBins>(assing, target, binlamda);
                        Exprlist.Add(lamda.Compile());
                    }
                    else
                    if (proptype == typeof(float[]))
                    {
                        var method = typeof(T).GetMethod(nameof(ReadFloatArray), new Type[] { typeof(byte).MakeByRefType(), typeof(byte[]).MakeByRefType() });
                        var target = Expression.Parameter(typeof(DataBinary<T>), "target");
                        var codelamda = Expression.Constant(code);
                        var binlamda = Expression.Parameter(typeof(byte[]).MakeByRefType(), "source");
                        var readint = Expression.Call(target, method, codelamda, binlamda);
                        var propcall = Expression.Property(Expression.Convert(target, typeof(T)), properties.Name);//子クラスにキャストしてPropertyからゲット
                        var assing = Expression.Assign(propcall, readint);//target.prop = readint(code,ref source)
                        var lamda = Expression.Lambda<ReadByBins>(assing, target, binlamda);
                        Exprlist.Add(lamda.Compile());
                    }
                    else
                    if (IsParent(proptype, typeof(DataBinaryBase)))
                    {
                        var method = proptype.BaseType.GetMethods(BindingFlags.Public|BindingFlags.Static).Where( m
                         =>
                        {
                            var name = m.Name.Equals(nameof(ReadDataBinary));
                            if(name == false){return false;}
                            var parameters =  m.GetParameters();
                            if(parameters.Count() != 3) { return false; }
                            var databins = parameters[0].ParameterType == typeof(DataBinaryBase).MakeByRefType();
                            var codearg = parameters[1].ParameterType == typeof(byte).MakeByRefType();
                            var sourcearg = parameters[2].ParameterType == typeof(byte[]).MakeByRefType();
                            return databins && codearg && sourcearg;
                            }).FirstOrDefault();
                        var target = Expression.Parameter(typeof(DataBinary<T>), "target");
                        var codelamda = Expression.Constant(code);
                        var binlamda = Expression.Parameter(typeof(byte[]).MakeByRefType(), "source");
                        var readint = Expression.Call(method,target,codelamda, binlamda);
                        var propcall = Expression.Property(Expression.Convert(target, typeof(T)), properties.Name);//子クラスにキャストしてPropertyからゲット
                        var assing = Expression.Assign(propcall, readint);//target.prop = readint(code,ref source)
                        var lamda = Expression.Lambda<ReadByBins>(assing, target, binlamda);
                        Exprlist.Add(lamda.Compile());
                    }else
                    if(proptype.IsArray && IsParent(proptype.GetElementType(),typeof(DataBinaryBase)))
                    {
                        var elementtype = proptype.GetElementType().BaseType.GetMethods(BindingFlags.Public | BindingFlags.Static);
                        var method = proptype.GetElementType().BaseType.GetMethods(BindingFlags.Public | BindingFlags.Static).Where(m
  =>
                        {
                            var name = m.Name.Equals(nameof(ReadDataBinaryArray));
                            if (name == false) { return false; }
                            var parameters = m.GetParameters();
                            if (parameters.Count() != 3) { return false; }
                            var databins = parameters[0].ParameterType == typeof(DataBinaryBase).MakeByRefType();
                            var codearg = parameters[1].ParameterType == typeof(byte).MakeByRefType();
                            var sourcearg = parameters[2].ParameterType == typeof(byte[]).MakeByRefType();
                            return databins && codearg && sourcearg;
                        }).FirstOrDefault();
                        var target = Expression.Parameter(typeof(DataBinary<T>), "target");
                        var codelamda = Expression.Constant(code);
                        var binlamda = Expression.Parameter(typeof(byte[]).MakeByRefType(), "source");
                        var readint = Expression.Call(method, target, codelamda, binlamda);
                        var propcall = Expression.Property(Expression.Convert(target, typeof(T)), properties.Name);//子クラスにキャストしてPropertyからゲット
                        var assing = Expression.Assign(propcall, readint);//target.prop = readint(code,ref source)
                        var lamda = Expression.Lambda<ReadByBins>(assing, target, binlamda);
                        Exprlist.Add(lamda.Compile());
                    }else
                    if(proptype.IsGenericType  
                       && proptype.GetGenericTypeDefinition() == typeof(List<>) 
                       && IsParent(proptype.GetGenericArguments()[0],typeof(DataBinaryBase))
                       ){
                        var method = proptype.GetGenericArguments()[0].BaseType.GetMethods(BindingFlags.Public | BindingFlags.Static).Where(m
=>
                        {
                            var name = m.Name.Equals(nameof(ReadDataBinaryList));
                            if (name == false) { return false; }
                            var parameters = m.GetParameters();
                            if (parameters.Count() != 3) { return false; }
                            var databins = parameters[0].ParameterType == typeof(DataBinaryBase).MakeByRefType();
                            var codearg = parameters[1].ParameterType == typeof(byte).MakeByRefType();
                            var sourcearg = parameters[2].ParameterType == typeof(byte[]).MakeByRefType();
                            return databins && codearg && sourcearg;
                        }).FirstOrDefault();
                        var target = Expression.Parameter(typeof(DataBinary<T>), "target");
                        var codelamda = Expression.Constant(code);
                        var binlamda = Expression.Parameter(typeof(byte[]).MakeByRefType(), "source");
                        var readint = Expression.Call(method, target, codelamda, binlamda);
                        var propcall = Expression.Property(Expression.Convert(target, typeof(T)), properties.Name);//子クラスにキャストしてPropertyからゲット
                        var assing = Expression.Assign(propcall, readint);//target.prop = readint(code,ref source)
                        var lamda = Expression.Lambda<ReadByBins>(assing, target, binlamda);
                        Exprlist.Add(lamda.Compile());

                    }else
                    if( proptype.IsGenericType
                        && proptype.GetGenericTypeDefinition() == typeof(Dictionary<,>)
                        && IsParent(proptype.GetGenericArguments()[1],typeof(DataBinaryBase))
                        && proptype.GetGenericArguments()[0] == typeof(byte)
                        )
                    {
                        var method = proptype.GetGenericArguments()[1].BaseType.GetMethods(BindingFlags.Public | BindingFlags.Static).Where(m
=>
                        {
                            var name = m.Name.Equals(nameof(ReadDataBinaryByteDic));
                            if (name == false) { return false; }
                            var parameters = m.GetParameters();
                            if (parameters.Count() != 3) { return false; }
                            var databins = parameters[0].ParameterType == typeof(DataBinaryBase).MakeByRefType();
                            var codearg = parameters[1].ParameterType == typeof(byte).MakeByRefType();
                            var sourcearg = parameters[2].ParameterType == typeof(byte[]).MakeByRefType();
                            return databins && codearg && sourcearg;
                        }).FirstOrDefault();
                        var target = Expression.Parameter(typeof(DataBinary<T>), "target");
                        var codelamda = Expression.Constant(code);
                        var binlamda = Expression.Parameter(typeof(byte[]).MakeByRefType(), "source");
                        var readint = Expression.Call(method, target, codelamda, binlamda);
                        var propcall = Expression.Property(Expression.Convert(target, typeof(T)), properties.Name);//子クラスにキャストしてPropertyからゲット
                        var assing = Expression.Assign(propcall, readint);//target.prop = readint(code,ref source)
                        var lamda = Expression.Lambda<ReadByBins>(assing, target, binlamda);
                        Exprlist.Add(lamda.Compile());

                    }else
                    if (proptype.IsGenericType
                        && proptype.GetGenericTypeDefinition() == typeof(Dictionary<,>)
                        && IsParent(proptype.GetGenericArguments()[1], typeof(DataBinaryBase))
                        && proptype.GetGenericArguments()[0] == typeof(int)
                        )
                    {
                        var method = proptype.GetGenericArguments()[1].BaseType.GetMethods(BindingFlags.Public | BindingFlags.Static).Where(m
=>
                        {
                            var name = m.Name.Equals(nameof(ReadDataBinaryIntDic));
                            if (name == false) { return false; }
                            var parameters = m.GetParameters();
                            if (parameters.Count() != 3) { return false; }
                            var databins = parameters[0].ParameterType == typeof(DataBinaryBase).MakeByRefType();
                            var codearg = parameters[1].ParameterType == typeof(byte).MakeByRefType();
                            var sourcearg = parameters[2].ParameterType == typeof(byte[]).MakeByRefType();
                            return databins && codearg && sourcearg;
                        }).FirstOrDefault();
                        var target = Expression.Parameter(typeof(DataBinary<T>), "target");
                        var codelamda = Expression.Constant(code);
                        var binlamda = Expression.Parameter(typeof(byte[]).MakeByRefType(), "source");
                        var readint = Expression.Call(method, target, codelamda, binlamda);
                        var propcall = Expression.Property(Expression.Convert(target, typeof(T)), properties.Name);//子クラスにキャストしてPropertyからゲット
                        var assing = Expression.Assign(propcall, readint);//target.prop = readint(code,ref source)
                        var lamda = Expression.Lambda<ReadByBins>(assing, target, binlamda);
                        Exprlist.Add(lamda.Compile());
                    }
                }
            }
            foreach(var a in Exprlist)
            {
                a(this, ref source);
            }
            InstancePool<Dictionary<byte, int>>.Free(CacheIndex);
            CacheIndex = null;
            return true;
        }
        


    }

    internal class InstancePool<T> where T :new()
    {
        [ThreadStatic]
        public static Queue<T> Pool;
        public static T Rent()
        {
            Pool = Pool == null ? new Queue<T>() : Pool;
            if(Pool.Count == 0)
            {
                return new T();
            }else
            {
                return Pool.Dequeue();
            }
        }
        public static void Free(T t)
        {
            Pool.Enqueue(t);
        }
    }

    internal class InnerByteBufferPool
    {

        private int Index;
        public byte[] Rent()
        {
            return null;
        }
        public void Return(byte[] data)
        {

        }

    }

    internal class InnerBufferPool
    {
        /// <summary>
        /// Codeごとのバッファープール
        /// [code][stack][bufferlength]
        /// サーバー側はGC不可能
        /// </summary>
        [ThreadStatic]
        public static byte[][][] InnerPool;

        /// <summary>
        /// stack[code]
        /// </summary>
        [ThreadStatic]
        public static byte[] stack;

        const int maxstack = 10;
        public static byte[] GetBuffer(byte code)
        {
            stack = stack == null ? new byte[255] : stack;
            if(stack[code] == maxstack)
            {
                throw new Exception("InnverBuffer StackOver MaxRecursive:"+maxstack);
            }
            var currentstack = stack[code];
            stack[code] += 1;
            InnerPool = InnerPool == null ? new byte[255][][]: InnerPool;
            InnerPool[code] = InnerPool[code] == null ? new byte[maxstack][] : InnerPool[code];
            InnerPool[code][currentstack] = InnerPool[code][currentstack] == null ? new byte[65534] : InnerPool[code][currentstack];
            return InnerPool[code][currentstack];//return 後Stackを増やす
        }
        public static void StackReset()
        {
            stack = stack == null ? new byte[255] : stack;
            Array.Clear(stack, 0, stack.Length);
        }
    }

    /// <summary>
    /// https://github.com/neuecc/MessagePack-CSharp/blob/master/src/MessagePack/Internal/ArrayPool.cs
    /// </summary>
    internal sealed class BufferPool : ArrayPool<byte>
    {
        public static readonly BufferPool Default = new BufferPool(65535);

        public BufferPool(int bufferLength)
            : base(bufferLength)
        {
        }
    }
    internal class ArrayPool<T>
    {
        readonly int bufferLength;
        readonly object gate;
        int index;
        T[][] buffers;

        public ArrayPool(int bufferLength)
        {
            this.bufferLength = bufferLength;
            this.buffers = new T[4][];
            this.gate = new object();
        }

        public T[] Rent()
        {
            lock (gate)
            {
                if (index >= buffers.Length)
                {
                    Array.Resize(ref buffers, buffers.Length * 2);
                }

                if (buffers[index] == null)
                {
                    buffers[index] = new T[bufferLength];
                }

                var buffer = buffers[index];
                buffers[index] = null;
                index++;

                return buffer;
            }
        }

        public void Return(T[] array)
        {
            if (array.Length != bufferLength)
            {
                throw new InvalidOperationException("return buffer is not from pool");
            }

            lock (gate)
            {
                if (index != 0)
                {
                    buffers[--index] = array;
                }
            }
        }
    }
}
