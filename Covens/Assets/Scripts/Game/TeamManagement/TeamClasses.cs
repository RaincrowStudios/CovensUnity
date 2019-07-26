using Newtonsoft.Json;
using System.Collections.Generic;
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
        [SerializeField] private List<TeamMemberData> members;
        [SerializeField] private List<PendingRequest> pendingRequests;
        [SerializeField] private List<PendingInvite> pendingInvites;

        public string Id { get => _id; }
        public long CreatedOn { get => createdOn; }
        public string Motto { get => motto; }
        public string Name { get => name; }
        public int School { get => school; }
        public string Dominion { get => dominion; }
        public int WorldRank { get => worldRank; }
        public int DominionRank { get => dominionRank; }
        public string CreatedBy { get => createdBy; }
        public List<TeamMemberData> Members { get => members; }
        public List<PendingRequest> PendingRequests { get => pendingRequests; }
        public List<PendingInvite> PendingInvites { get => pendingInvites; }

        [JsonIgnore]
        private TeamMemberData m_Founder = null;

        [JsonIgnore]
        public TeamMemberData Founder
        {
            get
            {
                if (m_Founder == null)
                {
                    foreach (var member in members)
                    {
                        if (member.Id == createdBy)
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
    public class TeamMemberData
    {
        [SerializeField] private string _id;
        [SerializeField] private string state;
        [SerializeField] private string name;
        [SerializeField] private string title;
        [SerializeField] private int level;
        [SerializeField] private int degree;
        [SerializeField] private int role;
        [SerializeField] private long joinedOn;
        [SerializeField] private long lastActiveOn;

        public string Id { get => _id; }
        public string State { get => state; }
        public string Name { get => name; }
        public string Title { get => title; }
        public int Level { get => level; }
        public int Degree { get => degree; }
        public CovenRole Role { get => (CovenRole)role; set => role = (int)value; }
        public long JoinedOn { get => joinedOn; }
        public long LastActiveOn { get => lastActiveOn; }


        public int School
        {
            get
            {
                if (degree < 0)
                    return -1;
                if (degree > 0)
                    return 1;
                return 0;
            }
        }
}

    [System.Serializable]
    public class PendingRequest
    {
        [SerializeField] private string character;
        [SerializeField] private string name;
        [SerializeField] private int level;
        [SerializeField] private long date;

        public string Name { get => name; }
        public int Level { get => level; }
        public string Character{ get => character; }
        public long RequestedOn { get => date; }
    }

    [System.Serializable]
    public class PendingInvite
    {
        [SerializeField] private string character;
        [SerializeField] private string name;
        [SerializeField] private int level;
        [SerializeField] private long date;

        public string Name { get => name; }
        public int Level { get => level; }
        public string Character { get => character; }
        public long RequestedOn { get => date; }
    }
}