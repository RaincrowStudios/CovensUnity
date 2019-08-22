{
    _id: "spell_bless",
    name: "Bless",
    school: 1,
    alignment: 0,
    cost: 0,
    canCrit: true,
    level: 1,
    target: true,
    successChance: 65,
    requiredLevel: 1,
    self: false,
    cooldown: 5,
    xp: 2,
    damage: {
        min: "#30*caster:level/1",
        max: "#50*caster:level/1"
    },
    effect: {
        buff: true,
        modifiers: {
            resilience: "#1*caster:level/3"
        },
        duration: "60*caster:level/1",
        stackable: 3
    }
}