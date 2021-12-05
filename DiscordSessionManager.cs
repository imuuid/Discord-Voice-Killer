using System.Collections.Generic;
using System.Threading;

public class DiscordSessionManager
{
    public List<DiscordSession> sessions;

    public DiscordSessionManager()
    {
        this.sessions = new List<DiscordSession>();

        foreach (string token in Utils.GetTokensList())
        {
            this.sessions.Add(new DiscordSession(token));
        }
    }
}