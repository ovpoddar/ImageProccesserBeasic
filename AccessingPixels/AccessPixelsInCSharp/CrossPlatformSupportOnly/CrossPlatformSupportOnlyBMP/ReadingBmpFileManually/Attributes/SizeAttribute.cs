using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Attributes.Arrey;
[AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
internal sealed class SizeAttribute : SizeInternal
{
    public SizeAttribute(int length, int size) : base(size)
    {
		this.Length = length;
	}

    public int Length { get; }
}
