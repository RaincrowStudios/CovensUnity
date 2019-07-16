using Newtonsoft.Json;
using System.Collections.Generic;

public class TeamData
{
    public double createdOn { get; set; }
    public string motto { get; set; }
    public string coven { get; set; }
    public double disbandedOn { get; set; }
    public string covenName { get; set; }
    public string dominion { get; set; }
    public int rank { get; set; }
    public int score { get; set; }
    public int dominionRank { get; set; }
    public string createdBy { get; set; }
    public int totalSilver { get; set; }
    public int totalGold { get; set; }
    public int totalEnergy { get; set; }
    public int controlledLocations { get; set; }
    public List<TeamMemberData> members { get; set; }

    [JsonIgnore]
    public int Degree
    {
        get
        {
            int result = 0;
            for (int i = 0; i < members.Count; i++)
            {
                result += members[i].degree;
            }
            return result;
        }
    }

    [JsonIgnore]
    public int CreatorDegree
    {
        get
        {
            for (int i = 0; i < members.Count; i++)
            {
                if (members[i].displayName == createdBy)
                {
                    return members[i].degree;
                }
            }
            return 0;
        }
    }
}

public class TeamMemberData
{
    public string state { get; set; }
    public string displayName { get; set; }
    public string title { get; set; }
    public int level { get; set; }
    public int degree { get; set; }
    public int role { get; set; }
    public double joinedOn { get; set; }
    public double lastActiveOn { get; set; }
}

public class TeamInviteRequest
{
    public string displayName { get; set; }
    public int level { get; set; }
    public int degree { get; set; }
    public string request { get; set; }
    public long requestedOn { get; set; }
}

public class TeamInvite
{
    public string covenId { get; set; }
    public string displayName { get; set; }
    public string covenName { get; set; }
    public long invitedOn { get; set; }
    public string inviteToken { get; set; }
}