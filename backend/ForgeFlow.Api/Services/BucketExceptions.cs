namespace ForgeFlow.Api.Services;

public class BucketAlreadyExistsException(string message) : Exception(message);

public class BucketNotFoundException(string message) : Exception(message);

// Autodesk refused because the bucket belongs to a different application.
public class BucketAccessDeniedException(string message) : Exception(message);
