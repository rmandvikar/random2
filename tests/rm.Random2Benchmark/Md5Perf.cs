using System.Security.Cryptography;
using System.Text;
using BenchmarkDotNet.Attributes;

public class Md5Perf
{
	private readonly byte[] bytes = Encoding.UTF8.GetBytes("the overtinkerer");

	private readonly MD5 md5 = MD5.Create();

	[ThreadStatic]
	private static MD5 md5_ts;

	public Md5Perf()
	{

	}

	[Benchmark]
	public byte[] SharedInstance()
	{
		var hash = md5.ComputeHash(bytes);
		//Console.WriteLine(hash);
		return hash;
	}

	[Benchmark]
	public byte[] NewInstance()
	{
		using var md5 = MD5.Create();
		{
			var hash = md5.ComputeHash(bytes);
			//Console.WriteLine(hash);
			return hash;
		}
	}

	[Benchmark]
	public byte[] HashData()
	{
		var hash = MD5.HashData(bytes);
		//Console.WriteLine(hash);
		return hash;
	}

	[Benchmark]
	public byte[] ThreadStatic()
	{
		var md5Local = md5_ts ?? MD5.Create();
		var hash = md5Local.ComputeHash(bytes);
		//Console.WriteLine(hash);
		return hash;
	}
}
