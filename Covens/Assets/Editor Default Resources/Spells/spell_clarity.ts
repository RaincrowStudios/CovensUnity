{
    _id: "spell_clarity",
    name: "Clarity",
    school: 0,
    alignment: 1,
    cost: 50,
    cooldown: 15,
    xp: 15,
    self: false,
    target: true,
    requiredLevel: 6,
    successChance: 65,
    canCrit: false,
    effect: {
        buff: true,
        modifiers: {
            hitChance: "#1*caster:level/3"
        },
        duration: "60*caster:level"
    }
}