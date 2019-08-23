{
    _id: "spell_wail",
    name: "Wail",
    cost: 45,
    school: -1,
    xp: 2,
    alignment: 5,
    cooldown: 120,
    target: true,
    self: false,
    canCrit: true,
    level: 7,
    requiredLevel: 7,
    successChance: 65,
    restrictions: {
        type: "character"
    },
    effect: {
        realocate: {
            random: true,
            latitude: 0,
            longitude: 0,
            damage: {
                min: 50,
                max: 600
            }
        }
    },
    ingredients: [
        {
            collectible: "coll_candleBlack",
            count: 1,
            type: "tool"
        },
        {
            collectible: "coll_malachite",
            count: 1,
            type: "gem"
        }
    ]
}