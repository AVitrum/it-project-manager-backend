namespace UserHelper;

public class UserException(string message) : Exception(message);

public class NotUniqueEmailException(string email) : UserException($"Email: {email} is not unique.");

public class UserNotFoundException : UserException
{
    public UserNotFoundException(long id) : base($"User not found by id: {id}!") { }
    public UserNotFoundException(string email) : base($"User not found by email: {email}!") { }
}