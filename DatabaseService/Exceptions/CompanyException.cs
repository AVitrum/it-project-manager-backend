namespace DatabaseService.Exceptions;

public class CompanyException(string message) : Exception(message) { }

public class PermissionException(string message) : CompanyException(message) { }
