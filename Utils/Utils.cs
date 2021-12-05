using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Text;
using Newtonsoft.Json.Linq;
using Leaf.xNet;

public partial class Utils
{
    public static Random rand = new Random();

    public static List<string> UsedStrings = new List<string>();
    public static List<string> queue = new List<string>();
    public static List<Tuple<string, string>> languages = new List<Tuple<string, string>>();
    public static bool hideLiveLogs = false;
    public static int tokenLimit = 1;
    public static string tokensList = "";

    public static Leaf.xNet.HttpRequest CreateBasicRequest()
    {
        try
        {
            HttpRequest request = new HttpRequest();

            request.KeepTemporaryHeadersOnRedirect = false;
            request.EnableMiddleHeaders = false;
            request.ClearAllHeaders();
            request.AllowEmptyHeaderValues = false;
            request.SslProtocols = System.Security.Authentication.SslProtocols.Tls12;
            request.Proxy = null;
            request.Username = null;
            request.UserAgent = null;
            request.UseCookies = false;
            request.CookieSingleHeader = true;
            request.Authorization = null;
            request.BaseAddress = null;
            request.Referer = null;
            request.Reconnect = false;
            request.ReconnectDelay = 0;
            request.Password = null;
            request.KeepAlive = false;
            request.IgnoreInvalidCookie = true;
            request.IgnoreProtocolErrors = true;
            request.KeepTemporaryHeadersOnRedirect = false;
            request.MaximumKeepAliveRequests = 1;
            request.Cookies = null;
            request.CharacterSet = null;
            request.AcceptEncoding = null;
            request.Culture = null;
            request.AllowAutoRedirect = false;
            request.MaximumAutomaticRedirections = 1;

            return request;
        }
        catch
        {
            return new HttpRequest();
        }
    }

    public static List<string> GetTokensList()
    {
        List<string> tokens = new List<string>();
        int i = 0;

        foreach (string token in SplitToLines(tokensList))
        {
            tokens.Add(token);
            i++;

            if (i == tokenLimit)
            {
                break;
            }
        }
        
        return tokens;
    }

    public static int GetTotalTokensCount()
    {
        List<string> tokens = new List<string>();
        int i = 0;

        foreach (string token in SplitToLines(tokensList))
        {
            tokens.Add(token);
            i++;
        }

        return i;
    }

    public static string GetRandomToken()
    {
        return GetTokensList()[GetTokensList().Count - 1];
    }

    public static string getTokenLanguage(string token)
    {
        try
        {
            foreach (Tuple<string, string> theTuple in languages)
            {
                try
                {
                    if (theTuple.Item1 == token)
                    {
                        return theTuple.Item2;
                    }
                }
                catch
                {

                }
            }

            var httpRequest = new Leaf.xNet.HttpRequest();

            httpRequest.KeepTemporaryHeadersOnRedirect = false;
            httpRequest.EnableMiddleHeaders = false;
            httpRequest.ClearAllHeaders();
            httpRequest.AllowEmptyHeaderValues = false;

            httpRequest.AddHeader("Accept", "*/*");
            httpRequest.AddHeader("Accept-Encoding", "gzip, deflate, br");
            httpRequest.AddHeader("Accept-Language", "it");
            httpRequest.AddHeader("Authorization", token);
            httpRequest.AddHeader("Connection", "keep-alive");
            httpRequest.AddHeader("Cookie", Utils.GetRandomCookie());
            httpRequest.AddHeader("DNT", "1");
            httpRequest.AddHeader("Host", "discord.com");
            httpRequest.AddHeader("Origin", "https://discord.com");
            httpRequest.AddHeader("Referer", "https://discord.com/channels/@me");
            httpRequest.AddHeader("TE", "Trailers");
            httpRequest.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:88.0) Gecko/20100101 Firefox/88.0");
            httpRequest.AddHeader("X-Super-Properties", "eyJvcyI6IldpbmRvd3MiLCJicm93c2VyIjoiRmlyZWZveCIsImRldmljZSI6IiIsImJyb3dzZXJfdXNlcl9hZ2VudCI6Ik1vemlsbGEvNS4wIChXaW5kb3dzIE5UIDEwLjA7IFdpbjY0OyB4NjQ7IHJ2Ojg3LjApIEdlY2tvLzIwMTAwMTAxIEZpcmVmb3gvODcuMCIsImJyb3dzZXJfdmVyc2lvbiI6Ijg3LjAiLCJvc192ZXJzaW9uIjoiMTAiLCJyZWZlcnJlciI6IiIsInJlZmVycmluZ19kb21haW4iOiIiLCJyZWZlcnJlcl9jdXJyZW50IjoiIiwicmVmZXJyaW5nX2RvbWFpbl9jdXJyZW50IjoiIiwicmVsZWFzZV9jaGFubmVsIjoic3RhYmxlIiwiY2xpZW50X2J1aWxkX251bWJlciI6ODE2NDgsImNsaWVudF9ldmVudF9zb3VyY2UiOm51bGx9");

            httpRequest.SslProtocols = System.Security.Authentication.SslProtocols.Tls12;

            dynamic jss = JObject.Parse(Utils.DecompressResponse(httpRequest.Get("https://discord.com/api/v9/users/@me/settings")));
            string locale = (string)jss.locale;

            languages.Add(new Tuple<string, string>(token, locale));

            return locale;
        }
        catch
        {
            return "";
        }
    }

    public static string GetInviteCodeByInviteLink(string inviteLink)
    {
        try
        {
            if (inviteLink.EndsWith("/"))
            {
                inviteLink = inviteLink.Substring(0, inviteLink.Length - 1);
            }

            if (inviteLink.Contains("discord") && inviteLink.Contains("/") && inviteLink.Contains("http"))
            {
                string[] splitter = Microsoft.VisualBasic.Strings.Split(inviteLink, "/");
                return splitter[splitter.Length - 1];
            }
        }
        catch
        {

        }

        return inviteLink;
    }

    public static Tuple<bool, string, string, string> GetInviteInformations(string inviteCode, string token)
    {
        try
        {
            inviteCode = GetInviteCodeByInviteLink(inviteCode);

            if (inviteCode.Length == 18)
            {
                return new Tuple<bool, string, string, string>(false, inviteCode, "", "");
            }

            var httpRequest = new Leaf.xNet.HttpRequest();

            httpRequest.KeepTemporaryHeadersOnRedirect = false;
            httpRequest.EnableMiddleHeaders = false;
            httpRequest.ClearAllHeaders();
            httpRequest.AllowEmptyHeaderValues = false;

            httpRequest.AddHeader("Accept", "*/*");
            httpRequest.AddHeader("Accept-Encoding", "gzip, deflate, br");
            httpRequest.AddHeader("Accept-Language", getTokenLanguage(token));
            httpRequest.AddHeader("Authorization", token);
            httpRequest.AddHeader("Connection", "keep-alive");
            httpRequest.AddHeader("Cookie", Utils.GetRandomCookie(getTokenLanguage(token)));
            httpRequest.AddHeader("DNT", "1");
            httpRequest.AddHeader("Host", "discord.com");
            httpRequest.AddHeader("Referer", "https://discord.com/channels/@me");
            httpRequest.AddHeader("TE", "Trailers");
            httpRequest.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:88.0) Gecko/20100101 Firefox/88.0");
            httpRequest.AddHeader("X-Super-Properties", "eyJvcyI6IldpbmRvd3MiLCJicm93c2VyIjoiRmlyZWZveCIsImRldmljZSI6IiIsImJyb3dzZXJfdXNlcl9hZ2VudCI6Ik1vemlsbGEvNS4wIChXaW5kb3dzIE5UIDEwLjA7IFdpbjY0OyB4NjQ7IHJ2Ojg3LjApIEdlY2tvLzIwMTAwMTAxIEZpcmVmb3gvODcuMCIsImJyb3dzZXJfdmVyc2lvbiI6Ijg3LjAiLCJvc192ZXJzaW9uIjoiMTAiLCJyZWZlcnJlciI6IiIsInJlZmVycmluZ19kb21haW4iOiIiLCJyZWZlcnJlcl9jdXJyZW50IjoiIiwicmVmZXJyaW5nX2RvbWFpbl9jdXJyZW50IjoiIiwicmVsZWFzZV9jaGFubmVsIjoic3RhYmxlIiwiY2xpZW50X2J1aWxkX251bWJlciI6ODE0NTIsImNsaWVudF9ldmVudF9zb3VyY2UiOm51bGx9");

            httpRequest.SslProtocols = System.Security.Authentication.SslProtocols.Tls12;

            dynamic jss = JObject.Parse(Utils.DecompressResponse(httpRequest.Get("https://discord.com/api/v9/invites/" + inviteCode + "?inputValue=https://discord.gg/" + inviteCode + "&with_counts=true")));

            string guildId = "", channelId = "", channelType = "", statusCode = (string)jss.code;
            bool status = true;

            if (statusCode == "10006" || statusCode == "0")
            {
                status = false;
            }

            if (status)
            {
                guildId = (string)jss.guild.id;
                channelId = (string)jss.channel.id;
                channelType = (string)jss.channel.type;
            }

            return new Tuple<bool, string, string, string>(status, guildId, channelId, channelType);
        }
        catch
        {
            return new Tuple<bool, string, string, string>(false, "", "", "");
        }
    }

    public static IEnumerable<string> SplitToLines(string input)
    {
        if (input == null)
        {
            yield break;
        }

        using (System.IO.StringReader reader = new System.IO.StringReader(input))
        {
            string line;

            while ((line = reader.ReadLine()) != null)
            {
                yield return line;
            }
        }
    }

    public static bool IsIDValid(string id)
    {
        if (id.Length != 18)
        {
            return false;
        }

        if (!Microsoft.VisualBasic.Information.IsNumeric(id))
        {
            return false;
        }

        return true;
    }

    public static long LongRandom(long min, long max, Random rand)
    {
        byte[] buf = new byte[8];
        rand.NextBytes(buf);
        long longRand = BitConverter.ToInt64(buf, 0);

        return (Math.Abs(longRand % (max - min)) + min);
    }

    public static string GetRandomCookie(string lang)
    {
        return "__cfduid=d1d" + GetRandomNumber(100, 900).ToString() + "edabc" + GetRandomNumber(100, 900).ToString() + "b4b" + GetRandomNumber(1000, 9000).ToString() + "ad8d" + GetRandomNumber(10, 90).ToString() + "fd" + LongRandom(10000000000000L, 90000000000000L, rand).ToString() + "; __dcfduid=9a8e" + Utils.GetRandomNumber(1000, 9000).ToString() + "d2dbb82a" + Utils.GetRandomNumber(100, 900).ToString() + "a3c8d160b" + GetRandomNumber(1000, 9000).ToString() + "; locale=" + lang;
    }

    public static string GetRandomCookie()
    {
        return GetRandomCookie("it");
    }

    public static string Base64Encode(string plainText)
    {
        return System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(plainText));
    }

    public static string GetXCP(string guildId, string channelId, string channelType)
    {
        try
        {
            return Base64Encode("{\"location\":\"Join Guild\",\"location_guild_id\":\"" + guildId + "\",\"location_channel_id\":\"" + channelId + "\",\"location_channel_type\":" + channelType + "}");
        }
        catch
        {
            return "eyJsb2NhdGlvbiI6IkpvaW4gR3VpbGQiLCJsb2NhdGlvbl9ndWlsZF9pZCI6IjgyMjU4NDA5NTg5MTY1MjYyOSIsImxvY2F0aW9uX2NoYW5uZWxfaWQiOiI4MjI1ODQwOTYzNzA3MjA3NjgiLCJsb2NhdGlvbl9jaGFubmVsX3R5cGUiOjB9";
        }
    }

    public static string RandomNormalString(int length)
    {
        string Chr = "abcedfghijklmnopqrstuvxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        var sb = new StringBuilder();

        for (int i = 1, loopTo = length; i <= loopTo; i++)
        {
            int idx = rand.Next(0, Chr.Length);

            sb.Append(Chr.Substring(idx, 1));
        }

        if (UsedStrings.Contains(sb.ToString()))
        {
            while (UsedStrings.Contains(sb.ToString()))
            {
                sb.Clear();

                for (int i = 1, loopTo1 = length; i <= loopTo1; i++)
                {
                    int idx = rand.Next(0, Chr.Length);

                    sb.Append(Chr.Substring(idx, 1));
                }
            }
        }

        UsedStrings.Add(sb.ToString());

        return sb.ToString();
    }

    public static int GetRandomNumber(int cap)
    {
        return rand.Next(0, cap);
    }

    public static int GetRandomNumber(int min, int cap)
    {
        return rand.Next(min, cap);
    }

    public static byte[] DecompressGZip(byte[] toDecompress)
    {
        MemoryStream stream = new MemoryStream(toDecompress);
        MemoryStream newStream = new MemoryStream();

        using (GZipStream decompressionStream = new GZipStream(stream, CompressionMode.Decompress))
        {
            decompressionStream.CopyTo(newStream);
        }

        return newStream.ToArray();
    }

    public static byte[] DecompressDeflate(byte[] toDecompress)
    {
        MemoryStream stream = new MemoryStream(toDecompress);
        MemoryStream newStream = new MemoryStream();

        using (DeflateStream decompressionStream = new DeflateStream(stream, CompressionMode.Decompress))
        {
            decompressionStream.CopyTo(newStream);
        }

        return newStream.ToArray();
    }

    public static byte[] DecompressBr(byte[] toDecompress)
    {
        return BrotliSharpLib.Brotli.DecompressBuffer(toDecompress, 0, toDecompress.Length);
    }

    public static string DecompressResponse(Leaf.xNet.HttpResponse response)
    {
        Dictionary<string, string>.Enumerator enumerator = response.EnumerateHeaders();
        Dictionary<string, string> theDictionary = new Dictionary<string, string>();

        string theKey = "a";

        while (theKey != null && theKey.Replace(" ", "").Replace('\t'.ToString(), "") != "")
        {
            try
            {
                enumerator.MoveNext();

                theDictionary.Add(enumerator.Current.Key.ToLower(), enumerator.Current.Value);

                theKey = enumerator.Current.Key;
            }
            catch
            {
                break;
            }
        }

        try
        {
            if (theDictionary["content-encoding"].Equals("br"))
            {
                return System.Text.Encoding.UTF8.GetString(DecompressBr(response.ToBytes()));
            }
            else if (theDictionary["content-encoding"].Equals("deflate"))
            {
                return System.Text.Encoding.UTF8.GetString(DecompressDeflate(response.ToBytes()));
            }
            else if (theDictionary["content-encoding"].Equals("gzip"))
            {
                return System.Text.Encoding.UTF8.GetString(DecompressGZip(response.ToBytes()));
            }
            else
            {
                return response.ToString();
            }
        }
        catch
        {
            return response.ToString();
        }
    }
}