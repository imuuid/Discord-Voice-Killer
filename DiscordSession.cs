using Newtonsoft.Json.Linq;
using System.Threading;
using WebSocketSharp;
using System.Collections.Generic;
using System.Text;
using System;

public class DiscordSession
{
    private string token, voiceGuildId, voiceChannelId, voiceUserId;
    public string userId;
    private WebSocket ws;
    private bool connected, voiceConnected, microphone, headphones, webcam, screenshare, userScreenshare, autoReconnect;

    public DiscordSession(string token)
    {
        this.token = token;
    }

    public void joinVoice(string guildId, string channelId, string userId, bool microphone, bool headphones, bool webcam, bool screenshare, bool joinScreenshare, bool speakRequest, bool autoReconnect, bool autoLeave)
    {
        if (voiceConnected)
        {
            return;
        }

        this.voiceConnected = true;
        this.voiceGuildId = guildId;
        this.voiceChannelId = channelId;
        this.voiceUserId = userId;
        this.microphone = microphone;
        this.headphones = headphones;
        this.webcam = webcam;
        this.screenshare = screenshare;
        this.userScreenshare = joinScreenshare;
        this.autoReconnect = autoReconnect;

        this.updateVoiceState();
        
        if (screenshare)
        {
            this.startScreenshare();
        }

        if (joinScreenshare)
        {
            this.joinScreenshare();
        }

        if (speakRequest)
        {
            this.speakRequest();
        }

        if (autoLeave)
        {
            leaveVoice();
        }
    }

    private void speakRequest()
    {
        try
        {
            string timestamp = "";

            string year = "", month = "", day = "", hour = "", minute = "", second = "";

            year = DateTime.Now.Year.ToString();
            month = DateTime.Now.Month.ToString();

            if (month.Length == 1)
            {
                month = "0" + month;
            }

            day = DateTime.Now.Day.ToString();

            if (day.Length == 1)
            {
                day = "0" + day;
            }

            hour = DateTime.Now.Hour.ToString();

            if (hour.Length == 1)
            {
                hour = "0" + hour;
            }

            minute = DateTime.Now.Minute.ToString();

            if (minute.Length == 1)
            {
                minute = "0" + minute;
            }

            second = DateTime.Now.Minute.ToString();

            if (second.Length == 1)
            {
                second = "0" + second;
            }

            timestamp = year + "-" + month + "-" + day + "T" + hour + ":" + minute + ":" + second + "." + DateTime.Now.Millisecond.ToString() + "Z";

            string messageJson = "{\"request_to_speak_timestamp\":\"" + timestamp + "\",\"channel_id\":\"" + voiceChannelId + "\"}";

            Leaf.xNet.HttpRequest httpRequest = Utils.CreateBasicRequest();

            httpRequest.AddHeader("Accept", "*/*");
            httpRequest.AddHeader("Accept-Encoding", "gzip, deflate, br");
            httpRequest.AddHeader("Accept-Language", Utils.getTokenLanguage(token));
            httpRequest.AddHeader("Authorization",token);
            httpRequest.AddHeader("Connection", "keep-alive");
            httpRequest.AddHeader("Content-Length", messageJson.Length.ToString());
            httpRequest.AddHeader("Content-Type", "application/json");
            httpRequest.AddHeader("Cookie", Utils.GetRandomCookie(Utils.getTokenLanguage(token)));
            httpRequest.AddHeader("DNT", "1");
            httpRequest.AddHeader("Host", "discord.com");
            httpRequest.AddHeader("Origin", "https://discord.com");
            httpRequest.AddHeader("Referer", "https://discord.com/channels/" + voiceGuildId + "/" + voiceChannelId);
            httpRequest.AddHeader("TE", "Trailers");
            httpRequest.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:88.0) Gecko/20100101 Firefox/88.0");
            httpRequest.AddHeader("X-Super-Properties", "eyJvcyI6IldpbmRvd3MiLCJicm93c2VyIjoiRmlyZWZveCIsImRldmljZSI6IiIsImJyb3dzZXJfdXNlcl9hZ2VudCI6Ik1vemlsbGEvNS4wIChXaW5kb3dzIE5UIDEwLjA7IFdpbjY0OyB4NjQ7IHJ2Ojg3LjApIEdlY2tvLzIwMTAwMTAxIEZpcmVmb3gvODcuMCIsImJyb3dzZXJfdmVyc2lvbiI6Ijg3LjAiLCJvc192ZXJzaW9uIjoiMTAiLCJyZWZlcnJlciI6IiIsInJlZmVycmluZ19kb21haW4iOiIiLCJyZWZlcnJlcl9jdXJyZW50IjoiIiwicmVmZXJyaW5nX2RvbWFpbl9jdXJyZW50IjoiIiwicmVsZWFzZV9jaGFubmVsIjoic3RhYmxlIiwiY2xpZW50X2J1aWxkX251bWJlciI6ODE0NTIsImNsaWVudF9ldmVudF9zb3VyY2UiOm51bGx9");

            httpRequest.Patch("https://discord.com/api/v9/guilds/" + voiceGuildId + "/voice-states/@me", messageJson, "application/json");
        }
        catch
        {

        }
    }

    private void startScreenshare()
    {
        try
        {
            send("{\"op\":18,\"d\":{\"type\":\"guild\",\"guild_id\":\"" + voiceGuildId.ToString() + "\",\"channel_id\":\"" + voiceChannelId.ToString() + "\",\"preferred_region\":null}}");
            send("{\"op\":22,\"d\":{\"stream_key\":\"guild:" + voiceGuildId.ToString() + ":" + voiceChannelId.ToString() + ":" + userId.ToString() + "\",\"paused\":false}}");
        }
        catch
        {

        }
    }

    private void stopScreenshare()
    {
        try
        {
            send("{\"op\":19,\"d\":{\"stream_key\":\"guild:" + voiceGuildId.ToString() + ":" + voiceChannelId.ToString() + ":" + userId.ToString() + "}}");
        }
        catch
        {

        }
    }

    private void joinScreenshare()
    {
        try
        {
            send("{\"op\":20,\"d\":{\"stream_key\":\"guild:" + voiceGuildId.ToString() + ":" + voiceChannelId.ToString() + ":" + voiceUserId.ToString() + "\"}}");
        }
        catch
        {

        }
    }

    private void leaveScreenshare()
    {
        try
        {
            send("{\"op\":19,\"d\":{\"stream_key\":\"guild:" + voiceGuildId.ToString() + ":" + voiceChannelId.ToString() + ":" + voiceUserId.ToString() + "\"}}");
        }
        catch
        {

        }
    }

    public void updateVoiceState()
    {
        send("{\"op\":4,\"d\":{\"guild_id\":\"" + this.voiceGuildId + "\",\"channel_id\":\"" + this.voiceChannelId + "\",\"self_mute\":" + (!microphone).ToString().ToLower() + ",\"self_deaf\":" + (!headphones).ToString().ToLower() + ",\"self_video\":" + webcam.ToString().ToLower() + ",\"preferred_region\":null}}");
    }

    private void decideScreenshare()
    {
        if (screenshare)
        {
            startScreenshare();
        }
        else
        {
            stopScreenshare();
        }
    }

    private void decideJoinScreenshare()
    {
        if (userScreenshare)
        {
            joinScreenshare();
        }
        else
        {
            leaveScreenshare();
        }
    }

    public void setMicrophone(bool enabled)
    {
        if (!voiceConnected)
        {
            return;
        }

        if (enabled == microphone)
        {
            return;
        }

        microphone = enabled;
        updateVoiceState();
    }

    public void setHeadphones(bool enabled)
    {
        if (!voiceConnected)
        {
            return;
        }

        if (enabled == headphones)
        {
            return;
        }

        headphones = enabled;
        updateVoiceState();
    }

    public void setWebcam(bool enabled)
    {
        if (!voiceConnected)
        {
            return;
        }

        if (enabled == webcam)
        {
            return;
        }

        webcam = enabled;
        updateVoiceState();
    }

    public void setScreenshare(bool enabled)
    {
        try
        {
            if (!voiceConnected)
            {
                return;
            }

            if (enabled == screenshare)
            {
                return;
            }

            screenshare = enabled;
            decideScreenshare();
        }
        catch
        {

        }
    }

    public void setJoinScreenshare(bool enabled, string userId = "")
    {
        try
        {
            if (!voiceConnected)
            {
                return;
            }

            if (enabled == userScreenshare)
            {
                return;
            }

            userScreenshare = enabled;
            voiceUserId = userId;
            decideJoinScreenshare();
        }
        catch
        {

        }
    }

    public void leaveVoice()
    {
        if (!voiceConnected)
        {
            return;
        }

        reset();
        send("{\"op\":4,\"d\":{\"guild_id\":null,\"channel_id\":null,\"self_mute\":true,\"self_deaf\":false,\"self_video\":false}}");
    }

    public void reset()
    {
        voiceConnected = false;
        microphone = false;
        headphones = false;
        webcam = false;
        screenshare = false;
        userScreenshare = false;
        voiceGuildId = "";
        voiceUserId = "";
        voiceChannelId = "";
    }

    public void send(string str)
    {
        try
        {
            ws.Send(Encoding.UTF8.GetBytes(str));
        }
        catch
        {

        }
    }

    public void sendAsync(string str)
    {
        try
        {
            ws.SendAsync(Encoding.UTF8.GetBytes(str), null);
        }
        catch
        {

        }
    }

    public void connectWebSocket(bool heartbeats)
    {
        try
        {
            if (connected)
            {
                return;
            }

            ws = new WebSocket("wss://gateway.discord.gg/?encoding=json&v=9");

            ws.CustomHeaders = new Dictionary<string, string>
            {
            { "Accept", "*/*" },
            { "Accept-Encoding", "gzip, deflate, br" },
            { "Accept-Language", "it-IT,it;q=0.8,en-US;q=0.5,en;q=0.3" },
            { "Cache-Control", "no-cache" },
            { "Connection", "keep-alive, Upgrade" },
            { "DNT", "1" },
            { "Host", "gateway.discord.gg" },
            { "Origin", "https://discord.com" },
            { "Pragma", "no-cache" },
            { "Sec-WebSocket-Extensions", "permessage-deflate" },
            { "Sec-WebSocket-Version", "13" },
            { "Upgrade", "websocket" },
            { "User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:88.0) Gecko/20100101 Firefox/88.0" }
            };

            ws.SslConfiguration.EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12;
            ws.Origin = "https://discord.com";
            ws.EnableRedirection = false;
            ws.EmitOnPing = false;

            ws.OnMessage += Ws_OnMessage;

            ws.Connect();
            ws.Send("{\"op\":2,\"d\":{\"token\":\"" + this.token + "\",\"capabilities\":61,\"properties\":{\"os\":\"Windows\",\"browser\":\"Firefox\",\"device\":\"\",\"system_locale\":\"it-IT\",\"browser_user_agent\":\"Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:88.0) Gecko/20100101 Firefox/88.0\",\"browser_version\":\"88.0\",\"os_version\":\"10\",\"referrer\":\"\",\"referring_domain\":\"\",\"referrer_current\":\"\",\"referring_domain_current\":\"\",\"release_channel\":\"stable\",\"client_build_number\":83364,\"client_event_source\":null},\"presence\":{\"status\":\"online\",\"since\":0,\"activities\":[],\"afk\":false},\"compress\":false,\"client_state\":{\"guild_hashes\":{},\"highest_last_message_id\":\"0\",\"read_state_version\":0,\"user_guild_settings_version\":-1}}}");
            connected = true;
            reset();

            if (heartbeats)
            {
                new Thread(sendHeartbeats).Start();
            }
        }
        catch
        {

        }
    }

    private void Ws_OnMessage(object sender, MessageEventArgs e)
    {
        try
        {
            dynamic jss = JObject.Parse(e.Data);

            if ((string) jss.t == "VOICE_STATE_UPDATE" && (string) jss.d.member.user.id == userId && this.autoReconnect)
            {
                this.voiceConnected = false;
                this.joinVoice(this.voiceGuildId, this.voiceChannelId, this.voiceUserId, this.microphone, this.headphones, this.webcam, this.screenshare, this.userScreenshare, false, this.autoReconnect, false);
            }
        }
        catch
        {

        }
    }

    public string getUserId()
    {
        try
        {
            if (userId != "" && userId != null)
            {
                return userId;
            }

            Leaf.xNet.HttpRequest httpRequest = Utils.CreateBasicRequest();

            httpRequest.AddHeader("Accept", "*/*");
            httpRequest.AddHeader("Accept-Encoding", "gzip, deflate, br");
            httpRequest.AddHeader("Accept-Language", "it");
            httpRequest.AddHeader("Authorization", token);
            httpRequest.AddHeader("Connection", "keep-alive");
            httpRequest.AddHeader("Cookie", Utils.GetRandomCookie(Utils.getTokenLanguage(token)));
            httpRequest.AddHeader("DNT", "1");
            httpRequest.AddHeader("Host", "discord.com");
            httpRequest.AddHeader("Origin", "https://discord.com");
            httpRequest.AddHeader("Referer", "https://discord.com/channels/@me");
            httpRequest.AddHeader("TE", "Trailers");
            httpRequest.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:88.0) Gecko/20100101 Firefox/88.0");
            httpRequest.AddHeader("X-Super-Properties", "eyJvcyI6IldpbmRvd3MiLCJicm93c2VyIjoiRmlyZWZveCIsImRldmljZSI6IiIsImJyb3dzZXJfdXNlcl9hZ2VudCI6Ik1vemlsbGEvNS4wIChXaW5kb3dzIE5UIDEwLjA7IFdpbjY0OyB4NjQ7IHJ2Ojg3LjApIEdlY2tvLzIwMTAwMTAxIEZpcmVmb3gvODcuMCIsImJyb3dzZXJfdmVyc2lvbiI6Ijg3LjAiLCJvc192ZXJzaW9uIjoiMTAiLCJyZWZlcnJlciI6IiIsInJlZmVycmluZ19kb21haW4iOiIiLCJyZWZlcnJlcl9jdXJyZW50IjoiIiwicmVmZXJyaW5nX2RvbWFpbl9jdXJyZW50IjoiIiwicmVsZWFzZV9jaGFubmVsIjoic3RhYmxlIiwiY2xpZW50X2J1aWxkX251bWJlciI6ODE2NDgsImNsaWVudF9ldmVudF9zb3VyY2UiOm51bGx9");

            dynamic jss = JObject.Parse(Utils.DecompressResponse(httpRequest.Get("https://discord.com/api/v9/users/@me")));
            string locale = (string)jss.id;
            userId = locale;

            return locale;
        }
        catch
        {
            return "";
        }
    }

    private void sendHeartbeats()
    {
        while (connected)
        {
            Thread.Sleep(39000);

            try
            {
                ws.Send("{\"op\":1,\"d\":null}");
            }
            catch
            {

            }
        }
    }

    public void disconnectWebSocket()
    {
        try
        {
            if (!connected)
            {
                return;
            }

            connected = false;
            ws.Close();
            ws = null;
            reset();
        }
        catch
        {

        }
    }

    public void joinGuild(string invite, string xcp64, string guildId, string channelId)
    {
        try
        {
            Leaf.xNet.HttpRequest httpRequest = Utils.CreateBasicRequest();

            httpRequest.AddHeader("Accept", "*/*");
            httpRequest.AddHeader("Accept-Encoding", "gzip, deflate, br");
            httpRequest.AddHeader("Accept-Language", Utils.getTokenLanguage(token));
            httpRequest.AddHeader("Authorization", token);
            httpRequest.AddHeader("Connection", "keep-alive");
            httpRequest.AddHeader("Content-Length", "0");
            httpRequest.AddHeader("Cookie", Utils.GetRandomCookie(Utils.getTokenLanguage(token)));
            httpRequest.AddHeader("DNT", "1");
            httpRequest.AddHeader("Host", "discord.com");
            httpRequest.AddHeader("Origin", "https://discord.com");
            httpRequest.AddHeader("TE", "Trailers");
            httpRequest.AddHeader("Referer", "https://discord.com/channels/@me");
            httpRequest.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:88.0) Gecko/20100101 Firefox/88.0");
            httpRequest.AddHeader("X-Context-Properties", xcp64);
            httpRequest.AddHeader("X-Super-Properties", "eyJvcyI6IldpbmRvd3MiLCJicm93c2VyIjoiRmlyZWZveCIsImRldmljZSI6IiIsInN5c3RlbV9sb2NhbGUiOiJpdC1JVCIsImJyb3dzZXJfdXNlcl9hZ2VudCI6Ik1vemlsbGEvNS4wIChXaW5kb3dzIE5UIDEwLjA7IFdpbjY0OyB4NjQ7IHJ2Ojg4LjApIEdlY2tvLzIwMTAwMTAxIEZpcmVmb3gvODguMCIsImJyb3dzZXJfdmVyc2lvbiI6Ijg4LjAiLCJvc192ZXJzaW9uIjoiMTAiLCJyZWZlcnJlciI6IiIsInJlZmVycmluZ19kb21haW4iOiIiLCJyZWZlcnJlcl9jdXJyZW50IjoiIiwicmVmZXJyaW5nX2RvbWFpbl9jdXJyZW50IjoiIiwicmVsZWFzZV9jaGFubmVsIjoic3RhYmxlIiwiY2xpZW50X2J1aWxkX251bWJlciI6ODMwNDAsImNsaWVudF9ldmVudF9zb3VyY2UiOm51bGx9");

            dynamic jss = JObject.Parse(Utils.DecompressResponse(httpRequest.Post("https://discord.com/api/v9/invites/" + Utils.GetInviteCodeByInviteLink(invite))));

            if ((bool) jss.show_verification_form)
            {
                Thread.Sleep(500);   
                bypassRules(Utils.GetInviteCodeByInviteLink(invite), guildId, channelId);
            }
        }
        catch
        {

        }
    }

    private void bypassRules(string code, string guildId, string channelId)
    {
        Leaf.xNet.HttpRequest httpRequest = Utils.CreateBasicRequest();

        httpRequest.AddHeader("Accept", "*/*");
        httpRequest.AddHeader("Accept-Encoding", "gzip, deflate, br");
        httpRequest.AddHeader("Accept-Language", Utils.getTokenLanguage(token));
        httpRequest.AddHeader("Authorization", token);
        httpRequest.AddHeader("Connection", "keep-alive");
        httpRequest.AddHeader("Cookie", Utils.GetRandomCookie(Utils.getTokenLanguage(token)));
        httpRequest.AddHeader("DNT", "1");
        httpRequest.AddHeader("Host", "discord.com");
        httpRequest.AddHeader("Referer", "https://discord.com/channels/" + guildId + "/" + channelId);
        httpRequest.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:88.0) Gecko/20100101 Firefox/88.0");
        httpRequest.AddHeader("X-Super-Properties", "eyJvcyI6IldpbmRvd3MiLCJicm93c2VyIjoiRmlyZWZveCIsImRldmljZSI6IiIsImJyb3dzZXJfdXNlcl9hZ2VudCI6Ik1vemlsbGEvNS4wIChXaW5kb3dzIE5UIDEwLjA7IFdpbjY0OyB4NjQ7IHJ2Ojg3LjApIEdlY2tvLzIwMTAwMTAxIEZpcmVmb3gvODcuMCIsImJyb3dzZXJfdmVyc2lvbiI6Ijg3LjAiLCJvc192ZXJzaW9uIjoiMTAiLCJyZWZlcnJlciI6IiIsInJlZmVycmluZ19kb21haW4iOiIiLCJyZWZlcnJlcl9jdXJyZW50IjoiIiwicmVmZXJyaW5nX2RvbWFpbl9jdXJyZW50IjoiIiwicmVsZWFzZV9jaGFubmVsIjoic3RhYmxlIiwiY2xpZW50X2J1aWxkX251bWJlciI6ODE0NTIsImNsaWVudF9ldmVudF9zb3VyY2UiOm51bGx9");

        string result = Utils.DecompressResponse(httpRequest.Get("https://discord.com/api/v9/guilds/" + guildId + "/member-verification?with_guild=false&invite_code=" + code));

        httpRequest = Utils.CreateBasicRequest();

        string toSend = "";

        if (result.Contains("\"form_fields\": []") || result.Contains("\"form_fields\":[]"))
        {
            toSend = "{\"version\": \"" + Microsoft.VisualBasic.Strings.Split(Microsoft.VisualBasic.Strings.Split(result, "{\"version\": \"")[1], "\"")[0] + "\",\"form_fields\": []}";
        }
        else
        {
            toSend = Microsoft.VisualBasic.Strings.Split(result, "}], \"description\":")[0] + ",\"response\":true}]}";
        }

        httpRequest.AddHeader("Accept", "*/*");
        httpRequest.AddHeader("Accept-Encoding", "gzip, deflate, br");
        httpRequest.AddHeader("Accept-Language", Utils.getTokenLanguage(token));
        httpRequest.AddHeader("Authorization", token);
        httpRequest.AddHeader("Connection", "keep-alive");
        httpRequest.AddHeader("Content-Length", toSend.Length.ToString());
        httpRequest.AddHeader("Content-Type", "application/json");
        httpRequest.AddHeader("Cookie", Utils.GetRandomCookie(Utils.getTokenLanguage(token)));
        httpRequest.AddHeader("DNT", "1");
        httpRequest.AddHeader("Host", "discord.com");
        httpRequest.AddHeader("Origin", "https://discord.com");
        httpRequest.AddHeader("Referer", "https://discord.com/channels/" + guildId + "/" + channelId);
        httpRequest.AddHeader("TE", "Trailers");
        httpRequest.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:88.0) Gecko/20100101 Firefox/88.0");
        httpRequest.AddHeader("X-Super-Properties", "eyJvcyI6IldpbmRvd3MiLCJicm93c2VyIjoiRmlyZWZveCIsImRldmljZSI6IiIsImJyb3dzZXJfdXNlcl9hZ2VudCI6Ik1vemlsbGEvNS4wIChXaW5kb3dzIE5UIDEwLjA7IFdpbjY0OyB4NjQ7IHJ2Ojg3LjApIEdlY2tvLzIwMTAwMTAxIEZpcmVmb3gvODcuMCIsImJyb3dzZXJfdmVyc2lvbiI6Ijg3LjAiLCJvc192ZXJzaW9uIjoiMTAiLCJyZWZlcnJlciI6IiIsInJlZmVycmluZ19kb21haW4iOiIiLCJyZWZlcnJlcl9jdXJyZW50IjoiIiwicmVmZXJyaW5nX2RvbWFpbl9jdXJyZW50IjoiIiwicmVsZWFzZV9jaGFubmVsIjoic3RhYmxlIiwiY2xpZW50X2J1aWxkX251bWJlciI6ODE0NTIsImNsaWVudF9ldmVudF9zb3VyY2UiOm51bGx9");

        httpRequest.Put("https://discord.com/api/v9/guilds/" + guildId + "/requests/@me", toSend, "application/json");
    }

    public void leaveGuild(string guildId)
    {
        try
        {
            Leaf.xNet.HttpRequest httpRequest = Utils.CreateBasicRequest();

            httpRequest.AddHeader("Accept", "*/*");
            httpRequest.AddHeader("Accept-Encoding", "gzip, deflate, br");
            httpRequest.AddHeader("Accept-Language", Utils.getTokenLanguage(token));
            httpRequest.AddHeader("Authorization", token);
            httpRequest.AddHeader("Connection", "keep-alive");
            httpRequest.AddHeader("Cookie", Utils.GetRandomCookie(Utils.getTokenLanguage(token)));
            httpRequest.AddHeader("DNT", "1");
            httpRequest.AddHeader("Host", "discord.com");
            httpRequest.AddHeader("Origin", "https://discord.com");
            httpRequest.AddHeader("Referer", "https://discord.com/channels/@me");
            httpRequest.AddHeader("TE", "Trailers");
            httpRequest.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:88.0) Gecko/20100101 Firefox/88.0");
            httpRequest.AddHeader("X-Super-Properties", "eyJvcyI6IldpbmRvd3MiLCJicm93c2VyIjoiRmlyZWZveCIsImRldmljZSI6IiIsImJyb3dzZXJfdXNlcl9hZ2VudCI6Ik1vemlsbGEvNS4wIChXaW5kb3dzIE5UIDEwLjA7IFdpbjY0OyB4NjQ7IHJ2Ojg3LjApIEdlY2tvLzIwMTAwMTAxIEZpcmVmb3gvODcuMCIsImJyb3dzZXJfdmVyc2lvbiI6Ijg3LjAiLCJvc192ZXJzaW9uIjoiMTAiLCJyZWZlcnJlciI6IiIsInJlZmVycmluZ19kb21haW4iOiIiLCJyZWZlcnJlcl9jdXJyZW50IjoiIiwicmVmZXJyaW5nX2RvbWFpbl9jdXJyZW50IjoiIiwicmVsZWFzZV9jaGFubmVsIjoic3RhYmxlIiwiY2xpZW50X2J1aWxkX251bWJlciI6ODE0NTIsImNsaWVudF9ldmVudF9zb3VyY2UiOm51bGx9");

            httpRequest.Delete("https://discord.com/api/v9/users/@me/guilds/" + guildId);
        }
        catch
        {

        }
    }
}