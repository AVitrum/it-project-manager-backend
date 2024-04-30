namespace DatabaseService;
public class EntityNotFoundException(string entityType) : DatabaseException($"{entityType} not found");