using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using LocresLib;
using System.IO;
using Newtonsoft.Json.Linq;

namespace PD3LocresTool
{
	internal class Program
	{
		internal class KeyValuePair
		{
			[JsonProperty("Key")]
			public string Key { get; set; }
			[JsonProperty("Value")]
			public string Value { get; set; }
		}
		static string source = @"D:\software\FModel\Output\Exports\PAYDAY3\Content\Localization\Game\en\Game.locres";
		static void savetojson()
		{
			var locres = new LocresFile();

			using (var file = File.OpenRead(source))
			{
				locres.Load(file);
			}
			Dictionary<string, List<KeyValuePair>> final = new Dictionary<string, List<KeyValuePair>>();

			foreach (var anamespace in locres)
			{
				List<KeyValuePair> list = new List<KeyValuePair>();
				foreach (var keypair in anamespace)
				{
					list.Add(new KeyValuePair() { Key = keypair.Key, Value = keypair.Value });
				}
				final.Add(anamespace.Name, list);
			}
			File.WriteAllText("./locres.json", JsonConvert.SerializeObject(final));
		}
		static void writefromjson()
		{
			var output = new LocresFile();
			using (var file = File.OpenRead(source))
			{
				output.Load(file);
			}
			Dictionary<string, List<KeyValuePair>> input = JsonConvert.DeserializeObject<Dictionary<string, List<KeyValuePair>>>(File.ReadAllText("./locres.json"));

			foreach(var anamespace in output)
			{
				foreach (var keypair in anamespace)
				{
					if (input.ContainsKey(anamespace.Name))
					{
						var replacement = input[anamespace.Name].Find(p => p.Key == keypair.Key);
						if (replacement != null)
						{
							var indx1 = output.IndexOf(anamespace);
							var indx2 = output[indx1].FindIndex(p => p.Key == keypair.Key);
							output[indx1][indx2].Value = replacement.Value;
						}
					}
				}
			}

			//var newoutput = (LocresFile)namespaces;
			output.Save(File.OpenWrite("newlocres.locres"));
		}
		static void Main(string[] args)
		{
			if (args.Length == 0) {
				Console.WriteLine("Usage: PD3LocresTool -tojson/-tolocres");
			}
			switch(args[0])
			{
				case "-tojson":
					savetojson();
					break;
				case "-tolocres":
					writefromjson();
					break;
			}
		}
	}
}
