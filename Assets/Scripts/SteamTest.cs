using System;
using UnityEngine;
using System.Collections;
using System.Linq;
using Facepunch.Steamworks;

public class SteamTest : MonoBehaviour
{
    public static Facepunch.Steamworks.Client SteamClient;
    private ServerList.Request serverRequest;
    private Leaderboard leaderBoard;

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

        //
        // Request a leaderboard
        //
	    {
            leaderBoard = SteamClient.GetLeaderboard( "TestLeaderboard", Client.LeaderboardSortMethod.Ascending, Client.LeaderboardDisplayType.Numeric );
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

            if ( leaderBoard != null )
            {
                GUILayout.Label( "leaderBoard.IsValid: " + leaderBoard.IsValid );
                GUILayout.Label( "leaderBoard.IsError: " + leaderBoard.IsError );

                if ( GUILayout.Button( "Refresh Leaderboard" ) )
                {
                    leaderBoard.FetchScores( Leaderboard.RequestType.Global, 0, 100 );

                    leaderBoard.AddScore( true, true, 456, 1, 2, 3, 4, 5, 6 );
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
        if ( SteamClient != null )
        {
            SteamClient.Update();
        }
    }

}
