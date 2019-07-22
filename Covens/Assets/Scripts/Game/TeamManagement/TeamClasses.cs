using Newtonsoft.Json;
using UnityEngine;

namespace Raincrow.Team
{
    public enum CovenRole
    {
        MEMBER = 0,
        MODERATOR = 1,
        ADMIN = 2,
        NONE = 100
    }

    [System.Serializable]
    public class TeamData
    {
        [SerializeField] private string _id;
        [SerializeField] private long createdOn;
        [SerializeField] private string motto;
        [SerializeField] private string name;
        [SerializeField] private int school;
        [SerializeField] private string dominion;
        [SerializeField] private int worldRank;
        [SerializeField] private int dominionRank;
        [SerializeField] private string createdBy;
        //[SerializeField] private int totalSilver;
        //[SerializeField] private int totalGold;
        //[SerializeField] private int totalEnergy;
        [SerializeField] private TeamMemberData[] members;

        public string Id { get => _id; }
        public long CreatedOn { get => createdOn; }
        public string Motto { get => motto; }
        public string Name { get => name; }
        public int School { get => school; }
        public string Dominion { get => dominion; }
        public int WorldRank { get => worldRank; }
        public int DominionRank { get => dominionRank; }
        public string CreatedBy { get => createdBy; }
        //public int TotalSilver { get => totalSilver; }
        //public int TotalGold { get => totalGold; }
        //public int TotalEnergy { get => totalEnergy; }
        public TeamMemberData[] Members { get => members; }

        [JsonIgnore]
        private TeamMemberData m_Founder = null;

        [JsonIgnore]
        public TeamMemberData Founder
        {
            get
            {
                if (m_Founder == null)
                {
                    foreach (var member in Members)
                    {
                        if (member.Id == CreatedBy)
                        {
                            m_Founder = member;
                            break;
                        }
                    }
                }

                return m_Founder;
            }
        }

        [JsonIgnore]
        public bool IsMember { get { return TeamManager.MyCovenId == Id; } }
    }

    [System.Serializable]
    public class TeamMemberData : System.IComparable
    {
        [SerializeField] private string _id;
        [SerializeField] private string state;
        [SerializeField] private string name;
        [SerializeField] private string title;
        [SerializeField] private int level;
        [SerializeField] private int school;
        [SerializeField] private int degree;
        [SerializeField] private int role;
        [SerializeField] private long joinedOn;
        [SerializeField] private long lastActiveOn;

        public string Id { get => _id; }
        public string State { get => state; }
        public string Name { get => name; }
        public string Title { get => title; }
        public int Level { get => level; }
        public int School { get => school; }
        public int Degree { get => degree; }
        public CovenRole Role { get => (CovenRole)role; }
        public long JoinedOn { get => joinedOn; }
        public long LastActiveOn { get => lastActiveOn; }

        public int CompareTo(object obj)
        {
            if (obj is TeamMemberData teamMemberData)
            {
                return teamMemberData.role.CompareTo(role);
            }
            return -1;
        }
    }

    [System.Serializable]
    public class TeamInviteRequest
    {
        [SerializeField] private string name;
        [SerializeField] private int level;
        [SerializeField] private int degree;
        [SerializeField] private string requestMessage;
        [SerializeField] private long requestedOn;

        public string Name { get => name; }
        public int Level { get => level; }
        public int Degree { get => degree; }
        public string RequestMessage { get => requestMessage; }
        public long RequestedOn { get => requestedOn; }
    }

    [System.Serializable]
    public class TeamInvite
    {
        [SerializeField] private string covenId;
        [SerializeField] private string displayName;
        [SerializeField] private string covenName;
        [SerializeField] private long invitedOn;
        [SerializeField] private string inviteToken;

        public string CovenId { get => covenId; }
        public string DisplayName { get => displayName; }
        public string CovenName { get => covenName; }
        public long InvitedOn { get => invitedOn; }
        public string InviteToken { get => inviteToken; }
    }
}