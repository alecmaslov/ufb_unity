using System.Collections;
using System.Collections.Generic;
using UFB.Core;
using UFB.Events;
using UFB.Network;
using UFB.Network.RoomMessageTypes;
using UFB.UI;
using UnityEngine;
using UnityEngine.UI;

public class MainScene : MonoBehaviour
{
    public static MainScene instance;
    
    public InputField loginEmailField;
    public InputField passwordField;
    
    public InputField signUpEmailField;
    public InputField signUpPasswordField;
    public InputField signUpConfirmField;
    
    // Start is called before the first frame update
    void Start()
    {
        // PlayerPrefs.SetString("token", "");
        instance = this;
        string email = PlayerPrefs.GetString("email");
        string password = PlayerPrefs.GetString("password");
        
        loginEmailField.text = email;
        passwordField.text = password;
        
        if (email != "" && password != "")
        {
            Debug.Log("Please fill all the fields");
            SignIn();
        }
    }

    public async void SignUp()
    {
        string email = signUpEmailField.text;
        string password = signUpPasswordField.text;
        string confirm = signUpConfirmField.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirm))
        {
            ShowNotificationMessage("error", "Please fill all fields");
            return;
        }

        if (password != confirm)
        {
            ShowNotificationMessage("error", "Passwords do not match");
            return;
        }
        
        UfbApiClient.RegisterUserResponse response = await ServiceLocator.Current.Get<NetworkService>().SignUpHandler(email, password);
        string result = response.error;
        Debug.Log(result);

        if (result == GlobalDefine.RESPONSE_MESSAGE.ALREADY_EXIST)
        {
            ShowNotificationMessage("error", "User already exists.");
        } 
        else if (result == GlobalDefine.RESPONSE_MESSAGE.SUCCESS)
        {
            ShowNotificationMessage("error", "User registration success.");
        }
    }

    public async void SignIn()
    {
        string email = loginEmailField.text;
        string password = passwordField.text;
        
        UfbApiClient.RegisterUserResponse data = await ServiceLocator.Current.Get<NetworkService>().LoginHandler(email, password);
        string result = data.error;
        Debug.Log(result);

        if (result == GlobalDefine.RESPONSE_MESSAGE.NOT_EXIST)
        {
            ShowNotificationMessage("error", "User does not exist.");
        }
        else if (result == GlobalDefine.RESPONSE_MESSAGE.WRONG_PASSWORD)
        {
            ShowNotificationMessage("error", "Passwords do not match.");
        }
        else if (result == GlobalDefine.RESPONSE_MESSAGE.ERROR)
        {
            
            ShowNotificationMessage("error", "An error occured.");
        }
        else if (result == GlobalDefine.RESPONSE_MESSAGE.SUCCESS)
        {
            ShowNotificationMessage("error", "User successfully signed in.");
            PlayerPrefs.SetString("email", email);
            PlayerPrefs.SetString("password", password);
            Debug.Log(data.clientId);
            ConnectServer(data.clientId);
        }
        else
        {
            ShowNotificationMessage("error", "Server error occured.");
        }
    }

    public async void ConnectServer(string userId)
    {
        // we must wait until we are connected to try and perform any other actions
        await ServiceLocator.Current.Get<NetworkService>().Connect(userId);
        MenuManager.Instance.OpenMenu(MenuManager.Instance.initialMenu);
        
        gameObject.SetActive(false);
        // OnReconnectRoom();
    }

    public void ShowNotificationMessage(string title, string message)
    {
        NotificationMessage _message = new NotificationMessage();
        _message.message = message;
        _message.type = title;

        EventBus.Publish(new RoomReceieveMessageEvent<NotificationMessage>(_message));
    }
    
    public void OnReconnectRoom()
    {
        string roomId = PlayerPrefs.GetString("roomId");
        string token = PlayerPrefs.GetString("sessionId");
            
        Debug.Log(roomId);
        Debug.Log(token);
            
        if (string.IsNullOrEmpty(roomId))
        {
            ShowNotificationMessage("error", "Room id is empty");
        }

        if (string.IsNullOrEmpty(token))
        {
            ShowNotificationMessage("error", "Token is empty");
        }
            
        ServiceLocator.Current.Get<GameService>().ReConnectRoom(roomId, token);
    }

}
