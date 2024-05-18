using System.IO;

namespace DeserializeClasses {
    class Settings {

        public bool? WriteLogs { get; set; }
        public string? CurrentDb { get; set; }
        public string? ConnectionString { get; set; }
    }
}
