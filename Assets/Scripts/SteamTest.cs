using System;
using UnityEngine;
using System.Collections;
using System.Linq;
using Facepunch.Steamworks;

public class SteamTest : MonoBehaviour
{
    private ServerList.Request serverRequest;
    private Leaderboard leaderBoard;

    void Start ()
	{
        //
        // Don't destroy this when loading new scenes
        //
        DontDestroyOnLoad( gameObject );

        //
        // Configure for Unity
        // This is VERY important - call this before 
        //
	    Facepunch.Steamworks.Config.ForUnity( Application.platform.ToString() );

        //
        // Create the steam client using Rust's AppId
        //
        new Facepunch.Steamworks.Client( 252490 );

        //
        // Make sure we started up okay
        //
        if ( Client.Instance == null )
        {
            Debug.LogError( "Error starting Steam!" );
            return;
        }

        //
        // Request a list of servers
        //
	    {
	        serverRequest = Client.Instance.ServerList.Internet();
	    }

        //
        // Request a leaderboard
        //
	    {
            leaderBoard = Client.Instance.GetLeaderboard( "TestLeaderboard", Client.LeaderboardSortMethod.Ascending, Client.LeaderboardDisplayType.Numeric );
        }
	}

    private void OnDestroy()
    {
        if ( Client.Instance != null )
        {
            Client.Instance.Dispose();
        }
    }

    void OnGUI()
    {
        GUILayout.BeginArea( new Rect( 16, 16, Screen.width - 32, Screen.height - 32 ) );

        if ( Client.Instance != null )
        {
            GUILayout.Label( "SteamId: " + Client.Instance.SteamId );
            GUILayout.Label( "Username: " + Client.Instance.Username );

            GUILayout.Label( "Friend Count: " + Client.Instance.Friends.AllFriends.Count() );
            GUILayout.Label( "Online Friend Count: " + Client.Instance.Friends.AllFriends.Count( x => x.IsOnline ) );

            if ( Client.Instance.Inventory.Definitions != null )
                GUILayout.Label( "Item Definitions: " + Client.Instance.Inventory.Definitions.Length );

            if ( Client.Instance.Inventory.Items != null )
                GUILayout.Label( "Item Count: " + Client.Instance.Inventory.Items.Length );

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

            if ( leaderBoard != null )
            {
                GUILayout.Label( "leaderBoard.IsValid: " + leaderBoard.IsValid );
                GUILayout.Label( "leaderBoard.IsError: " + leaderBoard.IsError );

                if ( GUILayout.Button( "Refresh Leaderboard" ) )
                {
                    leaderBoard.FetchScores( Leaderboard.RequestType.Global, 0, 100 );

                    leaderBoard.AddScore( true, 456, 1, 2, 3, 4, 5, 6 );
                }

                if ( leaderBoard.IsQuerying )
                {
                    GUILayout.Label( "QUERYING.." ); 
                }
                else if ( leaderBoard.Results != null )
                {
                    foreach ( var result in leaderBoard.Results )
                    {
                        GUILayout.Label( string.Format( "{0}. {1} ({2})", result.GlobalRank, result.Name, result.Score ) );
                    }
                }
                else
                {
                    GUILayout.Label( "No Leaderboard results" );
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
        if ( Client.Instance != null )
        {
            Client.Instance.Update();
        }
    }

}
