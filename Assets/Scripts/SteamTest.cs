using System;
using UnityEngine;
using System.Collections;
using System.Linq;
using Facepunch.Steamworks;

public class SteamTest : MonoBehaviour
{
    public static Facepunch.Steamworks.Client SteamClient;
    private ServerList.Request serverRequest;

    void Start ()
	{
        //
        // Configure for Unity
        //
	    Facepunch.Steamworks.Config.ForUnity( Application.platform.ToString() );

        //
        // Create the steam client using Rust's AppId
        //
        SteamClient = new Client( 252490 );

        //
        // Make sure we started up okay
        //
        if ( !SteamClient.IsValid )
        {
            SteamClient.Dispose();
            SteamClient = null;
            return;
        }

        //
        // Request a list of servers
        //
	    {
	        serverRequest = SteamClient.ServerList.Internet();
	    }
	}

    void OnGUI()
    {
        GUILayout.BeginArea( new Rect( 16, 16, Screen.width - 32, Screen.height - 32 ) );

        if ( SteamClient != null )
        {
            GUILayout.Label( "SteamId: " + SteamClient.SteamId );
            GUILayout.Label( "Username: " + SteamClient.Username );

            GUILayout.Label( "Friend Count: " + SteamClient.Friends.AllFriends.Count() );
            GUILayout.Label( "Online Friend Count: " + SteamClient.Friends.AllFriends.Count( x => x.IsOnline ) );

            if ( SteamClient.Inventory.Definitions != null )
                GUILayout.Label( "Item Definitions: " + SteamClient.Inventory.Definitions.Length );

            if ( SteamClient.Inventory.Items != null )
                GUILayout.Label( "Item Count: " + SteamClient.Inventory.Items.Length );

            if ( serverRequest != null )
            {
                GUILayout.Label( "Server List: " + (serverRequest.Finished ? "Finished" : "Querying") );
                GUILayout.Label( "Servers Responded: " + serverRequest.Responded.Count );
                GUILayout.Label( "Servers Unresponsive: " + serverRequest.Unresponsive.Count );

                if ( serverRequest.Responded.Count > 0 )
                {
                    GUILayout.Label( "Last Server: " + serverRequest.Responded.Last().Name );
                }
            }
        }
        else
        {
            GUILayout.Label( "SteamClient Failed. Make sure appropriate files are in root, and steam is running and signed in." );
        }

        GUILayout.EndArea();
    }

    void Update()
    {
        if ( SteamClient != null )
        {
            SteamClient.Update();
        }
    }

}
