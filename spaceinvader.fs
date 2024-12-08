
20 Value game-heigth
70 Value game-width
Create board game-width 2 + game-heigth * allot
36 Constant num-enemies
Create enemies num-enemies 3 * allot
100 Constant tick \ length of a tick is ms
10 Constant marco-tick \ every <value> tick is a macro-tick

: clear-window ( -- )
    10 0 u+do cr loop
;

: print-welcome ( -- )
    cr cr cr
    s" ###########################" type cr
    s" #                         #" type cr
    s" #      GFORTH INVADERS    #" type cr
    s" #                         #" type cr
    s" #         by Alex         #" type cr
    s" ###########################" type cr
    cr cr cr
;

: write-c-to-pos ( addr c x y -- addr)
    game-width * + rot dup -rot + rot swap ( addr c offset-addr )
    c! \ write char
;

: clear-board { addr } ( addr -- addr )
    addr
    \ clear board
    game-heigth 0 u+do
        game-width 0 u+do
            bl i j write-c-to-pos
        loop
    loop
    \ draw the side walls
    game-heigth 0 u+do
        '#' 0 i write-c-to-pos
        '#' game-width 1 - i write-c-to-pos
    loop
;

\ prints the game board
: print-board { addr }
    clear-window
    game-heigth 0 u+do
        game-width 0 u+do
            addr j game-width * i + + c@ emit
        loop
        cr
    loop
;

\ gets the input if not available false
: get-input ( -- c | false )
    key? if
        ekey \ read key

        \ cleanup queue
        begin
            key?
        while
            ekey drop
        repeat
    else
        false
    then
;

: sleep ( ms -- )
    1000 * utime throw +
    begin
        dup utime throw >
    while
        \ do nothing
    repeat
    drop
;

: add-player { addr x y } ( addr x y -- addr )
    addr 'o' x y write-c-to-pos addr
;

: add-enemy { addr x y type }
    type 1 = if
        addr 'T' x y write-c-to-pos
    then
    type 2 = if
        addr 'A' x y write-c-to-pos
    then
    type 3 = if
        addr 'U' x y write-c-to-pos
    then
;

: init-enemies ( -- )
    num-enemies 0 u+do
        enemies i 3 * + \ base offset
        num-enemies 3 / i > if
            dup i num-enemies 3 / mod 2 * 1 + swap c! \ store x (offset 0)
            dup 1 + 0 swap c! \ store y (offset 1)
            2 + 1 swap c! \ store type (offset 2) (type=1)
        then
        num-enemies 3 / i <= num-enemies 3 / 2 * i > and if
            dup i num-enemies 3 / mod 2 * 1 + swap c! \ store x (offset 0)
            dup 1 + 2 swap c! \ store y (offset 1)
            2 + 2 swap c! \ store type (offset 2) (type=2)
        then
        num-enemies 3 / 2 * i <= if
            dup i num-enemies 3 / mod 2 * 1 + swap c! \ store x (offset 0)
            dup 1 + 4 swap c! \ store y (offset 1)
            2 + 3 swap c! \ store type (offset 2) (type=3)
        then
    loop
;

: add-enemies { board } ( board -- board )
    num-enemies 0 u+do
        board
        enemies i 3 * + \ base offset
        dup c@ swap \ fetch x
        dup 1 + c@ swap \ fetch y
        2 + c@ \ fetch type
        .s cr
        add-enemy
    loop
    board
;

: game-init ( -- board )
    init-enemies board clear-board add-enemies
    game-width 2 / \ x pos
    game-heigth 1 - \ y pos
    add-player
;

: game-loop { board } ( board -- )
    0 \ alien position tracker
    0 \ player position tracker

    0 begin \ endless game loop

        marco-tick 0 +do
            get-input dup if
                .
            else
                drop \ no key input
            then
            tick sleep
        loop

    again
;
