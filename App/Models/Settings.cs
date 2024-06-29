using Newtonsoft.Json;

namespace App.Models {
	[JsonObject(MemberSerialization.OptOut)]
	public class Settings {
		public uint Threads { get; set; } = 5;
		public uint ConnectionTimeout { get; set; } = 5000;
		public uint LoadingDelay { get; set; } = 1000;
		public uint CertificateWarningDelay { get; set; } = 1000;
		public uint StickyKeysWarningDelay { get; set; } = 1000;
	}
}