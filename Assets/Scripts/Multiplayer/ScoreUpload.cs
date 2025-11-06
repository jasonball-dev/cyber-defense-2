using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Multiplayer
{
    public class ScoreUpload : Singleton<ScoreUpload>
    {
        // frontend url https://d3smdgu2ijqhl9.cloudfront.net/   (https://shorturl.at/j2vsc)

        private const string GraphQlEndpoint =
            "https://57ytwon5lvcrraslr7xaae25yq.appsync-api.eu-central-1.amazonaws.com/graphql";

        private const string ApiKey = "da2-n4lf3ydd4zftxibv5pkzozj2o4";

        public void SendScore(string playerName, int playerScore)
        {
            var query = @"
        mutation MyMutation {
            createItem(input: {name: ""NAMEINPUT"", score: SCOREINPUT}) {
                id
                name
                score
            }
        }";

            query = query.Replace("NAMEINPUT", playerName);
            query = query.Replace("SCOREINPUT", playerScore.ToString());

            StartCoroutine(SendRequest(query));
        }

        private static IEnumerator SendRequest(string query)
        {
            var payload = JsonUtility.ToJson(new GraphQlQuery { query = query });

            var request = new UnityWebRequest(GraphQlEndpoint, "POST");
            var bodyRaw = Encoding.UTF8.GetBytes(payload);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();

            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("x-api-key", ApiKey);

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("Error: " + request.error);
            }
        }
    }

    [System.Serializable]
    public class GraphQlQuery
    {
        public string query;
    }
}