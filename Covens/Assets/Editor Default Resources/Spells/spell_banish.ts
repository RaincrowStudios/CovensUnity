{
    _id: "spell_banish",
    name: "Banish",
    level: 1,
    requiredLevel: 3,
    target: true,
    self: false,
    cost: 30,
    xp: 2,
    cooldown: 120,
    successChance: 65,
    canCrit: false,
    alignment: 5,
    effect: {
        buff: false,
        realocate: {
            random: true,
            latitude: 0,
            longitude: 0
        }
    },
    ingredients: [
        {
            collectible: "coll_whiteCrystal",
            count: 1,
            type: "tool"
        },
        {
            collectible: "coll_motherTear",
            count: 1,
            type: "gem"
        },
        {
            collectible: "coll_angelica",
            count: 1,
            type: "herb"
        }
    ]
}