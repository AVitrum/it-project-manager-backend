namespace DatabaseService.Exceptions;
public class EntityNotFoundException(string entityType) : DatabaseException($"{entityType} not found");