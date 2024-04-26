namespace Server.Exceptions;
public class EntityNotFoundException(string entityType) : Exception($"{entityType} not found");