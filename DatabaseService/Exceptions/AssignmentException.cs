namespace DatabaseService.Exceptions;

public class AssignmentException(string message) : Exception(message);

public class AssignmentDeadlineException() : AssignmentException("The deadline has passed");
