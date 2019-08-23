{
    _id: "spell_foolBargain",
    name: "Fool's Bargain",
    level: 3,
    requiredLevel: 9,
    self: false,
    school: 0,
    xp: 15,
    cost: 0,
    alignment: 2,
    target: true,
    cooldown: 140,
    canCrit: true,
    successChance: 100,
    effect: {
        buff: true,
        overtime: {
            skipInitial: false,
            tickInterval: 20,
            damage: {
                min: "#0",
                max: "#100"
            },
            maxTicks: "#6",
            tickCost: 0,
            onExpire: {
                damage: {
                    min: 100,
                    max: 400
                }
            }
        }
    }
}