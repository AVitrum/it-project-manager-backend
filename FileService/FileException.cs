namespace FileService;

public class FileException(string message) : Exception(message) { }

public class FileToLargeException(string message) : FileException(message);

public class FileInvalidFormatException(string message) : FileException(message);