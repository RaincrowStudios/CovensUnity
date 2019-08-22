{
    _id: "spell_rageWard",
    name: "Rage Ward",
    cost: 500,
    school: -1,
    target: true,
    self: true,
    xp: 25,
    cooldown: 600,
    level: 6,
    requiredLevel: 6,
    alignment: 5,
    canCrit: false,
    successChance: 65,
    effect: {
        buff: true,
        modifiers: {
            power: "#2*:caster:power",
            default: 3
        },
        duration: "60*:caster:level/3"
    }
}