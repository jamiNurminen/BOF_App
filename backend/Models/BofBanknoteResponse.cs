using System.ComponentModel;

namespace BOF_app.Models
{
    public sealed class BofBanknoteResponseDto
    {
        public required int CurrentPage { get; init; }
        public required int TotalPages { get; init; }
        public required int PageSize { get; init; }
        public required int TotalCount { get; init; }
        public required List<BofBanknoteItemDto> Items { get; init; }
    }

    public sealed class BofBanknoteItemDto
    {
        public required string Dataset { get; init; }
        public required string Name { get; init; }
        public required List<BofBanknoteObservationDto> Observations { get; init; }
    }

    public sealed class BofBanknoteObservationDto
    {
        public required string Period { get; init; }
        public required long Value { get; init; }
    }
}