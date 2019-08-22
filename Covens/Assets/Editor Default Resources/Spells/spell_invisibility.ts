{
    _id: "spell_invisibility",
    name: "Invisibility",
    alignment: 10,
    school: 0,
    cost: 100,
    successChance: 100,
    self: true,
    target: true,
    level: 1,
    requiredLevel: 1,
    cooldown: 120,
    xp: 15,
    effect: {
        buff: true,
        modifiers: {
            status: ["invisible"]
        },
        stackable: 1,
        duration: 0
    },
    ingredients: [
        {
            collectible: "coll_rainwater",
            count: 1,
            type: "tool"
        },
        {
            collectible: "coll_eyeBright",
            count: 1,
            type: "herb"
        },
        {
            collectible: "coll_brimstone",
            count: 1,
            type: "gem"
        }
    ]
}