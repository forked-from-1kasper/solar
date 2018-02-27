namespace Solar

module Keymap =
    [<Literal>]
    let UP = 'w'
    [<Literal>]
    let DOWN = 's'
    [<Literal>]
    let LEFT = 'a'
    [<Literal>]
    let RIGHT = 'd'
    [<Literal>]
    let Z_UP = 'e'
    [<Literal>]
    let Z_DOWN = 'q'

    [<Literal>]
    let SCALE_UP = '.'
    [<Literal>]
    let SCALE_DOWN = ','
    
    [<Literal>]
    let PLANET_NEXT = '0'
    [<Literal>]
    let PLANET_BACK = '9'
    [<Literal>]
    let PLANET_GO = '8'
    
    [<Literal>]
    let CHANGE_PROJECTION = 'p'

    [<Literal>]
    let DT_SCALE_UP = '+'
    [<Literal>]
    let DT_SCALE_DOWN = '-'

    [<Literal>]
    let PAUSE = ' '

    [<Literal>]
    let TOGGLE_INFO = 'o'

    [<Literal>]
    let TOGGLE_ORBITS = 'i'

    let isMoving (c : char) =
        List.contains c [DOWN; UP; RIGHT; LEFT;
                         Z_DOWN; Z_UP;
                         SCALE_UP; SCALE_DOWN;
                         PLANET_GO;
                         CHANGE_PROJECTION]
