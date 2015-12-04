# LCPiratesOnline
A simpel pvp online pirate game made using unity and .NET.
This is the full game including the client software.
The server software will soon be available from another repository.

## Network Package Strucutre
I made easy to use but high speed package system.
Every package can be parsed from bytes or be parsed to bytes.
A referenced index variable is passed to the package, the
package will increase this index by the bytes it has read.
This system makes is possible to append multiple packages
in every data package being sent.

### Vector3 Package example
```C#
public class Vector3Data : PackageData
{
	#region Vars

	public Vector3 Vector;

	#endregion

	#region Construct

	public Vector3Data()
	{
		Vector = Vector3.zero;
	}

	public Vector3Data(Vector3 vector)
	{
		Vector = vector;
	}

	public Vector3Data(byte[] data, ref int offset)
	{
		FromBytes(data, ref offset);
	}

	#endregion

	#region Methods

	public byte[] ToBytes()
	{
		List<byte> total = new List<byte>();
		total.AddRange(BitConverter.GetBytes(Vector.x));
		total.AddRange(BitConverter.GetBytes(Vector.y));
		total.AddRange(BitConverter.GetBytes(Vector.z));
		return total.ToArray();
	}

	public void FromBytes(byte[] data, ref int offset)
	{
		Vector = Vector3.zero;

		Vector.x = BitConverter.ToSingle(data, offset);
		offset += 4;
		
		Vector.y = BitConverter.ToSingle(data, offset);
		offset += 4;

		Vector.z = BitConverter.ToSingle(data, offset);
		offset += 4;
	}

	#endregion
}
```
