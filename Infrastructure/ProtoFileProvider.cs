namespace GrpcSample.Infrastructure {

    public class ProtoFileProvider {

        private readonly string _contentRootPath;
        
        public ProtoFileProvider(IWebHostEnvironment webHostEnvironment) {

            _contentRootPath = webHostEnvironment.ContentRootPath;
        }

        public Dictionary<string, IEnumerable<string>> GetAllProtoFiles() {

            string protoPath = Path.Combine(_contentRootPath, "Protos");

            if (!Directory.Exists(protoPath)) {

                return new Dictionary<string, IEnumerable<string>>();
            }

            return Directory.GetDirectories(protoPath)
                .Select(dir => new
                {
                    version = dir,
                    protos = Directory.Exists(dir) 
                        ? Directory.GetFiles(dir).Select(Path.GetFileName).Where(f => f != null).Cast<string>() 
                        : Enumerable.Empty<string>()
                })
                .ToDictionary(
                    c => Path.GetRelativePath(protoPath, c.version) ?? string.Empty,
                    c => c.protos
            );
        }

        public async Task<string> GetContent(int version, string protoName) {

            var filePath = $"{_contentRootPath}/Protos/v{version}/{protoName}";

            var exists = File.Exists(filePath);

            return exists ? await File.ReadAllTextAsync(filePath) : string.Empty;
        }

        public string GetPath(int version, string protoName) {

            var filePath = $"{_contentRootPath}/Protos/v{version}/{protoName}";

            var exists = File.Exists(filePath);

            return exists ? filePath : string.Empty;
        }
    }
}