using UnityEngine;
using Firebase;
using Firebase.Unity.Editor;
using Firebase.Database;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

public static class FirebaseDBHandler
{
    static Firebase.Auth.FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
    private const string projectId = "yipli-project"; //Taken from Firebase project settings
    //private static readonly string databaseURL = "https://yipli-project.firebaseio.com/"; // Taken from Firebase project settings

    public delegate void PostUserCallback();

    // Adds a PlayerSession to the Firebase Database
    public static void PostPlayerSession(PlayerSession session, PostUserCallback callback)
    {
        auth.SignInAnonymouslyAsync().ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInAnonymouslyAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInAnonymouslyAsync encountered an error: " + task.Exception);
                return;
            }

            Firebase.Auth.FirebaseUser newUser = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);

            FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://yipli-project.firebaseio.com/");
            DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;

            string key = reference.Child("stage-bucket/player-sessions").Push().Key;
            reference.Child("stage-bucket/player-sessions").Child(key).SetRawJsonValueAsync(JsonConvert.SerializeObject(session.GetJsonDic(), Formatting.None, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            }));
        });
    }

    public static void ChangeCurrentPlayer(string strUserId, string strPlayerId, PostUserCallback callback)
    {
        auth.SignInAnonymouslyAsync().ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInAnonymouslyAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInAnonymouslyAsync encountered an error: " + task.Exception);
                return;
            }

            Firebase.Auth.FirebaseUser newUser = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);

            FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://yipli-project.firebaseio.com/");
            DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;

            reference.Child("profiles/users").Child(strUserId).Child("current-player-id").SetValueAsync(strPlayerId);
        });
     }

    public static async Task<List<YipliPLayerInfo>> GetAllPlayerdetails(string userId, string playerId, PostUserCallback callback)
    {
        if (userId == null)
        {
            Debug.Log("User ID not found");
        }

        List<YipliPLayerInfo> players = new List<YipliPLayerInfo>();
        DataSnapshot snapshot = null;
        try
        {
            Firebase.Auth.FirebaseUser newUser = await auth.SignInAnonymouslyAsync();
            Debug.LogFormat("User signed in successfully: {0} ({1})",
            newUser.DisplayName, newUser.UserId);

            FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://yipli-project.firebaseio.com/");
            DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
            snapshot = await reference.Child("profiles/users/" + userId).Child("/players").GetValueAsync();

            foreach (var childSnapshot in snapshot.Children)
            {
                YipliPLayerInfo playerInstance = new YipliPLayerInfo(childSnapshot, childSnapshot.Key);
                players.Add(playerInstance);
            }
        }
        catch
        {
            Debug.Log("Failed to GetAllPlayerdetails");
        }
        return players;
    }

    public static async Task<YipliPLayerInfo> GetCurrentPlayerdetails(string userId, PostUserCallback callback)
    {
        Debug.Log("Getting the Default player from backend");
        if (userId.Equals(null) || userId.Equals(""))
        {
            Debug.Log("User ID not found");
        }

        DataSnapshot snapshot = null;
        YipliPLayerInfo defaultPlayer = new YipliPLayerInfo();
        try
        {
            Firebase.Auth.FirebaseUser newUser = await auth.SignInAnonymouslyAsync();
            Debug.LogFormat("User signed in successfully: {0} ({1})",
            newUser.DisplayName, newUser.UserId);

            FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://yipli-project.firebaseio.com/");
            DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;

            //First get the current player id from user Id
            snapshot = await reference.Child("profiles/users").Child(userId).GetValueAsync();

            string playerId = snapshot.Child("current-player-id").Value?.ToString() ?? "";
            //Now get the complete player details from Player Id
            DataSnapshot defaultPlayerSnapshot = await reference.Child("profiles/users/" + userId + "/players/" + playerId).GetValueAsync();

            defaultPlayer = new YipliPLayerInfo(defaultPlayerSnapshot, defaultPlayerSnapshot.Key);
        }
        catch
        {
            Debug.Log("Failed to GetAllPlayerdetails");
        }
        return defaultPlayer;
    }

    // Mat related queries
    //Fetches the default mat details
    /*public static async Task<YipliMatInfo> GetCurrentMatDetails(string userId, PostUserCallback callback)
    {
        Debug.Log("Getting the Default player from backend");
        if (userId.Equals(null) || userId.Equals(""))
        {
            Debug.Log("User ID not found");
        }

        DataSnapshot snapshot = null;
        YipliMatInfo defaultMat = new YipliMatInfo();
        try
        {
            Firebase.Auth.FirebaseUser newUser = await auth.SignInAnonymouslyAsync();
            Debug.LogFormat("User signed in successfully: {0} ({1})",
            newUser.DisplayName, newUser.UserId);

            FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://yipli-project.firebaseio.com/");
            DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;

            //First get the current player id from user Id
            snapshot = await reference.Child("users").Child(userId).GetValueAsync();

            string matId = snapshot.Child("current-mat-id").Value?.ToString() ?? "";
            //Now get the complete player details from Player Id
            DataSnapshot defaultMatSnapshot = await reference.Child("mats").Child(matId).GetValueAsync();

            defaultMat = new YipliMatInfo(defaultMatSnapshot, defaultMatSnapshot.Key);
        }
        catch
        {
            Debug.Log("Failed to GetAllPlayerdetails");
        }
        return defaultMat;
    }*/

    //Gets all mats registered under the current USER
    public static async Task<List<YipliMatInfo>> GetAllMatDetails(string userId, PostUserCallback callback)
    {
        if (userId == null)
        {
            Debug.Log("User ID not found");
        }

        List<YipliMatInfo> mats = new List<YipliMatInfo>();
        DataSnapshot snapshot = null;
        try
        {
            Firebase.Auth.FirebaseUser newUser = await auth.SignInAnonymouslyAsync();
            Debug.LogFormat("User signed in successfully: {0} ({1})",
            newUser.DisplayName, newUser.UserId);

            FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://yipli-project.firebaseio.com/");
            DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
            snapshot = await reference.Child("/profiles/users/" + userId + "/mats").GetValueAsync();

            foreach (var childSnapshot in snapshot.Children)
            {
                YipliMatInfo playerInstance = new YipliMatInfo(childSnapshot, childSnapshot.Key);
                mats.Add(playerInstance);
            }
        }
        catch
        {
            Debug.Log("Failed to GetAllPlayerdetails");
        }
        return mats;
    }

    // changes the default mat for the user
    /*public static void ChangeCurrentMat(string strUserId, string strMatId, PostUserCallback callback)
    {
        auth.SignInAnonymouslyAsync().ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInAnonymouslyAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInAnonymouslyAsync encountered an error: " + task.Exception);
                return;
            }

            Firebase.Auth.FirebaseUser newUser = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);

            FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://yipli-project.firebaseio.com/");
            DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;

            reference.Child("users").Child(strUserId).Child("current-mat-id").SetValueAsync(strMatId);
        });
    }*/
}
