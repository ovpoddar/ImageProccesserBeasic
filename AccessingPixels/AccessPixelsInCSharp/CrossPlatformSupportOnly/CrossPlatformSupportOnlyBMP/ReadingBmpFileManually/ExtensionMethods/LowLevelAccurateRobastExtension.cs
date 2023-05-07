using Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ReadingBmpFileManually.ExtensionMethods;
public sealed class LowLevelAccurateRobastExtension
{
	private static LowLevelAccurateRobastExtension? _instance = null;
	private static readonly object _lockObj = new object();
	private static readonly Dictionary<Type, PropertyInfo[]> _propertyCache;

	public static LowLevelAccurateRobastExtension Instance
	{
		get
		{
			if (_instance == null)
			{
				lock (_lockObj)
				{
					if (_instance == null)
					{
						_instance = new LowLevelAccurateRobastExtension();
					}
				}
			}
			return _instance;
		}
	}


	static LowLevelAccurateRobastExtension()
	{
		_propertyCache = new();
		_instance = Instance;
	}

	public long GetObjectSize<T>(T value) where T : struct
	{
		var result = 0l;
		var proprityes = GetProperties(value.GetType());
		foreach (var proprity in proprityes)
		{
			var attribute = Attribute.GetCustomAttributes(proprity).OfType<SizeInternal>()
				.FirstOrDefault();
			if (proprity.PropertyType.IsArray)
			{
				var array = proprity.GetValue(value) as Array;
				if (attribute == null)
				{
					result += GetArraySize(array, proprity.PropertyType.GetElementType()!);
				}
				else
				{
					var attributeArrey = attribute as Attributes.Arrey.SizeAttribute;

					if (attributeArrey != null)
					{
						if (array != null && array.Length != attributeArrey.Length)
							throw new Exception("Size OverFlow Exception");
						result += attributeArrey.Size * attributeArrey.Length;
					}
					else
					{
						result += array.Length * attribute.Size;
					}
				}
			}
			else
			{
				if (attribute == null)
					result = Marshal.SizeOf(proprity.PropertyType);
				else
				{
					var requiredSize = GetPropertySize(proprity.PropertyType, proprity.GetValue(value));
					if (requiredSize > attribute.Size)
						throw new Exception("Length DataOverFlow Exception");
					result = attribute.Size;
				}
			}
		}
		return result;
	}
	public T GetObjects<T>(in byte[] value, bool? isLittleEndian = null) where T : struct
	{
		var result = Activator.CreateInstance<T>();
		var proprityes = GetProperties(result.GetType());

		if (!proprityes.Any(a => a.PropertyType.IsArray || a.CustomAttributes.Any()))
		{
			return Unsafe.As<byte, T>(ref value[0]);
		}
		else
		{

			object boxedResult = result;
			var index = 0;
			foreach (var proprity in proprityes)
			{
				if (proprity.PropertyType.IsArray)
				{
					var arreySize = GetArraySize(proprity);
					var arrayType = proprity.PropertyType.GetElementType();
					var arraysegment = new ArraySegment<byte>(value, index + (index == 0 ? 0 : 1), (int)(arreySize.size * arreySize.length));
					var arrayValue = Array.CreateInstance(arrayType, arreySize.size);
					for (var i = 0; i < arreySize.size; i++)
					{
						var propritySegment = arraysegment.Slice((int)(i * arreySize.length), (int)arreySize.length).ToArray();
						var proprityValue = ReadPropertyValue(arrayType, ref propritySegment, ref index, isLittleEndian);
						arrayValue.SetValue(proprityValue, i);
					}
					proprity.SetValue(boxedResult, arrayValue);
				}
				else
				{
					var itemSize = GetPropertySize(proprity);
					var arraysegment = new ArraySegment<byte>(value, index + (index == 0 ? 0 : 1), (int)itemSize).ToArray();
					var proprityValue = ReadPropertyValue(proprity.PropertyType, ref arraysegment, ref index, isLittleEndian);
					proprity.SetValue(boxedResult, proprityValue);
				}
			}
			return (T)boxedResult;
		}
	}
	public byte[] GetBytes<T>(in T value, bool? isLittleEndian = null) where T : struct
	{
		var objectSize = GetObjectSize(value);
		var result = new byte[objectSize];
		var proprityes = GetProperties(value.GetType());
		if (!proprityes.Any(a => a.PropertyType.IsArray || a.CustomAttributes.Any()))
		{
			Unsafe.As<byte, T>(ref result[0]) = value;
		}
		else
		{
			var index = 0;
			foreach (var proprity in proprityes)
			{
				var proprityValue = proprity.GetValue(value);

				if (proprity.PropertyType.IsArray)
				{
					var arraySize = GetArraySize(proprity);
					var objectArrey = proprityValue as Array;
					if (arraySize.size != objectArrey.Length)
						throw new OverflowException();
					foreach (var obj in objectArrey)
					{
						var arrseg = new ArraySegment<byte>(result, index + (index == 0 ? 0 : 1), (int)arraySize.length).AsSpan();
						WritePropertyValue(ref arrseg, ref index, proprity.PropertyType.GetElementType(), obj, isLittleEndian);
					}

				}
				else
				{
					var propritySize = GetPropertySize(proprity);
					var arrSeg = new ArraySegment<byte>(result, index + (index == 0 ? 0 : 1), (int)propritySize).AsSpan();
					WritePropertyValue(ref arrSeg, ref index, proprity.PropertyType, proprityValue, isLittleEndian);
				}
			}
		}

		return result;
	}


	PropertyInfo[] GetProperties(Type type)
	{
		ref var valOrNull = ref CollectionsMarshal.GetValueRefOrAddDefault(_propertyCache, type, out var isExist);
		if (!isExist)
			valOrNull = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
		return valOrNull!;
	}
	long GetArraySize(Array array, Type elementType)
	{
		var result = 0l;
		foreach (var proprity in array)
			result += GetPropertySize(elementType, proprity);

		return result;
	}
	(long size, long length) GetArraySize(PropertyInfo propertyInfo)
	{
		var attribute = Attribute.GetCustomAttributes(propertyInfo).OfType<SizeInternal>()
				.FirstOrDefault();
		if (attribute == null)
			throw new Exception("Dynamic size could not determine");
		var arreyattributr = attribute as Attributes.Arrey.SizeAttribute;
		if (arreyattributr != null)
			return (arreyattributr.Length, arreyattributr.Size);
		else
		{
			var propritySize = GetPropertySize(propertyInfo);
			return (propritySize, attribute.Size);
		}
	}
	long GetPropertySize(Type type, object value) =>
	   Type.GetTypeCode(type) switch
	   {
		   TypeCode.Boolean => sizeof(bool),
		   TypeCode.Char => sizeof(char),
		   TypeCode.Int16 => sizeof(short),
		   TypeCode.UInt16 => sizeof(ushort),
		   TypeCode.Int32 => sizeof(int),
		   TypeCode.UInt32 => sizeof(uint),
		   TypeCode.Int64 => sizeof(long),
		   TypeCode.UInt64 => sizeof(ulong),
		   TypeCode.Single => sizeof(float),
		   TypeCode.Double => sizeof(double),
		   TypeCode.Decimal => sizeof(decimal),
		   TypeCode.Byte => sizeof(byte),
		   TypeCode.SByte => sizeof(sbyte),
		   TypeCode.String => ((string)value).Length,
		   _ => (long)IntPtr.Size,
	   };
	long GetPropertySize(PropertyInfo proprity)
	{
		var attribute = Attribute.GetCustomAttributes(proprity).OfType<SizeInternal>()
				.FirstOrDefault();
		return attribute != null
			? attribute.Size
			: Marshal.SizeOf(proprity.PropertyType);
	}
	object ReadPropertyValue(Type proprityType, ref byte[] values, ref int index, bool? isLittleEndian = null)
	{
		if ((isLittleEndian == null && BitConverter.IsLittleEndian) || isLittleEndian == true)
			Array.Reverse(values);

		var result = new object();
		switch (Type.GetTypeCode(proprityType))
		{
			case TypeCode.Boolean:
				result = Unsafe.ReadUnaligned<bool>(ref values[0]);
				break;
			case TypeCode.Char:
				result = Unsafe.ReadUnaligned<char>(ref values[0]);
				break;
			case TypeCode.Int16:
				result = Unsafe.ReadUnaligned<short>(ref values[0]);
				break;
			case TypeCode.UInt16:
				result = Unsafe.ReadUnaligned<ushort>(ref values[0]);
				break;
			case TypeCode.Int32:
				result = Unsafe.ReadUnaligned<int>(ref values[0]);
				break;
			case TypeCode.UInt32:
				result = Unsafe.ReadUnaligned<uint>(ref values[0]);
				break;
			case TypeCode.Int64:
				result = Unsafe.ReadUnaligned<long>(ref values[0]);
				break;
			case TypeCode.UInt64:
				result = Unsafe.ReadUnaligned<ulong>(ref values[0]);
				break;
			case TypeCode.Single:
				result = Unsafe.ReadUnaligned<float>(ref values[0]);
				break;
			case TypeCode.Double:
				result = Unsafe.ReadUnaligned<double>(ref values[0]);
				break;
			case TypeCode.Decimal:
				result = Unsafe.ReadUnaligned<double>(ref values[0]);
				break;
			case TypeCode.String:
				result = Encoding.ASCII.GetString(values);
				break;
			case TypeCode.Byte:
				result = Unsafe.ReadUnaligned<byte>(ref values[0]);
				break;
			case TypeCode.SByte:
				result = Unsafe.ReadUnaligned<sbyte>(ref values[0]);
				break;
			default:
				break;
		}

		index += values.Length - (index == 0 ? 1 : 0);
		return result;
	}
	void WritePropertyValue<T>(ref Span<byte> bytes, ref int index, Type type, T value, bool? isLittleEndian = null)
	{
		switch (Type.GetTypeCode(type))
		{
			case TypeCode.Boolean:
				Unsafe.WriteUnaligned(ref bytes[0], (byte)(((bool)(object)value) ? 1 : 0));
				break;
			case TypeCode.Char:
				Unsafe.WriteUnaligned(ref bytes[0], (char)(object)value);
				break;
			case TypeCode.Int16:
				Unsafe.WriteUnaligned(ref bytes[0], (short)(object)value);
				break;
			case TypeCode.UInt16:
				Unsafe.WriteUnaligned(ref bytes[0], (ushort)(object)value);
				break;
			case TypeCode.Int32:
				Unsafe.WriteUnaligned(ref bytes[0], (int)(object)value);
				break;
			case TypeCode.UInt32:
				Unsafe.WriteUnaligned(ref bytes[0], (uint)(object)value);
				break;
			case TypeCode.Int64:
				Unsafe.WriteUnaligned(ref bytes[0], (long)(object)value);
				break;
			case TypeCode.UInt64:
				Unsafe.WriteUnaligned(ref bytes[0], (ulong)(object)value);
				break;
			case TypeCode.Single:
				Unsafe.WriteUnaligned(ref bytes[0], (float)(object)value);
				break;
			case TypeCode.Double:
				Unsafe.WriteUnaligned(ref bytes[0], (double)(object)value);
				break;
			case TypeCode.Decimal:
				Unsafe.WriteUnaligned(ref bytes[0], (double)(object)value);
				break;
			case TypeCode.String:
				var valueAsBytes = Encoding.ASCII.GetBytes((string)(object)value).AsSpan();
				valueAsBytes.CopyTo(bytes.Slice(0, valueAsBytes.Length));
				break;
			case TypeCode.Byte:
				Unsafe.WriteUnaligned(ref bytes[0], (byte)(object)value);
				break;
			case TypeCode.SByte:
				Unsafe.WriteUnaligned(ref bytes[0], (sbyte)(object)value);
				break;
			default:
				var size = Marshal.SizeOf(type);
				break;
		}

		if ((isLittleEndian == null && BitConverter.IsLittleEndian) || isLittleEndian == true)
			MemoryExtensions.Reverse(bytes);
		index += bytes.Length - (index == 0 ? 1 : 0);
	}
}
