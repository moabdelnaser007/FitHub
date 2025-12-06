namespace FitHubBackendAPI.Entities.Enums
{
    public enum ErrorCode
    {
        NoError = 0,
        // range 100-199 for User related errors
        InvalidUserID = 100,
        UserNotFound = 101,
        // range 200-299 for GYM related errors
        GYMNotFound = 201,

    }
}
