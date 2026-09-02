namespace ForgeFlow.Api.Services;

public class BucketAlreadyExistsException(string message) : Exception(message);

public class BucketNotFoundException(string message) : Exception(message);
