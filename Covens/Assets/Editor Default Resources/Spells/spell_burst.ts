{
    _id: "spell_burst",
    name: "Burst",
    level: 3,
    requiredLevel: 3,
    cost: 80,
    xp: 5,
    cooldown: 15,
    successChance: 65,
    target: true,
    self: false,
    canCrit: true,
    alignment: 5,
    damage: {
        min: 300,
        max: 380,
        critChance: "#50"
    },
    ingredients: [
        {
            collectible: "coll_hourglass",
            count: 1,
            type: "tool"
        },
        {
            collectible: "coll_motherTear",
            count: 1,
            type: "gem"
        },
        {
            collectible: "coll_hyssop",
            count: 1,
            type: "herb"
        }
    ]
}