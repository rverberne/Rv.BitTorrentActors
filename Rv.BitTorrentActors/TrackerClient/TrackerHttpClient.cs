using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace Rv.BitTorrentActors.TrackerClient;

public class TrackerHttpClient
{
    private readonly HttpClient httpClient;

    public TrackerHttpClient(HttpMessageHandler? handler = null)
    {
        httpClient = new HttpClient(handler ?? new HttpClientHandler());
    }

    public async Task<TrackerResponseDto> CallTracker(string uri, TrackerRequestDto request)
    {
        HttpResponseMessage httpRes = await httpClient.GetAsync(TrackerRequestToUri(uri, request));
        var result = HttpResponseToTrackerResponse(httpRes);
        return result;
    }

    private string TrackerRequestToUri(string uri, TrackerRequestDto req)
    {
        var query = new Dictionary<string, string>();
        query.Add("info_hash", HttpUtility.UrlEncode(req.InfoHash ?? new byte[0]));
        query.Add("peer_id", HttpUtility.UrlEncode(req.PeerId ?? new byte[0]));
        query.Add("port", req.Port?.ToString() ?? string.Empty);
        query.Add("uploaded", req.Uploaded?.ToString() ?? string.Empty);
        query.Add("downloaded", req.Downloaded?.ToString() ?? string.Empty);
        query.Add("left", req.Left?.ToString() ?? string.Empty);
        query.Add("compact", "1");
        query.Add("event", "started");

        string queryString = CreateQueryString(query);
        string result = $"{uri}?{queryString}";
        return result;
    }

    private string CreateQueryString(Dictionary<string, string> query)
    {
        string result = string.Join(
            '&',
            query.Select(p => $"{p.Key}={p.Value}"));
        return result;
    }

    private TrackerResponseDto HttpResponseToTrackerResponse(HttpResponseMessage res)
    {
        using (Stream stream = res.Content.ReadAsStreamAsync().Result)
        {
            var ser = new TrackerResponseSerializer();
            var result = ser.Deserialize(stream);
            return result;
        }
    }
}
