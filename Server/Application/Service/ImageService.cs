using Application.Interface;
using Domain.Enum;
using Domain.ValueObject;

namespace Application.Service
{
    public class ImageService : IImageService 
    {
        #region Attributes
        private readonly IImageStorage storage;
        #endregion

        #region Properties
        #endregion

        public ImageService(IImageStorage storage)
        {
            this.storage = storage;
        }

        #region Methods
        public async Task<ImagePath> UploadAsync(
            Stream stream,
            string contentType,
            CancellationToken ct)
        {
            if (!contentType.StartsWith("image/"))
                throw new InvalidOperationException("Invalid image type");

            return await storage.SaveAsync(stream, contentType, ct);
        }
        #endregion
    }
}
