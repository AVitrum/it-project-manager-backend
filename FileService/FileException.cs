namespace FileService;

public class FileException(string message) : Exception(message) { }

public class FileToLargeException() : FileException("The file size must be less than 5 MB!");

public class FileInvalidFormatException(string message) : FileException(message);