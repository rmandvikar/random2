using System.Security.Cryptography;
using System.Text;
using BenchmarkDotNet.Attributes;

public class Md5Perf
{
	private readonly byte[] bytes = Encoding.UTF8.GetBytes("the overtinkerer");

	public Md5Perf()
	{ }

	private readonly MD5 md5 = MD5.Create();
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

	[ThreadStatic]
	private static MD5 md5_ts;
	[Benchmark]
	public byte[] ThreadStatic()
	{
		var md5Local = md5_ts;
		if (md5_ts == null)
		{
			md5Local = md5_ts = MD5.Create();
		}
		var hash = md5Local.ComputeHash(bytes);
		//Console.WriteLine(hash);
		return hash;
	}

	private readonly MD5 md5_lock = MD5.Create();
	private readonly object locker = new();
	[Benchmark]
	public byte[] SharedInstance_Lock()
	{
		lock (locker)
		{
			var hash = md5_lock.ComputeHash(bytes);
			//Console.WriteLine(hash);
			return hash;
		}
	}
}
