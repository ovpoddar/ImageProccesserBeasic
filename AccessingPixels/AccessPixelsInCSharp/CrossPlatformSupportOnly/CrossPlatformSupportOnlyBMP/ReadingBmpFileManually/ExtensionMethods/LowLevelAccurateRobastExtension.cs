using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

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

	public long GetObjectSize<T>(in T structObject) where T : struct
	{
		var result = 0L;
		var properties = GetProperties(typeof(T));
		if (!properties.Any(a => a.PropertyType.IsArray))
		{
			result = Marshal.SizeOf(structObject);
		}
		else
		{
			foreach (var property in properties)
			{
				if (property.PropertyType.IsArray)
				{
					var array = property.GetValue(structObject) as Array;
					if (array != null)
					{
						result += GetArraySize(array, property.PropertyType.GetElementType()!);
					}
				}
				else
				{
					result += GetPropertySize(property.PropertyType, property.GetValue(structObject));
				}
			}
		}
		return result;
	}
	
	public byte[] GetBytes<T>(T value) where T : struct
	{
		var objectSize = GetObjectSize(value);
		Span<byte> result = stackalloc byte[(int)objectSize];
		var infos = GetProperties(value.GetType());
		if (infos.Any(a => a.PropertyType.IsArray))
		{
			Unsafe.As<byte, T>(ref result[0]) = value;
		}
		else
		{
			var index = 0;
			foreach (var prop in infos)
			{
				var propValue = prop.GetValue(value);
				if (prop.PropertyType.IsArray)
				{
					var listType = prop.PropertyType.GetElementType();
					foreach (var item in (Array)propValue)
					{
						WriteProperty(ref result, ref index, listType, item);
					}
				}
				else
				{
					WriteProperty(ref result, ref index, prop.PropertyType, propValue);
				}
			}
		}
		return result.ToArray();
	}
	
	static LowLevelAccurateRobastExtension()
	{
		_propertyCache = new();
		_instance = Instance;
	}

	static PropertyInfo[] GetProperties(Type type)
	{
		ref var valOrNull = ref CollectionsMarshal.GetValueRefOrAddDefault(_propertyCache, type, out var isExist);
		if (!isExist)
			valOrNull = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
		return valOrNull!;
	}
	
	static long GetArraySize(Array array, Type elementType)
	{
		var result = 0l;
		foreach (var proprity in array)
			result += GetPropertySize(elementType, proprity);

		return result;
	}
	
	static long GetPropertySize(Type type, object value) =>
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

	static void WriteProperty<T>(ref Span<byte> bytes, ref int index, Type type, T value)
	{
		switch (Type.GetTypeCode(type))
		{
			case TypeCode.Boolean:
				Unsafe.WriteUnaligned(ref bytes[index], (byte)(((bool)(object)value) ? 1 : 0));
				index += sizeof(byte);
				break;
			case TypeCode.Char:
				Unsafe.WriteUnaligned(ref bytes[index], (char)(object)value);
				index += sizeof(char);
				break;
			case TypeCode.Int16:
				Unsafe.WriteUnaligned(ref bytes[index], (short)(object)value);
				index += sizeof(short);
				break;
			case TypeCode.UInt16:
				Unsafe.WriteUnaligned(ref bytes[index], (ushort)(object)value);
				index += sizeof(ushort);
				break;
			case TypeCode.Int32:
				Unsafe.WriteUnaligned(ref bytes[index], (int)(object)value);
				index += sizeof(int);
				break;
			case TypeCode.UInt32:
				Unsafe.WriteUnaligned(ref bytes[index], (uint)(object)value);
				index += sizeof(uint);
				break;
			case TypeCode.Int64:
				Unsafe.WriteUnaligned(ref bytes[index], (long)(object)value);
				index += sizeof(long);
				break;
			case TypeCode.UInt64:
				Unsafe.WriteUnaligned(ref bytes[index], (ulong)(object)value);
				index += sizeof(ulong);
				break;
			case TypeCode.Single:
				Unsafe.WriteUnaligned(ref bytes[index], (float)(object)value);
				index += sizeof(float);
				break;
			case TypeCode.Double:
				Unsafe.WriteUnaligned(ref bytes[index], (double)(object)value);
				index += sizeof(double);
				break;
			case TypeCode.Decimal:
				Unsafe.WriteUnaligned(ref bytes[index], (double)(object)value);
				index += sizeof(double);
				break;
			case TypeCode.String:
				var valueAsBytes = Encoding.ASCII.GetBytes((string)(object)value).AsSpan();
				valueAsBytes.CopyTo(bytes.Slice(index, valueAsBytes.Length));
				index += valueAsBytes.Length;
				break;
			case TypeCode.Byte:
				bytes[index] = (byte)(object)value;
				index++;
				break;
			case TypeCode.SByte:
				bytes[index] = (byte)(object)value;
				index++;
				break;
			default:
				var size = Marshal.SizeOf(type);
				index += size;
				break;
		}
	}
}
