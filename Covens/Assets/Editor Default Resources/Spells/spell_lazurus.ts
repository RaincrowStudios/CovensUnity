{
    _id: "spell_lazurus",
    name: "Lazurus",
    cost: 0,
    school: 1,
    canCrit: true,
    target: true,
    self: false,
    successChance: 65,
    xp: 15,
    cooldown: 30,
    level: 7,
    requiredLevel: 7,
    alignment: 10,
    restrictions: {
        state: "dead"
    },
    damage: {
        min: "#300",
        max: "#400"
    },
    effect: {
        modifiers: {
            resilience: "#1*caster:resilience"
        },
        duration: 180
    },
    ingredients: [
        {
            collectible: "coll_oilyFeather",
            count: 1,
            type: "tool"
        },
        {
            collectible: "coll_motherTear",
            count: 1,
            type: "gem"
        },
        {
            collectible: "coll_mugwort",
            count: 1,
            type: "herb"
        }
    ]
}