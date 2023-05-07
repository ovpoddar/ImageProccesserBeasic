using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Attributes.Object;
[AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
internal sealed class SizeAttribute : SizeInternal
{
    public SizeAttribute(int Size) : base(Size)
    {
    }
}

