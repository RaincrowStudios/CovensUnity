{
    _id: "spell_reflectiveWard",
    name: "Reflective Ward",
    school: 1,
    cost: 200,
    target: true,
    self: true,
    alignment: 5,
    xp: 15,
    canCrit: false,
    cooldown: 600,
    requiredLevel: 6,
    successChance: 65,
    effect: {
        buff: true,
        modifiers: {
            status: ["reflective"]
        },
        stackable: 1,
        duration: "60*caster:level/3"
    },
    ingredients: [
        {
            collectible: "coll_whiteFur",
            count: 1,
            type: "tool"
        },
        {
            collectible: "coll_motherTear",
            count: 1,
            type: "gem"
        },
        {
            collectible: "coll_orrisRoot",
            count: 1,
            type: "herb"
        }
    ]
}