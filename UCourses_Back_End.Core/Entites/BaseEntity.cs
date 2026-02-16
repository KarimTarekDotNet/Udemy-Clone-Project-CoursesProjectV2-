namespace UCourses_Back_End.Core.Entites
{
    public abstract class BaseEntity
    {
        public Guid Id { get; set; }
        public string PublicId { get; set; } = null!;
        public DateOnly CreatedAt { get; set; }

        public string GeneratePublicId(string user)
        {
            return $"{user}-{Guid.CreateVersion7().ToString()[..8]}";
        }
    }
}