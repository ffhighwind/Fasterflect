using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Benchmark
{
	public class TestUri
	{
		public TestUri(Uri uri)
		{
			Host = uri.Host;
		}

		public TestUri(string host)
		{
			Host = host;
		}

		private string host;
		private string Host {
			get { return host; }
			set { host = value; }
		}

		public string PublicHost {
			get { return host; }
			set { host = value; }
		}
	}
}
