{
    _id: "spell_channeling",
    name: "Channeling",
    cost: 5,
    school: 0,
    target: false,
    self: true,
    canCrit: false,
    level: 3,
    requiredLevel: 3,
    successChance: 100,
    alignment: 0,
    xp: 0,
    cooldown: 60,
    effect: {
        buff: true,
        modifiers: {
            status: ["channeling"]
        },
        overtime: {
            skipInitial: false,
            effect: {
                buff: true,
                modifiers: {
                    power: "#1",
                    resilience: "#1",
                    toCrit: "#1",
                },
                expiresOn: "cast",
                stackable: 0
            },
            tickInterval: 0.5,
            maxTicks: "#1*caster:level/1",
            tickCost: 5,
        },
        duration: "#1*caster:level/1"
    }
}