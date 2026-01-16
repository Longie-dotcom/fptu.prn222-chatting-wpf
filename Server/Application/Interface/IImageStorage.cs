using Domain.Enum;
using Domain.ValueObject;

namespace Application.Interface
{
    public interface IImageStorage
    {
        Task<ImagePath> SaveAsync(
            Stream file,
            string contentType,
            CancellationToken ct);
    }
}
