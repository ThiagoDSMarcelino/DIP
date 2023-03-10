public class InvalidParameterSizeException : System.Exception
{
    public InvalidParameterSizeException() { }
    public InvalidParameterSizeException(string message) : base(message) { }
    public InvalidParameterSizeException(string message, System.Exception inner) : base(message, inner) { }
    protected InvalidParameterSizeException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

    public override string Message
        => "The kernel must be of quadratic size and have a center";
}