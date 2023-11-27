using UnityEngine;

public class InputSystem : MonoBehaviour
{
    public string buttonsRequestUri = "/buttons";
    public float minimumRequestTime = 0f;

    private WebRequest buttonsRequest;

    private void Awake()
    {
        buttonsRequest = new WebRequest();
    }

    private void Update()
    {
        if (buttonsRequest.Status == WebRequest.RequestStatus.Created || Time.time > buttonsRequest.StartTime + minimumRequestTime)
            GetButtons();
    }

    private void GetButtons()
    {
        if (buttonsRequest.Status != WebRequest.RequestStatus.Running)
        {
            if (buttonsRequest.Status == WebRequest.RequestStatus.Success)
                Debug.Log("Got Buttons in " + buttonsRequest.Duration + "s");
            buttonsRequest.requestUri = buttonsRequestUri;
            buttonsRequest.type = WebRequest.RequestType.GET;
            if (WebClient.current != null) WebClient.current.SendRequest(buttonsRequest);
        }
    }
}
