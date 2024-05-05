namespace DatabaseService.Exceptions;

public class CompanyException(string message) : Exception(message) { }

public class PermissionException() : CompanyException("You do not have permission to perform this action") { }
