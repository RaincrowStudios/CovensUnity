{
    _id: "spell_bind",
    name: "Bind",
    school: -1,
    cost: 0,
    alignment: 5,
    cooldown: 30,
    canCrit: false,
    target: true,
    requiredLevel: 2,
    level: 2,
    successChance: 65,
    self: false,
    xp: 15,
    restrictions: {
        state: "vulnerable"
    },
    effect: {
        buff: false,
        modifiers: {
            status: ["bound"]
        },
        duration: "60-180"
    }
}