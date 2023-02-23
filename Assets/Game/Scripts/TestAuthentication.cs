using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BestHTTP;
using System;

public class TestAuthentication : MonoBehaviour
{

    string accessKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ0b2tlbl90eXBlIjoiYWNjZXNzIiwiZXhwIjoxNjc3MjA3NzgyLCJqdGkiOiI4ODZjZWNjOTI1ZTI0MWMwOWU0ZmVkOGVkMTM3NDVhNyIsInVzZXJfaWQiOjR9.29WT5dPlJbuL9wuyR8xC1zJawyRnkFp4OMRVW1Fk6XY";
    // Start is called before the first frame update
    void Start()
    {
        HTTPRequest request = new HTTPRequest(new Uri("http://127.0.0.1:9000/mainpage/players/me/"), OnRequestFinished);
        request.AddHeader("Authorization", "JWT " + accessKey);
        request.Send();

        // HTTPRequest request = new HTTPRequest(new Uri("http://127.0.0.1:9000/auth/jwt/create"), HTTPMethods.Post, OnRequestFinished);
        // request.AddField("username", "player2");
        // request.AddField("password", "asd860221");
        // request.Send();
    }

    private void OnRequestFinished(HTTPRequest request, HTTPResponse response)
    {
        Debug.Log("Request Finished! Text received: " + response.DataAsText);
    }
}
