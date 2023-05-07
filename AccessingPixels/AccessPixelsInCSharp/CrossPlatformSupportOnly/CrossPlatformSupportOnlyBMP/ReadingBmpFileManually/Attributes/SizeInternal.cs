using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Attributes;
internal class SizeInternal : Attribute
{
	public SizeInternal(int Size) => 
		this.Size = Size;

	public int Size { get; }
}
