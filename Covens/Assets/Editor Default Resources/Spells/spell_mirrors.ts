{
    _id: "spell_mirrors",
    name: "Mirrors",
    level: 9,
    requiredLevel: 5,
    school: 0,
    alignment: 2,
    successChance: 100,
    xp: 30,
    cost: 100,
    cooldown: 300,
    self: true,
    target: false,
    canCrit: false,
    restrictions: {
        targetSchool: 0
    },
    effect: {
        illusion: {
            clone: "#1*caster:level/2",
            distance: 50, // meters
            modfiers: {
                status: ["mirrored"]
            },
            duration: 300
        }
    }
}