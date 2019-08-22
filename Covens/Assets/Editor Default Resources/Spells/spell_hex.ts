{
    _id: "spell_hex",
    name: "Hex",
    canCrit: true,
    successChance: 65,
    school: -1,
    cost: 0,
    level: 1,
    cooldown: 5,
    requiredLevel: 1,
    target: true,
    alignment: 0,
    xp: 2,
    self: false,
    damage: {
        min: 10,
        max: 30
    },
    effect: {
        buff: false,
        modifiers: {
            beCrit: "#1*caster:power"
        },
        stackable: 3,
        duration: "60*caster:level/1",
        overtime: {
            skipInitial: true,
            tickInterval: 60,
            damage: {
                min: 10,
                max: 30
            },
            maxTicks: "#3",
            tickCost: 0,
            onExpire: 0
        }
    }
}