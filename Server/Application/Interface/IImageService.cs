using Domain.Enum;
using Domain.ValueObject;

namespace Application.Interface
{
    public interface IImageService
    {
        Task<ImagePath> UploadAsync(
            Stream stream,
            string contentType,
            CancellationToken ct);
    }
}
