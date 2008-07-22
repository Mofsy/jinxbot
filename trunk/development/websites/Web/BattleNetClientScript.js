/// <reference name="MicrosoftAjax.js"/>
var $EnumVal = function(name, value) { this._name = name; this._value = value; };

var Diablo2CharacterClass = 
{
    Unknown : $EnumVal('Unknown', 0),
    Amazon : $EnumVal('Amazon', 1),
    Sorceress : $EnumVal('Sorceress', 2),
    Necromancer : $EnumVal('Necromancer', 3),
    Paladin : $EnumVal('Paladin', 4),
    Barbarian : $EnumVal('Barbarian', 5),
    Druid : $EnumVal('Druid', 6),
    Assassin : $EnumVal('Assassin', 7)
};

var Diablo2DifficultyLevel = 
{
    Unknown : $EnumVal('Unknown', 0),
    Normal : $EnumVal('Normal', 1),
    Nightmare : $EnumVal('Nightmare', 2),
    Hell : $EnumVal('Hell', 3),
    getByValue : function(val)
        {
            for (var i in this)
            {
                if (this[i].value)
                {
                    if (this[i].value == val)
                        return this[i];
                }
            }
            return false;
        }
}

