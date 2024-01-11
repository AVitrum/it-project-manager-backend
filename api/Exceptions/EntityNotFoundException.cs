namespace api.Exceptions
{
    public class EntityNotFoundException : Exception
    {
        public string EntityType { get; }

        public EntityNotFoundException(string entityType) : base($"{entityType} not found")
        {
            EntityType = entityType;
        }

        public EntityNotFoundException(string entityType, string message) : base(message)
        {
            EntityType = entityType;
        }

        public EntityNotFoundException(string entityType, string message, Exception innerException) : base(message, innerException)
        {
            EntityType = entityType;
        }
    }
}