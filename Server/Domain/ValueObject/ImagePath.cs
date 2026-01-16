namespace Domain.ValueObject
{
    public record ImagePath(string Value)
    {
        public override string ToString() => Value;
    }
}
