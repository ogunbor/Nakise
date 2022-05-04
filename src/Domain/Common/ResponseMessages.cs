namespace Domain.Common
{
    public class ResponseMessages
    {
        public const string DuplicateEmail = "Email already exist";
        public const string RetrievalSuccessResponse = "Data retrieved successfully";
        public const string CreationSuccessResponse = "Data created successfully";
        public const string LoginSuccessResponse = "Login successful";
        public const string WrongEmailOrPassword = "Wrong email or password provided";
        public const string UserNotFound = "User not found";
        public const string InvalidToken = "Invalid token";
        public const string MissingClaim = "MissingClaim:";
        public static readonly string DataRetrievedSuccessfully = "Data retrieved successfully";
        public static readonly string UnAuthorized;
    }
}
