{
    _id: "spell_wither",
    name: "Wither",
    school: -1,
    cost: 0,
    alignment: 5,
    successChance: 65,
    requiredLevel: 7,
    cooldown: 120,
    canCrit: true,
    target: true,
    self: false,
    xp: 15,
    damage: {
        min: 80,
        max: 180
    },
    effect: {
        buff: false,
        modifiers: {
            status: ["bound"]
        },
        overtime: {
            skipInitial: false,
            tickInterval: 60,
            damage: {
                min: 300,
                max: 500
            },
            maxTicks: 1,
            tickCost: 0,
            onExpire: 0
        },
        stackable: 1,
        duration: 60
    }
}