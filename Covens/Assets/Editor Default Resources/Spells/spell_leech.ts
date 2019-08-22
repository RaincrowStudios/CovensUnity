{
    _id: "spell_leech",
    name: "Leech",
    school: -1,
    cost: 0,
    alignment: 5,
    xp: 25,
    cooldown: 1200,
    level: 4,
    requiredLevel: 5,
    self: false,
    target: true,
    canCrit: true,
    damage: {
        min: 200,
        max: 400
    },
    effect: {
        buff: true,
        lifesteal: 0.25
    }
}