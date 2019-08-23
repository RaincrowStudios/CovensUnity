{
    _id: "spell_addledMind",
    name: "Addled Mind",
    cost: 50,
    school: 0,
    target: true,
    self: false,
    canCrit: true,
    level: 5,
    requiredLevel: 5,
    successChance: 65,
    alignment: 0,
    xp: 15,
    cooldown: 20,
    restrictions: {
        status: ["channeling"]
    },
    effect: {
        buff: false,
        interrupt: {
            chance: "#4*caster:level",
            chanceOnCrit: 100
        }
    }
}