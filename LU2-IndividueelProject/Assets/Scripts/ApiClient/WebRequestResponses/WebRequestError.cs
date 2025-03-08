using UnityEngine;

public class WebRequestError: IWebRequestReponse
{
    public string ErrorMessage;
    public int ErrorCode;

    public WebRequestError(string errorMessage, int errorCode = 0)
    {
        ErrorMessage = errorMessage;
        ErrorCode = errorCode;
    }
}
