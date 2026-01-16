using Application.Interface;
using Domain.Enum;
using Domain.ValueObject;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.ExternalService.Storage
{
    public class LocalImageStorage : IImageStorage
    {
        #region Attributes
        private readonly string root;
        #endregion

        #region Properties
        #endregion

        public LocalImageStorage(IConfiguration config)
        {
            root = config["Storage:RootPath"]!;
        }

        #region Methods
        public async Task<ImagePath> SaveAsync(
            Stream file,
            string contentType,
            CancellationToken ct)
        {
            var now = DateTime.UtcNow;
            var path = Path.Combine(
                now.Year.ToString(),
                now.Month.ToString("D2"),
                $"{Guid.NewGuid()}.webp");

            var fullPath = Path.Combine(root, path);
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);

            using var output = File.Create(fullPath);
            await file.CopyToAsync(output, ct);

            return new ImagePath(path.Replace("\\", "/"));
        }
        #endregion
    }
}
